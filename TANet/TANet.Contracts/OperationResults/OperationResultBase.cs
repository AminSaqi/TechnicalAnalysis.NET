using System;
using System.Collections.Generic;
using System.Text;

namespace TANet.Contracts.OperationResults
{
    public class OperationResultBase
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public static OperationResultBase CreateSuccessResult(string message)
        {
            return new OperationResultBase
            {
                Success = true,
                StatusCode = 200,
                Message = message
            };
        }

        public static OperationResultBase CreateExceptionResult(Exception ex)
        {
            return new OperationResultBase
            {
                Success = false,
                StatusCode = 500,
                Message = ex.Message
            };
        }

        public static OperationResultBase CreateErrorResult(string errorMessage, int statusCode = 500)
        {
            return new OperationResultBase
            {
                Success = false,
                StatusCode = statusCode,
                Message = errorMessage
            };
        }
    }
}
