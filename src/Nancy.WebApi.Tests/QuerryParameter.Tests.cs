using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Helpers;
using Nancy.Testing;
using Xunit;

namespace Nancy.WebApi.Tests
{
    public class QuerryParameterTests
    {
        private readonly Browser _browser;

        public QuerryParameterTests()
        {
            var bootstrapper = typeof (QueryParameterModule).CreateConfigurableBootstrapper();
            _browser = new Browser(bootstrapper);
        }

        [Fact]
        public void CanSendInt()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendInt).GetQueryParameterModuleUrl(), 1);
        }

        [Fact]
        public void CanSendString()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendString).GetQueryParameterModuleUrl(), "abc");
        }

        [Fact]
        public void CanSendDouble()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendDouble).GetQueryParameterModuleUrl(), 12.12d);
        }

        [Fact]
        public void CanSendDate()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendDate).GetQueryParameterModuleUrl(), new DateTime(1999, 12, 12));
        }

        [Fact]
        public void CanSendEnumByValue()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendEnumByValue).GetQueryParameterModuleUrl(),
                Gender.Male.GetHashCode(),
                (response, expected) =>
                {
                    var gender = response.Body.Deserialize<Gender>();
                    Assert.Equal(expected, gender.GetHashCode());
                });
        }

        [Fact]
        public void CanSendEnumByName()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendEnumByName).GetQueryParameterModuleUrl(), Gender.Male);
        }

        [Fact]
        public void CanSendIntArray()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendIntArray).GetQueryParameterModuleUrl(), new[] { 1, 2, 3 });
        }

        [Fact]
        public void CanSendIntList()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendIntList).GetQueryParameterModuleUrl(), new List<int> { 1, 2, 3 });
        }

        [Fact]
        public void CanSendByteArray()
        {
            var expected = Encoding.ASCII.GetBytes("dummie content");
            var bodyInput = Config.JsonSerializer.Serialize(expected);
            var encodedUrl = HttpUtility.UrlEncode(nameof(QueryParameterModule.SendByeArray).GetQueryParameterModuleUrl() + bodyInput);

            var response = _browser.Get(encodedUrl);

            Assert.Equal(expected, response.Body.ToArray());
        }

        [Fact]
        public void CanSendStringKeyStringValueDictionary()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendStringKeyStringValueDictionary).GetQueryParameterModuleUrl(), new Dictionary<string, string> { ["1"] = "a", ["2"] = "b" });
        }

        [Fact]
        public void CanSendIntKeyStringValueDictionary()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendStringKeyStringValueDictionary).GetQueryParameterModuleUrl(), new Dictionary<int, string> { [1] = "a", [2] = "b" });
        }

        [Fact]
        public void CanSendStringKeyComplexValueDictionary()
        {
            AssertExpectedValueTypeSent(nameof(QueryParameterModule.SendStringKeyComplexValueDictionary).GetQueryParameterModuleUrl(),
                new Dictionary<string, User> { ["1"] = new User { Id = 1 }, ["2"] = new User { Id = 2 } },
                (response, expected) =>
                {
                    var result = response.Body.Deserialize<Dictionary<string, User>>();
                    Assert.Equal(expected.Count, result.Count);
                });
        }

        [Fact]
        public void CanUseSlashAndAmpSeperatedQueries()
        {
            var testResult = new TestBodyInput { I1 = "I1 string value", I2 = 2 };
            var p1 = 1;
            var p2 = new DateTime(1923, 12, 12);
            var p3 = "abc";
            var gender = Gender.Male;
            var date = new DateTime(1922, 1, 1);

            var url = nameof(QueryParameterModule.SendMixedQueryParameters).GetQueryParameterModuleUrl() + $"{ToJson(p1)}/{ToJson(p2)}/{ToJson(p3)}?gender={gender}&date={ToJson(date)}";

            var encodedUrl = HttpUtility.UrlEncode(url);
            var response = _browser.Get(encodedUrl, ctx => { ctx.Body(ToJson(testResult)); });
            var result = response.Body.AsString();

            var expected = $"{p1},{p2},{p3},{gender},{date.ToString("yyyy-MM-dd")}";

            Assert.Equal(expected, result);
        }


        private void AssertExpectedValueTypeSent<T>(string url, T expected, Action<BrowserResponse, T> assertAction = null)
        {
            var bodyInput = Config.JsonSerializer.Serialize(expected);

            var encodedUrl = HttpUtility.UrlEncode(url + bodyInput);
           
            var response = _browser.Get(encodedUrl);

            if (assertAction != null)
                assertAction(response, expected);
            else
            {
                if (expected is string)
                    Assert.Equal(expected.ToString(), response.Body.AsString());
                else
                    Assert.Equal(expected, response.Body.Deserialize<T>());
            }
        }

        private string ToJson(object obj)
        {
            return Config.JsonSerializer.Serialize(obj);
        }
    }
}
