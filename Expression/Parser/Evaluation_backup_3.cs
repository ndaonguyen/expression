/*using Expression.ExpressionNode;
using System.Runtime.CompilerServices;
using System.Text;
using ElementNode = Expression.ExpressionNode.ElementNode;

[assembly: InternalsVisibleTo("Expression.Tests")]
namespace Expression.Parser;

public static class Evaluation
{
    public const string VARIABLE = "x";


    public static string EvaluateExpression(ExpressionNode.ExpressionNode expressionNode)
    {
        return expressionNode.Accept(new PrintExpressionVisitor());
    }

    private class PrintExpressionVisitor : IExpressionNodeVisitor<string>
    {
        public string Visit(ElementNode node)
        {
            return node.Value;
        }

        public string Visit(AddNode node)
        {
            var result = new StringBuilder();
            foreach (var childNode in node.Expressions)
            {
                result.Append(childNode.Accept(this));
                result.Append("+");
            }

            result.Length--; // Remove the last '+'
            return result.ToString();
        }

        public string Visit(MultiplyNode node)
        {
            var result = new StringBuilder();
            var checkExpressionWithBracket = new ExpressionWithBracketVisitor();
            foreach (var childNode in node.Expressions)
            {
                var nodeResult = childNode.Accept(this);
                if (childNode.Accept(checkExpressionWithBracket))
                {
                    result.Append('(');
                    result.Append(nodeResult);
                    result.Append(')');
                }
                else
                {
                    result.Append(nodeResult);
                }
                result.Append('*');
            }
            result.Length--; // Remove the last '*'
            return result.ToString();
        }
    }

    public class EvaluationExpressionVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly SimplifyClass _simplifyClass;
        public EvaluationExpressionVisitor()
        {
            _simplifyClass = new SimplifyClass();
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            return node;
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            var resultNode = new AddNode();
            foreach (var childNode in node.Expressions)
            {
                if (childNode is ElementNode childElementNode)
                {
                    _simplifyClass.AddNode(childElementNode);
                    resultNode.AddElement(childElementNode);
                }
                else if (childNode is AddNode childAddNode)
                {
                    resultNode.AddElement(childAddNode.Accept(this));
                }
                else if (childNode is MultiplyNode childMultiplyNode)
                {
                    resultNode.AddElement(childMultiplyNode.Accept(this));
                }
            }

            _simplifyClass.ToString();
            return resultNode;
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            ExpressionNode.ExpressionNode? result = null;
            foreach (var childNode in node.Expressions)
            {
                result = result == null
                    ? childNode // first node
                    : result.Accept(new ExpressionMultiplyVisitor(childNode));
            }

            return result ?? new MultiplyNode();
        }
    }

    private class ExpressionMultiplyVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly ExpressionNode.ExpressionNode _nodeToOperate;

        public ExpressionMultiplyVisitor(ExpressionNode.ExpressionNode nodeToOperate)
        {
            _nodeToOperate = nodeToOperate ?? throw new ArgumentNullException(nameof(nodeToOperate));
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            return _nodeToOperate.Accept(new ElementNodeMultiplyVisitor(node));
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            var newAddNode = new AddNode();

            foreach (var childExpression in node.Expressions)
            {
                var result = childExpression.Accept(new ExpressionMultiplyVisitor(_nodeToOperate));
                newAddNode.AddElement(result);
            }

            return newAddNode;
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            var nodeToMultiple = _nodeToOperate; // assign this to continue multiply next node
            foreach (var childExpression in node.Expressions)
            {
                nodeToMultiple = childExpression.Accept(new MultiplyChildVisitor(nodeToMultiple));
            }

            return nodeToMultiple;
        }
    }

    ///Note that in the end, the result is the group of element node only => in Evaluation, we can group and simplify
    private class ElementNodeMultiplyVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly ElementNode _nodeToOperate;

        public ElementNodeMultiplyVisitor(ElementNode nodeToOperate)
        {
            _nodeToOperate = nodeToOperate ?? throw new ArgumentNullException(nameof(nodeToOperate));
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            var result = ElementNodeAnalysis.CombineForMultiply(_nodeToOperate, node);
            return result;
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            var newAddNode = new AddNode();

            foreach (var childExpression in node.Expressions)
            {
                var result = childExpression.Accept(new ElementNodeMultiplyVisitor(_nodeToOperate));
                newAddNode.AddElement(result);
            }

            return newAddNode;
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            ExpressionNode.ExpressionNode nodeToMultiple = _nodeToOperate; // assign this to continue multiply next node
            foreach (var childExpression in node.Expressions)
            {
                nodeToMultiple = childExpression.Accept(new MultiplyChildVisitor(nodeToMultiple));
                /*if (childExpression is ElementNode childElementNode) // ndnguyen this break extension ability
                {
                    nodeToMultiple = nodeToMultiple.Accept(new ElementNodeMultiplyVisitor(childElementNode));
                }
                else if (childExpression is AddNode childAddNode)
                {
                    nodeToMultiple = childAddNode.Accept(new ExpressionMultiplyVisitor(nodeToMultiple));
                }
                else if (childExpression is MultiplyNode childNode)
                {
                    nodeToMultiple = childNode.Accept(new ExpressionMultiplyVisitor(nodeToMultiple));
                }#1#
            }

            return nodeToMultiple;
        }
    }

    private class MultiplyChildVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly ExpressionNode.ExpressionNode _nodeToMultiple;

        public MultiplyChildVisitor(ExpressionNode.ExpressionNode nodeToMultiple)
        {
            _nodeToMultiple = nodeToMultiple;
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            return _nodeToMultiple.Accept(new ElementNodeMultiplyVisitor(node));
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            return node.Accept(new ExpressionMultiplyVisitor(_nodeToMultiple));
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            return node.Accept(new ExpressionMultiplyVisitor(_nodeToMultiple));
        }
    }


    private class ExpressionWithBracketVisitor : IExpressionNodeVisitor<bool>
    {
        public bool Visit(ElementNode node)
        {
            return false;
        }

        public bool Visit(AddNode node)
        {
            return true;
        }

        public bool Visit(MultiplyNode node)
        {
            return true;
        }
    }

    private class ExpressionPlusSimplifyVisitor : IExpressionNodeVisitor<ExpressionPlusSimplifyVisitor>
    {
        public ExpressionPlusSimplifyVisitor(int? simplifyPlus = null, List<string>? variable = null)
        {
            SimplifyPlus = simplifyPlus ?? 0;
            Variable = variable ?? new List<string>();
            IsSimplify = false;
        }

        public List<string> Variable { get; private set; }
        public int? SimplifyPlus { get; private set; }
        public bool IsSimplify { get; private set; }


        public ExpressionPlusSimplifyVisitor Visit(ElementNode node)
        {
            var value = node.Value;
            if (value == VARIABLE)
            {
                Variable.Add(VARIABLE);
                return this;
            }

            if (int.TryParse(value, out var intValue))
            {
                SimplifyPlus += intValue;
                IsSimplify = true;
                return this;
            }

            throw new InvalidOperationException("Value is not correct format");
        }

        public ExpressionPlusSimplifyVisitor Visit(AddNode node)
        {
            foreach (var childNode in node.Expressions)
            {
                childNode.Accept(this);
            }

            return this;
        }

        public ExpressionPlusSimplifyVisitor Visit(MultiplyNode node)
        {
            return this;
        }
    }

    private class ExpressionMultiplySimplifyVisitor : IExpressionNodeVisitor<ExpressionMultiplySimplifyVisitor>
    {
        public ExpressionMultiplySimplifyVisitor(int? simplifyMultiply = null, List<string>? variable = null)
        {
            SimplifyMultiply = simplifyMultiply ?? 1;
            Variable = variable ?? new List<string>();
            IsSimplify = false;
        }

        public List<string> Variable { get; private set; }
        public int? SimplifyMultiply { get; private set; }
        public bool IsSimplify { get; private set; }

        public ExpressionMultiplySimplifyVisitor Visit(ElementNode node)
        {
            var value = node.Value;
            if (value == VARIABLE)
            {
                Variable.Add(VARIABLE);
                return this;
            }

            if (int.TryParse(value, out var intValue))
            {
                SimplifyMultiply *= intValue;
                IsSimplify = true;
                return this;
            }

            throw new InvalidOperationException("Value is not correct format");
        }

        public ExpressionMultiplySimplifyVisitor Visit(AddNode node)
        {
            return this;
        }

        public ExpressionMultiplySimplifyVisitor Visit(MultiplyNode node)
        {
            foreach (var childNode in node.Expressions)
            {
                childNode.Accept(this);
            }

            return this;
        }
    }

}*/