using System;

namespace Common.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message, string source)
        : base(message)
        {
            Source = source;
        }
    }
}
