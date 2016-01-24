using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nancy.WebApi.AttributeRouting;
using Xunit;

namespace Nancy.WebApi.Tests
{
    public class TestResult
    {
        public string P1 { get; set; }
        public int P2 { get; set; }
    }

    public class TestBodyInput
    {
        public string I1 { get; set; }
        public int I2 { get; set; }
    }

    public class TestModule
    {
        [HttpGet("api/p1/{p1}/p2/{p2}/p3/{p3}/gender/{gender}?date={date}&list={list}&numbers={numbers}")]
        public TestResult TestMethod(int p1, DateTime p2, string p3, Gender gender, DateTime date, List<string> list, double[] numbers, TestBodyInput testBodyInput)
        {
            return new TestResult();
        }
    }

    public class RouteInfoTests
    {
        private MethodInfo _testMethod;

        public RouteInfoTests()
        {
            _testMethod = typeof(TestModule).GetMethod("TestMethod");
        }
        [Fact]
        public void UrlParameters_Returns_ParametersDefinedOnTheAttribute()
        {
            var routeInfo = new RouteInfo(_testMethod);
            var urlParameters = routeInfo.QueryParameters;
            var parameters = string.Join(",", urlParameters.Select(a => a.Name));
            Assert.Equal("p1,p2,p3,gender,date,list,numbers", parameters);
        }
        [Fact]
        public void BodyParameter_Returns_TheFirstNonQueryParameter()
        {
            var routeInfo = new RouteInfo(_testMethod);           
            Assert.Equal("testBodyInput", routeInfo.BodyParameter.Name);
        }

        [Fact]
        public void ReturnParameter_Returns_ReturnParameter()
        {
            var routeInfo = new RouteInfo(_testMethod);
            var returnParameter = routeInfo.ReturnParameter;

            Assert.NotNull(returnParameter);
            Assert.Equal(typeof(TestResult), returnParameter.ParameterType);
        }

    }
}
