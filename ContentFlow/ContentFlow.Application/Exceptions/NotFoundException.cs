using System.Runtime.Serialization;

namespace ContentFlow.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base() {}
    
    public NotFoundException(string message) : base(message) {}
    
    public NotFoundException(string message, Exception inner) : base(message, inner) {}
    
    protected NotFoundException(SerializationInfo info, StreamingContext context) :  base(info, context) {}
}