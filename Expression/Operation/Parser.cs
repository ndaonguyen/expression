using Expression.Common;
using Expression.Model;

namespace Expression.Operation;

public static class Parser
{
    public static TryResults<ExpressionNode> TryParseExpression(string expression)
    {
        var index = 0;
        var stack = new Stack<char>();

        var expressionNode = TryParseExprInternal(expression, stack, ref index);
        if (expressionNode.Failed)
        {
            return TryResults<ExpressionNode>.Fail(expressionNode.ErrorMessage);
        }

        if (stack.Count != 0)
        {
            return TryResults<ExpressionNode>.Fail($"Invalid format of bracket : '{expression}'");
        }

        return TryResults<ExpressionNode>.Succeed(expressionNode.Value!);
    }


    private static TryResults<ExpressionNode> TryParseExpressionTerm(string expression, Stack<char> stack, ref int index)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (stack == null) throw new ArgumentNullException(nameof(stack));

        var c = expression[index];
        if (char.IsDigit(c))
        {
            index++;
            return TryResults<ExpressionNode>.Succeed(new ElementNode(c.ToString()));
        }
        
        if (c == 'x')
        {
            index++;
            return TryResults<ExpressionNode>.Succeed(new ElementNode(c.ToString()));
        }

        if (c == '(')
        {
            index++;
            stack.Push(c);
            return TryParseExprInternal(expression, stack, ref index);
        }

        return TryResults<ExpressionNode>.Fail("Invalid character: " + c);
    }

    private static TryResults<ExpressionNode> TryParseExprInternal(string expression, Stack<char> stack, ref int index)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (stack == null) throw new ArgumentNullException(nameof(stack));

        var nodeResult = TryParseExpressionTerm(expression, stack, ref index);
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

                var nextNode = TryParseExpressionTerm(expression, stack, ref index);
                if (nextNode.Failed)
                {
                    return nextNode;
                }

                if (node is AddNode parentNode) // in case we have multiple operands (like x+1+2+x), not only 2 operands
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

                var nextNode = TryParseExpressionTerm(expression, stack, ref index);
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
                    return TryResults<ExpressionNode>.Fail($"Invalid format of bracket : '{expression}'");
                }

                index++;
                break;
            }
            else
            {
                return TryResults<ExpressionNode>.Fail("Invalid character:" + c);
            }
        }

        return TryResults<ExpressionNode>.Succeed(node);
    }
}