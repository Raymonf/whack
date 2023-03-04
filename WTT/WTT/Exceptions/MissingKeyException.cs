namespace WTT.Exceptions;

public class MissingKeyException : Exception
{
    public MissingKeyException()
    {
    }

    public MissingKeyException(string message)
        : base(message)
    {
    }

    public MissingKeyException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
