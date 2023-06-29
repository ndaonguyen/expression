using Expression.ExpressionNode;

namespace Expression.Parser;

public static class Parser
{
    public static ExpressionNode.ExpressionNode ParseExpression(string expression)
    {
        var index = 0;
        var stack = new Stack<char>();

        var expressionNode = ParseExprInternal(expression, stack, ref index);
        if (stack.Count != 0)
        {
            throw new InvalidOperationException($"Invalid format of bracket : '{expression}'");
        }

        return expressionNode;
    }


    private static ExpressionNode.ExpressionNode ParseExpressionTerm(string expression, Stack<char> stack, ref int index)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (stack == null) throw new ArgumentNullException(nameof(stack));

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
            stack.Push(c);
            return ParseExprInternal(expression, stack, ref index);
        }

        throw new InvalidOperationException("Invalid character: " + c);
    }

    private static ExpressionNode.ExpressionNode ParseExprInternal(string expression, Stack<char> stack, ref int index)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (stack == null) throw new ArgumentNullException(nameof(stack));

        var node = ParseExpressionTerm(expression, stack, ref index);

        while (index < expression.Length)
        {
            var c = expression[index];
            if (c == '+')
            {
                index++;
                var addNode = new AddNode();
                addNode.AddElement(node);

                var nextNode = ParseExpressionTerm(expression, stack, ref index);
                addNode.AddElement(nextNode);

                node = addNode;
            }
            else if (c == '*')
            {
                index++;
                var multiplyNode = new MultiplyNode();
                multiplyNode.AddElement(node);

                var nextNode = ParseExpressionTerm(expression, stack, ref index);
                multiplyNode.AddElement(nextNode);

                node = multiplyNode;
            }
            else if (c == ')')
            {
                if (stack.Count == 0 || stack.Pop() != '(')
                {
                    throw new InvalidOperationException($"Invalid format of bracket : '{expression}'");
                }

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