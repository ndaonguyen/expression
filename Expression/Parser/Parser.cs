using Expression.ExpressionNode;

namespace Expression.Parser;

public static class Parser
{
    public static TryResults<ExpressionNode.ExpressionNode> ParseExpression(string expression)
    {
        var index = 0;
        var stack = new Stack<char>();

        var expressionNode = ParseExprInternal(expression, stack, ref index);
        if (expressionNode.Failed)
        {
            return TryResults<ExpressionNode.ExpressionNode>.Fail(expressionNode.ErrorMessage);
        }

        if (stack.Count != 0)
        {
            return TryResults<ExpressionNode.ExpressionNode>.Fail($"Invalid format of bracket : '{expression}'");
        }

        return TryResults<ExpressionNode.ExpressionNode>.Succeed(expressionNode.Value!);
    }


    private static TryResults<ExpressionNode.ExpressionNode> ParseExpressionTerm(string expression, Stack<char> stack, ref int index)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (stack == null) throw new ArgumentNullException(nameof(stack));

        var c = expression[index];
        if (char.IsDigit(c))
        {
            index++;
            return TryResults<ExpressionNode.ExpressionNode>.Succeed(new ElementNode(c.ToString()));
        }
        
        if (c == 'x')
        {
            index++;
            return TryResults<ExpressionNode.ExpressionNode>.Succeed(new ElementNode(c.ToString()));
        }

        if (c == '(')
        {
            index++;
            stack.Push(c);
            return ParseExprInternal(expression, stack, ref index);
        }

        return TryResults<ExpressionNode.ExpressionNode>.Fail("Invalid character: " + c);
    }

    private static TryResults<ExpressionNode.ExpressionNode> ParseExprInternal(string expression, Stack<char> stack, ref int index)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (stack == null) throw new ArgumentNullException(nameof(stack));

        var nodeResult = ParseExpressionTerm(expression, stack, ref index);
        if (nodeResult.Failed)
        {
            return nodeResult;
        }

        var node = nodeResult.Value!;
        while (index < expression.Length)
        {
            var c = expression[index];
            if (c == '+')
            {
                index++;

                var nextNode = ParseExpressionTerm(expression, stack, ref index);
                if (nextNode.Failed)
                {
                    return nextNode;
                }

                if (node is AddNode parentNode) // this one break the extension? use visitor?
                {
                    parentNode.AddElement(nextNode.Value!);
                    node = parentNode;
                }
                else
                {
                    var addNode = new AddNode();

                    addNode.AddElement(node);
                    addNode.AddElement(nextNode.Value!);

                    node = addNode;
                }
            }
            else if (c == '*')
            {
                index++;

                var nextNode = ParseExpressionTerm(expression, stack, ref index);
                if (nextNode.Failed)
                {
                    return nextNode;
                }

                if (node is MultiplyNode parentNode)
                {
                    parentNode.AddElement(nextNode.Value!);
                    node = parentNode;
                }
                else
                {
                    var multiplyNode = new MultiplyNode();

                    multiplyNode.AddElement(node);
                    multiplyNode.AddElement(nextNode.Value!);

                    node = multiplyNode;
                }
            }
            else if (c == ')')
            {
                if (stack.Count == 0 || stack.Pop() != '(')
                {
                    return TryResults<ExpressionNode.ExpressionNode>.Fail($"Invalid format of bracket : '{expression}'");
                }

                index++;
                break;
            }
            else
            {
                return TryResults<ExpressionNode.ExpressionNode>.Fail("Invalid character:" + c);
            }
        }

        return TryResults<ExpressionNode.ExpressionNode>.Succeed(node);
    }
}