using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nancy.WebApi.AttributeRouting;

namespace Nancy.WebApi
{
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Options,
        Patch
    }

    public class RouteInfo
    {
        IEnumerable<ParameterInfo> _queryParameters;
        ParameterInfo _bodyParameter;

        public ParameterInfo ReturnParameter => Method.ReturnParameter;
        public ParameterInfo BodyParameter => _bodyParameter ?? (_bodyParameter = GetBodyParameter());
        public IEnumerable<ParameterInfo> QueryParameters => _queryParameters ?? (_queryParameters = GetQueryParameters());

        public MethodInfo Method { get; set; }
        public HttpMethod HttpMethod { get; set; }

        public string Path { get; set; }

        public string Name => $"{HttpMethod} {Path}";

        public RouteInfo(MethodInfo method)
        {
            Method = method;
            ParseAttributes(method);
        }

        private void ParseAttributes(MethodInfo method)
        {
            var attribute = GetRouteAttribute(method);
            if (attribute != null)
            {
                var routePrefix = GetRoutePrefix(method);
                Path = ConstructPath(routePrefix, attribute.Path);
                HttpMethod = attribute.HttpMethod;
            }
        }

        private static string ConstructPath(string routePrefix, string routePath)
        {
            routePrefix = CleanPath(routePrefix);
            routePath = CleanPath(routePath);
            return CleanPath($"{routePrefix}/{routePath}");
        }

        private static string GetRoutePrefix(MethodBase method)
        {
            var routePrefixAttribute = method.DeclaringType.GetCustomAttributes(typeof (RoutePrefixAttribute), false).FirstOrDefault() as RoutePrefixAttribute;
            var routePrefix = routePrefixAttribute == null ? "" : routePrefixAttribute.Prefix;
            return routePrefix;
        }

        private static RouteAttributeBase GetRouteAttribute(MethodInfo method)
        {
            var attribute = method.GetCustomAttributes(typeof (RouteAttributeBase), false).FirstOrDefault() as RouteAttributeBase;
            return attribute;
        }

       
        private ParameterInfo GetBodyParameter()
        {
            return Method.GetParameters().FirstOrDefault(a => !QueryParameters.Contains(a));
        }

        private IEnumerable<string> GetQueryParameterNames()
        {
            var pathParts = Path.Split(new [] {'?'}, StringSplitOptions.RemoveEmptyEntries);

            var slashSeperateQueryPath = pathParts[0];
            var result = slashSeperateQueryPath.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries).Where(a => !a.Contains('/')).ToList();
            if (pathParts.Length > 1)
            {
                var ampSeperatedQuerysPath = pathParts[1];
                var ampSeperatedQuerys = ampSeperatedQuerysPath.Split(new[] { '=', '&' }, StringSplitOptions.RemoveEmptyEntries).Where(a=>!a.Contains('{'));
                result.AddRange(ampSeperatedQuerys);
            }
            return result;
        }
        private IEnumerable<ParameterInfo> GetQueryParameters()
        {
            var queryParameterName = GetQueryParameterNames();
            return Method.GetParameters().Where(a => queryParameterName.Contains(a.Name));
        }
        public static string CleanPath(string path)
        {
            return (path ?? string.Empty).Trim('/');
        }
    }
}
