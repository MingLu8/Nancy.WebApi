using System;
using System.Collections.Generic;
using System.Drawing;
using Nancy.WebApi.AttributeRouting;

namespace Nancy.WebApi.Tests
{
    [RoutePrefix("api/body")]
    public class BodyParameterModule : ApiController
    {
        [HttpGet(nameof(SendInt))]
        public dynamic SendInt(int input)
        {
            return input;
        }

        [HttpGet(nameof(SendString))]
        public dynamic SendString(string input)
        {
            return input;
        }

        [HttpGet(nameof(SendDouble))]
        public dynamic SendDouble(double input)
        {
            return input;
        }

        [HttpGet(nameof(SendDate))]
        public dynamic SendDate(DateTime input)
        {
            return input;
        }

        [HttpGet(nameof(SendEnumByValue))]
        public dynamic SendEnumByValue(Gender input)
        {
            return input;
        }

        [HttpGet(nameof(SendEnumByName))]
        public dynamic SendEnumByName(Gender input)
        {
            return input;
        }

        [HttpGet(nameof(SendIntArray))]
        public dynamic SendIntArray(int[] input)
        {
            return input;
        }

        [HttpGet(nameof(SendIntList))]
        public dynamic SendIntList(List<int> input)
        {
            return input;
        }

        [HttpGet(nameof(SendByeArray))]
        public byte[] SendByeArray(byte[] input)
        {
            return input;
        }


        [HttpGet(nameof(SendStringKeyStringValueDictionary))]
        public dynamic SendStringKeyStringValueDictionary(Dictionary<string, string> input)
        {
            return input;
        }

        [HttpGet(nameof(SendIntKeyStringValueDictionary))]
        public dynamic SendIntKeyStringValueDictionary(Dictionary<int, string> input)
        {
            return input;
        }

        [HttpGet(nameof(SendStringKeyComplexValueDictionary))]
        public dynamic SendStringKeyComplexValueDictionary(Dictionary<int, User> input)
        {
            return input;
        }

        [HttpGet(nameof(SendImage))]
        public dynamic SendImage(Image input)
        {
            return input;
        }
    }

    [RoutePrefix("api/query")]
    public class QueryParameterModule : ApiController
    {
        [HttpGet(nameof(SendInt) + "/{input}")]
        public dynamic SendInt(int input)
        {
            return input;
        }

        [HttpGet(nameof(SendString) + "/{input}")]
        public dynamic SendString(string input)
        {
            return input;
        }

        [HttpGet(nameof(SendDouble) + "/{input}")]
        public dynamic SendDouble(double input)
        {
            return input;
        }

        [HttpGet(nameof(SendDate) + "/{input}")]
        public dynamic SendDate(DateTime input)
        {
            return input;
        }

        [HttpGet(nameof(SendEnumByValue) + "/{input}")]
        public dynamic SendEnumByValue(Gender input)
        {
            return input;
        }

        [HttpGet(nameof(SendEnumByName) + "/{input}")]
        public dynamic SendEnumByName(Gender input)
        {
            return input;
        }

        [HttpGet(nameof(SendIntArray) + "/{input}")]
        public dynamic SendIntArray(int[] input)
        {
            return input;
        }

        [HttpGet(nameof(SendIntList) + "/{input}")]
        public dynamic SendIntList(List<int> input)
        {
            return input;
        }

        [HttpGet(nameof(SendByeArray) + "/{input}")]
        public byte[] SendByeArray(byte[] input)
        {
            return input;
        }


        [HttpGet(nameof(SendStringKeyStringValueDictionary) + "/{input}")]
        public dynamic SendStringKeyStringValueDictionary(Dictionary<string, string> input)
        {
            return input;
        }

        [HttpGet(nameof(SendIntKeyStringValueDictionary) + "/{input}")]
        public dynamic SendIntKeyStringValueDictionary(Dictionary<int, string> input)
        {
            return input;
        }

        [HttpGet(nameof(SendStringKeyComplexValueDictionary) + "/{input}")]
        public dynamic SendStringKeyComplexValueDictionary(Dictionary<int, User> input)
        {
            return input;
        }

        [HttpGet(nameof(SendMixedQueryParameters) + "/{p1}/{p2}/{p3}?gender={gender}&date={date}")]
        public string SendMixedQueryParameters(int p1, DateTime p2, string p3, Gender gender, DateTime date)
        {
            return $"{p1},{p2},{p3},{gender},{date.ToString("yyyy-MM-dd")}";
        }
    }
}
