using System;

namespace Nancy.WebApi
{
    public class ResponseMessage<T>
    {
        public Func<Response> ConstuctResponse { get; private set; }

        public ResponseMessage(Func<Response> constuctResponse)
        {
            ConstuctResponse = constuctResponse;
        }
    }
}
