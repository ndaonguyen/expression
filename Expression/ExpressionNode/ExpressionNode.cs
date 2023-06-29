namespace Expression.ExpressionNode;

public abstract class ExpressionNode
{
    public abstract T Accept<T>(IExpressionNodeVisitor<T> visitor);
}

public class ElementNode : ExpressionNode
{
    public ElementNode(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public override T Accept<T>(IExpressionNodeVisitor<T> visitor) => visitor.Visit(this);
}


public abstract class OperationExpressionNode : ExpressionNode
{
    public abstract IEnumerable<ExpressionNode> Expressions { get; }
    public abstract void AddElement(ExpressionNode node);
}

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

public class MultiplyNode : OperationExpressionNode
{
    private readonly List<ExpressionNode> _expressionNodes;
    public MultiplyNode()
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

public interface IExpressionNodeVisitor<out T>
{
    T Visit(ElementNode node);
    T Visit(AddNode node);
    T Visit(MultiplyNode node);
}