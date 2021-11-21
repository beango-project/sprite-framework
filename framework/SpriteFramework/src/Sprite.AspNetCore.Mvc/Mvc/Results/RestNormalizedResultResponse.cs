using System;

namespace Sprite.AspNetCore.Mvc.Results
{
    [Serializable]
    public class RestNormalizedResultResponse<TResult> : NormalizedResultResponse where TResult : class
    {
        public RestNormalizedResultResponse()
        {
            Success = true;
        }

        public RestNormalizedResultResponse(bool success)
        {
            Success = success;
        }

        public RestNormalizedResultResponse(TResult result)
            : this()
        {
            Result = result;
        }

        public TResult Result { get; set; }
    }

    [Serializable]
    public class RestNormalizedResultResponse : RestNormalizedResultResponse<object>
    {
        public RestNormalizedResultResponse()
        {
        }

        public RestNormalizedResultResponse(bool success) : base(success)
        {
        }

        public RestNormalizedResultResponse(object result)
            : base(result)
        {
        }
    }
}