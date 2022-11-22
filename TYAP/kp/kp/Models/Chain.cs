namespace kp.Models;

public struct Chain
{
    private static readonly string DELIMETER = " > ";

    public List<string> output = new();
    public string raw = "";

    public Chain(string raw, string[] output)
    {
        this.raw = raw;
        this.output.AddRange(output);
    }

    public string GetFormattedOutput()
    {
        return string.Join(DELIMETER, output);
    }

    public override string ToString()
    {
        return $"O: {string.Join(DELIMETER, output)}; R: {raw}";
    }
}
