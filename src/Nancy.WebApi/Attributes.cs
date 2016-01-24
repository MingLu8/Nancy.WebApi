using System;

namespace Nancy.WebApi.AttributeRouting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RoutePrefixAttribute : Attribute
    {
        public RoutePrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class RouteAttributeBase : Attribute
    {
        protected RouteAttributeBase(string path, HttpMethod httpMethod)
        {
            Path = path.TrimStart('/');
            HttpMethod = httpMethod;
        }

        public HttpMethod HttpMethod { get; }
        public string Path { get; }
    }

    public sealed class HttpPutAttribute : RouteAttributeBase
    {        
        public HttpPutAttribute(string path) : base(path, HttpMethod.Put)
        {
        }
    }

    public sealed class HttpPostAttribute : RouteAttributeBase
    {       
        public HttpPostAttribute(string path) : base(path, HttpMethod.Post)
        {
        }
    }

    public sealed class HttpPatchAttribute : RouteAttributeBase
    {       
        public HttpPatchAttribute(string path) : base(path, HttpMethod.Patch)
        {
        }
    }

    public sealed class HttpOptionsAttribute : RouteAttributeBase
    {       
        public HttpOptionsAttribute(string path) : base(path, HttpMethod.Options)
        {
        }
    }

    public sealed class HttpGetAttribute : RouteAttributeBase
    {       
        public HttpGetAttribute(string path) : base(path, HttpMethod.Get)
        {
        }
    }

    public sealed class HttpDeleteAttribute : RouteAttributeBase
    {      
        public HttpDeleteAttribute(string path)
            : base(path, HttpMethod.Delete)
        {
        }
    }
}

