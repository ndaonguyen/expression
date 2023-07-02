namespace Expression.Model;

public class ElementNode : ExpressionNode
{
    public ElementNode(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public override T Accept<T>(IExpressionNodeVisitor<T> visitor) => visitor.Visit(this);
}