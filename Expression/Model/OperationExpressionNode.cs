namespace Expression.Model;

public abstract class OperationExpressionNode : ExpressionNode
{
    public abstract IEnumerable<ExpressionNode> Expressions { get; }
    public abstract void AddElement(ExpressionNode node);
}