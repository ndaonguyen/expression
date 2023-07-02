namespace Expression.Model;

public abstract class ExpressionNode
{
    public abstract T Accept<T>(IExpressionNodeVisitor<T> visitor);
}