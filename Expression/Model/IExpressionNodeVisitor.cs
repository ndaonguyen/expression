namespace Expression.Model;

public interface IExpressionNodeVisitor<out T>
{
    T Visit(ElementNode node);
    T Visit(AddNode node);
    T Visit(MultiplyNode node);
}