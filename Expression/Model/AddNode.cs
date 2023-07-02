namespace Expression.Model;

public class AddNode : OperationExpressionNode
{
    private readonly List<ExpressionNode> _expressionNodes;
    public AddNode()
    {
        _expressionNodes = new List<ExpressionNode>();
    }

    public override void AddElement(ExpressionNode node)
    {
        _expressionNodes.Add(node);
    }

    public override IEnumerable<ExpressionNode> Expressions => _expressionNodes;
    public override T Accept<T>(IExpressionNodeVisitor<T> visitor) => visitor.Visit(this);
}