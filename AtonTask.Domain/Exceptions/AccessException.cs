namespace AtonTask.Domain.Exceptions;

public class AccessException : Exception
{
    public AccessException()
    {
        
    }
    
    public AccessException(string message) : base(message)
    {
        
    }

    public AccessException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}