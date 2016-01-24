using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Nancy.WebApi
{
    public interface IApiController
    {
    }

    /// <summary>
    /// only primative types, string, date, and enum are supported as input query parameters.
    /// </summary>
    public abstract class ApiController : NancyModule, IApiController
    {
        private readonly IMethodValueExtractor _methodValueExtractor;
        private readonly IModuleRouteExtractor _moduleRouteExtractor;

        protected ApiController(IModuleRouteExtractor moduleRouteExtractor = null, IMethodValueExtractor methodValueExtractor = null)
        {
            _moduleRouteExtractor = moduleRouteExtractor ?? new DefaultModuleRouteExtractor();
            _methodValueExtractor = methodValueExtractor ?? new DefaultMethodValueExtractor();

            SetupRoutes();
        }

        private void SetupRoutes()
        {
            var routeInfos = _moduleRouteExtractor.ExtractRoutes(GetType());
            routeInfos.ForEach(a => {
                var builder = GetRouteBuilder(a.HttpMethod);
                builder[a.Name, a.Path] = CreateRouteHandler(a.Method);
            });
        }

        private RouteBuilder GetRouteBuilder(HttpMethod httpMethod)
        {
            switch (httpMethod)
            {
                case HttpMethod.Get:
                    return Get;
                case HttpMethod.Delete:
                    return Delete;
                case HttpMethod.Options:
                    return Options;
                case HttpMethod.Patch:
                    return Patch;
                case HttpMethod.Post:
                    return Post;
                case HttpMethod.Put:
                    return Put;
                default:
                    throw new Exception("Not a valid http method.");
            }
        }
      
        private Func<dynamic, dynamic> CreateRouteHandler(MethodInfo method)
        {
            return dynamicParameters =>
            {
                object result = InvokeRouteHandlerMethod(method, dynamicParameters.ToDictionary());
                return CreateResponse(result);
            };
        }

        private dynamic CreateResponse(object result)
        {
            if (result is string)
                return CreateStringResponse(result);

            if (result is Image)
                return CreateImageResponse(result);

            if (result is byte[])
                return CreateByteArrayResponse(result);

            return CreateJsonResponse(result);
        }

        private object CreateJsonResponse(object result)
        {
            return Response.AsJson(result);
            //return new Response
            //{
            //    ContentType = "application/json",
            //    Contents = s =>
            //    {
            //       var json = JsonConvert.SerializeObject(result);
            //       var bytes = Encoding.ASCII.GetBytes(json);
            //       s.Write(bytes, 0, bytes.Length);
            //    }
            //};
        }

        private static dynamic CreateByteArrayResponse(object result)
        {
            return new Response
            {
                ContentType = "application/octet-stream",
                Contents = s =>
                {
                    var bytes = (byte[]) result;
                    s.Write(bytes, 0, bytes.Length);
                }
            };
        }

        private dynamic CreateImageResponse(object result)
        {           
            return new Response
            {
                ContentType = "application/octet-stream",
                Contents = s =>
                {
                    var image = result as Image;
                    image?.Save(s, image.RawFormat);
                }
            };
        }

        private dynamic CreateStringResponse(object result)
        {
            return Response.AsText(result.ToString());           
        }

        private object InvokeRouteHandlerMethod(MethodInfo method, IDictionary<string, object> quaryParameters)                        
        {
            object[] parameterValues = _methodValueExtractor.GetParameterValues(method, quaryParameters, Context);                         
            return method.Invoke(this, parameterValues);
        }      
    }
}
