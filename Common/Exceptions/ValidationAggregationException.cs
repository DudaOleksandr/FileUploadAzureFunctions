using System;
using System.Collections.Generic;

namespace Common.Exceptions
{
    [Serializable]
    public class ValidationAggregationException : Exception
    {
        public List<ValidationException> Exceptions { get; }
        public ValidationAggregationException() { }
        public ValidationAggregationException(string message, List<ValidationException> exceptions)
        : base(message)
        {
            Exceptions = exceptions;
        }
    }
}
