namespace WhackTranslationTool.Exceptions;

public class UnhandledTomlError : Exception
{
    public UnhandledTomlError()
    {
    }

    public UnhandledTomlError(string message)
        : base(message)
    {
    }

    public UnhandledTomlError(string message, Exception inner)
        : base(message, inner)
    {
    }
}
