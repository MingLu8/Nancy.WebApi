using System;
using System.Linq;
using Nancy.WebApi.AttributeRouting;
using Xunit;

namespace Nancy.WebApi.Tests
{
    public class DefaultModuleRouteExtractorTests
    {
        public class NonAttributeRoutingClass
        {
            
        }

        class NonPublicApiControllerClass:IApiController
        {

        }

        public abstract class AbstractApiControllerClass : IApiController
        {

        }

        public class QualifiedApiControllerClass : IApiController
        {

        }

        public class QualifiedApiControllerClassWithMethods : IApiController
        {
            [HttpGet("")]
            public void QualifiedMethod_WithGet()
            {
                
            }

            [HttpPost("")]
            public void QualifiedMethod_WithPost()
            {

            }

            [HttpPut("")]
            public void QualifiedMethod_WithPut()
            {

            }

            [HttpDelete("")]
            public void QualifiedMethod_WithDelete()
            {

            }

            [HttpPatch("")]
            public void QualifiedMethod_WithPatch()
            {

            }

            [HttpOptions("")]
            public void QualifiedMethod_WithOptions()
            {

            }

            [HttpGet("")]
            internal void NonQualifiedMethod_NotAPublic_Method()
            {

            }

            public void NonQualifiedMethod_MissingRoutingAttribute()
            {

            }
        }


        [Theory]
        [InlineData(typeof(NonAttributeRoutingClass))]
        [InlineData(typeof(NonPublicApiControllerClass))]
        [InlineData(typeof(AbstractApiControllerClass))]
        public void ShouldNotBeQualifiedType(Type notQualifiedType)
        {
            var routeExtractor = new DefaultModuleRouteExtractor();
            var qualified = routeExtractor.IsQualifiedType(notQualifiedType);
            Assert.False(qualified);
        }

        [Fact]
        public void PublicConcreteTypeOfIAttributeRouting_ShouldBeQualifiedType()
        {
            var routeExtractor = new DefaultModuleRouteExtractor();
            var qualified = routeExtractor.IsQualifiedType(typeof(QualifiedApiControllerClass));
            Assert.True(qualified);
        }

        [Fact]
        public void OnlyQualifiedMethods_ShouldBeReturned()
        {
            var routeExtractor = new DefaultModuleRouteExtractor();
            var methods = routeExtractor.FindQualifiedMethods(typeof(QualifiedApiControllerClassWithMethods));
            Assert.True(methods.All(a=> a.Name.StartsWith("QualifiedMethod")));
        }

        [Fact]
        public void QualifiedTypeWithQualifiedNumberOfMethods_ShouldBeReturn_TheSameNumberOfRouteInfos()
        {
            var routeExtractor = new DefaultModuleRouteExtractor();
            var methods = routeExtractor.FindQualifiedMethods(typeof(QualifiedApiControllerClassWithMethods));

            var routeInfos = routeExtractor.ExtractRoutes(typeof(QualifiedApiControllerClassWithMethods));
            Assert.Equal(methods.Count, routeInfos.Count);
        }
    }
}
