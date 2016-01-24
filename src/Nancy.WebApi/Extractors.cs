using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Nancy.WebApi.AttributeRouting;

namespace Nancy.WebApi
{
    public interface IModuleRouteExtractor
    {
        List<RouteInfo> ExtractRoutes(Type type);
        List<MethodInfo> FindQualifiedMethods(Type type);
        bool IsQualifiedType(Type type);
    }

    public interface IMethodValueExtractor
    {
        object[] GetParameterValues(MethodInfo method, IDictionary<string, object> quaryParameters, NancyContext context);
    }

    public class DefaultModuleRouteExtractor : IModuleRouteExtractor
    {
        private static readonly ConcurrentDictionary<Type, List<RouteInfo>> RouteTable = new ConcurrentDictionary<Type, List<RouteInfo>>();
        public virtual List<RouteInfo> ExtractRoutes(Type type)
        {

            List<RouteInfo> routeInfos;
            if (RouteTable.TryGetValue(type, out routeInfos))
                return routeInfos;

            if (!IsQualifiedType(type)) return new List<RouteInfo>();

            routeInfos = RouteTable.GetOrAdd(type, new List<RouteInfo>());

            var methods = FindQualifiedMethods(type);
            routeInfos.AddRange(methods.Select(a => new RouteInfo(a)));

            return routeInfos;
        }

        public virtual List<MethodInfo> FindQualifiedMethods(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).
                Where(a => a.GetCustomAttributes(typeof(RouteAttributeBase)).Any()).ToList();
            return methods;
        }

        public virtual bool IsQualifiedType(Type type)
        {
            return !type.IsAbstract && (type.IsPublic || type.IsNestedPublic) && typeof(IApiController).IsAssignableFrom(type);
        }
    }

    public class DefaultMethodValueExtractor : IMethodValueExtractor
    {
        public object[] GetParameterValues(MethodInfo method, IDictionary<string, object> quaryParameters, NancyContext context)
        {
            return method.GetParameters().Select(methodParameterInfo => ResolveMethodParameter(methodParameterInfo, quaryParameters, context)).ToArray();
        }

        private object ResolveMethodParameter(ParameterInfo methodParameterInfo, IDictionary<string, object> queryParameters, NancyContext context)
        {
            if (IsQueryParameter(methodParameterInfo, queryParameters))
                return GetValueFromQueryParameters(methodParameterInfo, queryParameters);

            return GetValueFromRequestBody(methodParameterInfo, context);
        }

        private object GetValueFromRequestBody(ParameterInfo methodParameterInfo, NancyContext context)
        {
            if (methodParameterInfo.ParameterType == typeof(Image))
                return CreateImage(context.Request, methodParameterInfo);

            if (methodParameterInfo.ParameterType == typeof(HttpFile[]))
                return context.Request.Files;


            if (methodParameterInfo.ParameterType == typeof(byte[]))
                return CreateByteArray(context.Request);

            return CreateValueFromJson(methodParameterInfo, context);
        }

        private static object CreateByteArray(Request request)
        {
            using (var memoryStream = new MemoryStream())
            {
                request.Body.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private object CreateImage(Request request, ParameterInfo parameterInfo)
        {
            try
            {
                return Image.FromStream(request.Body);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot create parameter {parameterInfo.Name} of type: {parameterInfo.ParameterType} with request body.   " + ex.Message);
            }
        }

        private object CreateValueFromJson(ParameterInfo parameterInfo, NancyContext context)
        {
            string requestBody = string.Empty;
            try
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    requestBody = reader.ReadToEnd();
                    return ParseSimpleValue(requestBody, parameterInfo.ParameterType);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot deserialize parameter {parameterInfo.Name} of type: {parameterInfo.ParameterType}, with request body of: {requestBody}");
            }
        }

        private object GetValueFromQueryParameters(ParameterInfo parameterInfo, IDictionary<string, object> queryParameters)
        {
            return ParseSimpleValue(queryParameters[parameterInfo.Name].ToString(), parameterInfo.ParameterType);
        }

        private object ParseSimpleValue(string stringValue, Type type)
        {
            return Config.JsonSerializer.Deserialize(stringValue, type);
        }

        private static bool IsQueryParameter(ParameterInfo parameterInfo, IDictionary<string, object> queryParameters)
        {
            return queryParameters.ContainsKey(parameterInfo.Name);
        }
    }
}
