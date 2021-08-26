using System.Text.Json;

namespace Common.Errors
{
    public class Error
    {
        public Error(string message, string source)
        {
            Message = message;
            Source = source;
        }
        public string Message { get; }
        public string Source { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
