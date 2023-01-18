namespace CourseWork.Models.RegularExpression;
public class RegularExpression : IRegularExpressionChild
{
    public enum Type
    {
        Multiply,
        Select,
    }
    public Type type = Type.Multiply;
    public bool isLoop = false;
    public string raw = string.Empty;
    public List<IRegularExpressionChild> tokens = new();

    public RegularExpression()
    {
    }

    public override string ToString()
    {
        var operation = type == Type.Multiply ? "." : "+";
        var stringTokens = string.Join(operation, tokens);
        var loop = isLoop ? "*" : "";
        return $"({stringTokens}){loop}";
    }
}
