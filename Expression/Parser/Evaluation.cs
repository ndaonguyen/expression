using Expression.ExpressionNode;
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
        return expressionNode.Accept(new EvaluationExpressionVisitor());
    }

    private class PrintExpressionVisitor : IExpressionNodeVisitor<string>
    {
        private readonly SimplifyClass _simplifyClass;

        public PrintExpressionVisitor(SimplifyClass simplifyClass)
        {
            _simplifyClass = simplifyClass ?? throw new ArgumentNullException(nameof(simplifyClass));
        }

        public string Visit(ElementNode node)
        {
            _simplifyClass.AddNode(node);
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
            throw new NotSupportedException("At this stage shouldn't have MultiplyNode anymore");
        }
    }

    private class EvaluationExpressionVisitor : IExpressionNodeVisitor<string>
    {
        private readonly SimplifyClass _simplifyClass;
        public EvaluationExpressionVisitor()
        {
            _simplifyClass = new SimplifyClass();
        }

        public string Visit(ElementNode node)
        {
            return node.Value;
        }

        public string Visit(AddNode node)
        {
            foreach (var childNode in node.Expressions)
            {
                if (childNode is ElementNode childElementNode)
                {
                    _simplifyClass.AddNode(childElementNode);
                }
                else if (childNode is AddNode childAddNode)
                {
                    childAddNode.Accept(this);
                }
                else if (childNode is MultiplyNode childMultiplyNode)
                {
                    childMultiplyNode.Accept(this);
                }
            }

            return _simplifyClass.ToString();
        }

        public string Visit(MultiplyNode node)
        {
            ExpressionNode.ExpressionNode? result = null;
            foreach (var childNode in node.Expressions)
            {
                result = result == null 
                    ? childNode // first node
                    : result.Accept(new ExpressionMultiplyVisitor(childNode, _simplifyClass));
            }

            var resultNode = result ?? new MultiplyNode();
            resultNode.Accept(new PrintExpressionVisitor(_simplifyClass));
            return _simplifyClass.ToString();
        }
    }

    private class ExpressionMultiplyVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly ExpressionNode.ExpressionNode _nodeToOperate;
        private readonly SimplifyClass _simplifyClass;

        public ExpressionMultiplyVisitor(
            ExpressionNode.ExpressionNode nodeToOperate, 
            SimplifyClass simplifyClass)
        {
            _nodeToOperate = nodeToOperate ?? throw new ArgumentNullException(nameof(nodeToOperate));
            _simplifyClass = simplifyClass ?? throw new ArgumentNullException(nameof(simplifyClass));
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            return _nodeToOperate.Accept(new ElementNodeMultiplyVisitor(node, _simplifyClass));
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            var newAddNode = new AddNode();

            foreach (var childExpression in node.Expressions)
            {
                var result = childExpression.Accept(new ExpressionMultiplyVisitor(_nodeToOperate, _simplifyClass));
                newAddNode.AddElement(result);
            }

            return newAddNode;
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            var nodeToMultiple = _nodeToOperate; // assign this to continue multiply next node
            foreach (var childExpression in node.Expressions)
            {
                nodeToMultiple = childExpression.Accept(new MultiplyChildVisitor(nodeToMultiple, _simplifyClass));
            }

            return nodeToMultiple;
        }
    }

    ///Note that in the end, the result is the group of element node only => in Evaluation, we can group and simplify
    private class ElementNodeMultiplyVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly ElementNode _nodeToOperate;
        private readonly SimplifyClass _simplifyClass;

        public ElementNodeMultiplyVisitor(ElementNode nodeToOperate, SimplifyClass simplifyClass)
        {
            _nodeToOperate = nodeToOperate ?? throw new ArgumentNullException(nameof(nodeToOperate));
            _simplifyClass = simplifyClass ?? throw new ArgumentNullException(nameof(simplifyClass));
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            var result = ElementNodeAnalysis.CombineForMultiply(_nodeToOperate, node);
            //_simplifyClass.AddNode(result);
            return result;
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            var newAddNode = new AddNode();

            foreach (var childExpression in node.Expressions)
            {
                var result = childExpression.Accept(new ElementNodeMultiplyVisitor(_nodeToOperate, _simplifyClass));
                newAddNode.AddElement(result);
            }

            return newAddNode;
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            ExpressionNode.ExpressionNode nodeToMultiple = _nodeToOperate; // assign this to continue multiply next node
            foreach (var childExpression in node.Expressions)
            {
                nodeToMultiple = childExpression.Accept(new MultiplyChildVisitor(nodeToMultiple, _simplifyClass));
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
                }*/
            }

            return nodeToMultiple;
        }
    }

    private class MultiplyChildVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly ExpressionNode.ExpressionNode _nodeToMultiple;
        private readonly SimplifyClass _simplifyClass;

        public MultiplyChildVisitor(ExpressionNode.ExpressionNode nodeToMultiple, SimplifyClass simplifyClass)
        {
            _nodeToMultiple = nodeToMultiple ?? throw new ArgumentNullException(nameof(nodeToMultiple));
            _simplifyClass = simplifyClass ?? throw new ArgumentNullException(nameof(simplifyClass));
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            return _nodeToMultiple.Accept(new ElementNodeMultiplyVisitor(node, _simplifyClass));
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            return node.Accept(new ExpressionMultiplyVisitor(_nodeToMultiple, _simplifyClass));
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            return node.Accept(new ExpressionMultiplyVisitor(_nodeToMultiple, _simplifyClass));
        }
    }
}