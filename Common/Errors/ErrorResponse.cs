using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Common.Errors
{
    public class ErrorResponse
    {
        public ErrorResponse(string message, string source, string traceId) : this(new Error(message, source), traceId) { }
        public ErrorResponse(string traceId, List<Error> errors)
        {
            TraceId = traceId;
            Errors = errors;
        }
        public ErrorResponse(Error error, string traceId) : this(new[] { error })
        {
            TraceId = traceId;
        }

        public ErrorResponse(IEnumerable<Error> errors)
        {
            Errors = errors.ToList();
        }

        public ErrorResponse(string traceId)
        {
            TraceId = traceId;
            Errors = new List<Error>();
        }

        public string TraceId { get; }
        public List<Error> Errors { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
