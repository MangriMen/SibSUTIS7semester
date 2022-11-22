namespace kp.Models;

public class GrammarException : Exception
{
    public GrammarException()
    {
    }

    public GrammarException(string message)
        : base(message)
    {
    }

    public GrammarException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
