using Nancy.Responses;
using Nancy.Testing;
using Nancy.WebApi.AttributeRouting;
using Xunit;

namespace Nancy.WebApi.Tests
{
    [RoutePrefix("api/responseMessage")]
    public class ResponseMessageModule : ApiController
    {
        [HttpGet(nameof(WithStatusCode))]
        public ResponseMessage<Address> WithStatusCode()
        {
            var result = new Address {City = "dummie city"};
            return new ResponseMessage<Address>(() => new JsonResponse(result, new JsonNetSerializer()).WithStatusCode(HttpStatusCode.NotModified));
        }
    }

    public class ResponseMessageTests
    {
        private Browser _browser;

        [Fact]
        public void ResponseMessage_Returns_StatusCode()
        {
            var bootstrapper = typeof(ResponseMessageModule).CreateConfigurableBootstrapper();
            _browser = new Browser(bootstrapper);

            var url = nameof(ResponseMessageModule.WithStatusCode).GetReponseMessageModuleUrl();
            var response = _browser.Get(url);
            var result = response.Body.Deserialize<Address>();
            Assert.Equal(HttpStatusCode.NotModified, response.StatusCode);
            Assert.Equal("dummie city", result.City);
        }
    }
}
