namespace CourseWork.Models.RegularExpression;
public class RegularExpressionToken : IRegularExpressionChild
{
    public char value = '\0';

    public RegularExpressionToken(char value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}
