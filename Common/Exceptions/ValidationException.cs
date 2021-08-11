using System;

namespace Common.Exceptions
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message, string source)
        : base(message)
        {
            Source = source;
        }
    }
}
