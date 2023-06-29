using System.Text;
using Expression.ExpressionNode;

namespace Expression.Parser;

public static class Parser
{
    


    private static ExpressionNode.ExpressionNode ParseExpressionTerm(string expression, ref int index)
    {
        var c = expression[index];
        if (char.IsDigit(c))
        {
            index++;
            return new ElementNode(c.ToString());
        }
        
        if (c == 'x')
        {
            index++;
            return new ElementNode(c.ToString());
        }

        if (c == '(')
        {
            index++;
            return ParseExpr(expression, ref index);
        }

        throw new InvalidOperationException("Invalid character: " + c);
    }

    public static ExpressionNode.ExpressionNode ParseExpr(string expression, ref int index)
    {
        var node = ParseExpressionTerm(expression, ref index);

        while (index < expression.Length)
        {
            var c = expression[index];
            if (c == '+')
            {
                index++;
                var addNode = new AddNode();
                addNode.AddElement(node);

                var nextNode = ParseExpressionTerm(expression, ref index);
                addNode.AddElement(nextNode);

                node = addNode;
            }
            else if (c == '*')
            {
                index++;
                var multiplyNode = new MultiplyNode();
                multiplyNode.AddElement(node);

                var nextNode = ParseExpressionTerm(expression, ref index);
                multiplyNode.AddElement(nextNode);

                node = multiplyNode;
            }
            else if (c == ')')
            {
                index++;
                break;
            }
            else
            {
                throw new InvalidOperationException("Invalid character:" + c);
            }
        }

        return node;
    }
}