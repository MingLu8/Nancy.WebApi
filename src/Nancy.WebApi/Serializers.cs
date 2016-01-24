using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nancy.WebApi
{
    public class JsonNetBodyDeserializer : IBodyDeserializer
    {
        private IJsonSerializer _serializer;
        public virtual IJsonSerializer Serializer => _serializer ?? (_serializer = Config.JsonSerializer);

        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="context">Current <see cref="BindingContext"/>.</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(string contentType, BindingContext context)
        {
            return Config.IsJsonContentType(contentType);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            if (context.DestinationType.IsPrimitive)
            {
                var bodyText = new StreamReader(bodyStream).ReadToEnd();

                return TypeDescriptor.GetConverter(context.DestinationType).ConvertFromString(bodyText);
            }

            var deserializedObject =
                Serializer.Deserialize(new StreamReader(bodyStream), context.DestinationType);

            var properties =
                context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new BindingMemberInfo(p));

            var fields =
                context.DestinationType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Select(f => new BindingMemberInfo(f));

            if (context.ValidModelBindingMembers != null &&
                properties.Concat(fields).Except(context.ValidModelBindingMembers).Any())
            {
                return CreateObjectWithBlacklistExcluded(context, deserializedObject);
            }

            return deserializedObject;
        }

        private static object ConvertCollection(object items, Type destinationType)
        {
            var returnCollection = Activator.CreateInstance(destinationType);

            var collectionAddMethod =
                destinationType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

            foreach (var item in (IEnumerable) items)
            {
                collectionAddMethod.Invoke(returnCollection, new[] {item});
            }

            return returnCollection;
        }

        private static object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
        {
            var returnObject = Activator.CreateInstance(context.DestinationType, true);

            if (context.DestinationType.IsCollection())
            {
                return ConvertCollection(deserializedObject, context.DestinationType);
            }

            foreach (var property in context.ValidModelBindingMembers)
            {
                CopyPropertyValue(property, deserializedObject, returnObject);
            }

            return returnObject;
        }

        private static void CopyPropertyValue(BindingMemberInfo property, object sourceObject, object destinationObject)
        {
            property.SetValue(destinationObject, property.GetValue(sourceObject));
        }

    }

    public static class Config
    {
        private static IJsonSerializer _jsonSerializer = new DefaultJsonSerializer();

        public static IJsonSerializer JsonSerializer => _jsonSerializer;

        public static void SetJsonSerializer(IJsonSerializer jsonSerializer)
        {
            if (jsonSerializer == null) throw new Exception("jsonSerializer is null.");
            _jsonSerializer = jsonSerializer;
        }

        public static Func<string, bool> IsJsonContentType = contentType =>
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.Equals("text/json", StringComparison.InvariantCultureIgnoreCase) ||
                   (contentMimeType.StartsWith("application/vnd", StringComparison.InvariantCultureIgnoreCase) &&
                    contentMimeType.EndsWith("+json", StringComparison.InvariantCultureIgnoreCase));        
        };      
    }

    public interface IJsonSerializer
    {
        JsonSerializerSettings Settings { get; }
        JsonSerializer Serializer { get; }
        string Serialize(object obj, Formatting formatting = Formatting.None, JsonSerializerSettings serializerSettings = null);
        void Serialize(JsonTextWriter writer, object obj);
        void Serialize(TextWriter writer, object obj);
        T Deserialize<T>(string json);
        object Deserialize(string json, Type type);
        object Deserialize(StreamReader reader, Type type);
    }

    public class DefaultJsonSerializer : IJsonSerializer
    {

        public virtual JsonSerializerSettings Settings => _settings;
        private JsonSerializer _serializer;
        private readonly JsonSerializerSettings _settings;

        public virtual JsonSerializer Serializer => _serializer ?? (_serializer = JsonSerializer.Create(Settings));

        public DefaultJsonSerializer()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new IsoDateTimeConverter());
        }
        public virtual string Serialize(object obj, Formatting formatting = Formatting.None, JsonSerializerSettings serializerSettings = null)
        {
            if (IsSimpleType(obj.GetType())) return obj.ToString();
            return JsonConvert.SerializeObject(obj, formatting, serializerSettings ?? Settings);
        }

        public virtual void Serialize(JsonTextWriter writer, object obj)
        {
            if (IsSimpleType(obj.GetType()))
                writer.WriteRaw(obj.ToString());
            else
                Serializer.Serialize(writer, obj);
        }

        public virtual void Serialize(TextWriter writer, object obj)
        {
            if (IsSimpleType(obj.GetType()))
                writer.Write(obj.ToString());
            else
                Serializer.Serialize(writer, obj);
        }

        public T Deserialize<T>(string json)
        {
            return (T)Deserialize(json, typeof(T));
        }

        public object Deserialize(string json, Type type)
        {
            try
            {
                var typeConverter = TypeDescriptor.GetConverter(type);
                return typeConverter.ConvertFromString(json);
            }
            catch
            {
                return JsonConvert.DeserializeObject(json, type);
            }
        }

        public object Deserialize(StreamReader reader, Type type)
        {
            return Deserialize(reader.ReadToEnd(), type);
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type.IsEnum;
        }
    }

    public class JsonNetSerializer : ISerializer
    {
        private IJsonSerializer _serializer;
        public virtual IJsonSerializer Serializer => _serializer ?? (_serializer = Config.JsonSerializer);

        public bool CanSerialize(string contentType)
        {
            return Config.IsJsonContentType(contentType);
        }

        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(new UnclosableStreamWrapper(outputStream))))
            {
                Serializer.Serialize(writer, model);
            }
        }

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
        public IEnumerable<string> Extensions
        {
            get { yield return "json"; }
        }
    }
}
