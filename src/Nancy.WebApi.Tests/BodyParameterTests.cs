using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Nancy.Helpers;
using Nancy.Testing;
using Xunit;

namespace Nancy.WebApi.Tests
{
    public class BodyParameterTests
    {
        private readonly Browser _browser;

        public BodyParameterTests()
        {
            var bootstrapper = typeof(BodyParameterModule).CreateConfigurableBootstrapper();
            _browser = new Browser(bootstrapper);
        }

        [Fact]
        public void CanSendInt()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendInt).GetBodyParameterModuleUrl(), 1);
        }

        [Fact]
        public void CanSendString()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendString).GetBodyParameterModuleUrl(), "abc");
        }

        [Fact]
        public void CanSendDouble()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendDouble).GetBodyParameterModuleUrl(), 12.12d);
        }

        [Fact]
        public void CanSendDate()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendDate).GetBodyParameterModuleUrl(), new DateTime(1999, 12, 12));
        }

        [Fact]
        public void CanSendEnumByValue()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendEnumByValue).GetBodyParameterModuleUrl(), 
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
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendEnumByName).GetBodyParameterModuleUrl(), Gender.Male);
        }

        [Fact]
        public void CanSendIntArray()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendIntArray).GetBodyParameterModuleUrl(), new[] { 1, 2, 3 });
        }

        [Fact]
        public void CanSendIntList()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendIntList).GetBodyParameterModuleUrl(), new List < int > { 1, 2, 3 });
        }

        [Fact]
        public void CanSendByteArray()
        {           
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendByeArray).GetBodyParameterModuleUrl(), Encoding.ASCII.GetBytes("dummie content"));
        }

        [Fact]
        public void CanSendStringKeyStringValueDictionary()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendStringKeyStringValueDictionary).GetBodyParameterModuleUrl(), new Dictionary<string, string>{["1"] = "a",["2"] = "b"});
        }

        [Fact]
        public void CanSendIntKeyStringValueDictionary()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendStringKeyStringValueDictionary).GetBodyParameterModuleUrl(), new Dictionary<int, string> { [1] = "a", [2] = "b" });
        }

        [Fact]
        public void CanSendStringKeyComplexValueDictionary()
        {
            AssertExpectedValueTypeSent(nameof(BodyParameterModule.SendStringKeyComplexValueDictionary).GetBodyParameterModuleUrl(), 
                new Dictionary<string, User> { ["1"] = new User { Id = 1 }, ["2"] = new User { Id = 2 } },
                (response, expected) =>
                {
                    var result = response.Body.Deserialize<Dictionary<string, User>>();
                    Assert.Equal(expected.Count, result.Count);
                });
        }

        [Fact]
        public void CanSendImage()
        {
            Bitmap expected = new Bitmap(100, 100, PixelFormat.Format24bppRgb);
            MemoryStream m = new MemoryStream();
            expected.Save(m, ImageFormat.Jpeg);

            var response = _browser.Get(nameof(BodyParameterModule.SendImage).GetBodyParameterModuleUrl(), ctx => { ctx.Body(m, "image/jpg"); });
            var result = Image.FromStream(response.Body.AsStream());
            Assert.Equal(expected.PixelFormat, result.PixelFormat);
            Assert.Equal(expected.Size, result.Size);
        }


        private void AssertExpectedValueTypeSent<T>(string url, T expected, Action<BrowserResponse, T> assertAction=null)
        {
            var encodedUrl = HttpUtility.UrlEncode(url);
            var bodyInput = Config.JsonSerializer.Serialize(expected);

            var response = _browser.Get(encodedUrl, ctx => ctx.Body(bodyInput));

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
    }

    public static class Helpers
    {
        public static ConfigurableBootstrapper CreateConfigurableBootstrapper(this Type type)
        {
            return new ConfigurableBootstrapper(config => { config.Modules(type); });
        }
        public static string GetBodyParameterModuleUrl(this string methodName)
        {
            return "/api/body/" + methodName;
        }

        public static string GetQueryParameterModuleUrl(this string methodName)
        {
            return "/api/query/" + methodName + "/";
        }

        public static T Deserialize<T>(this BrowserResponseBodyWrapper bodyWrapper)
        {           
            return bodyWrapper.Deserialize<T>(new JsonNetBodyDeserializer());
        }
    }
}
