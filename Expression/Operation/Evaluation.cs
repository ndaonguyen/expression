using System.Runtime.CompilerServices;
using Expression.Model;
using ElementNode = Expression.Model.ElementNode;

[assembly: InternalsVisibleTo("Expression.Tests")]
namespace Expression.Operation;

public static class Evaluation
{
    public const string VARIABLE = "x";
    

    public static string EvaluateExpression(ExpressionNode expressionNode)
    {
        return expressionNode.Accept(new EvaluationExpressionVisitor());
    }

    private class EvaluationExpressionVisitor : IExpressionNodeVisitor<string>
    {
        private readonly GroupingClass _groupingClass;
        public EvaluationExpressionVisitor()
        {
            _groupingClass = new GroupingClass();
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
                    _groupingClass.AddNode(childElementNode);
                }
                else 
                {
                    childNode.Accept(this);
                }
            }

            return _groupingClass.ToString();
        }

        public string Visit(MultiplyNode node)
        {
            ExpressionNode? result = null;
            foreach (var childNode in node.Expressions)
            {
                result = result == null 
                    ? childNode // first node
                    : result.Accept(new ExpressionMultiplyVisitor(childNode));
            }

            var resultNode = result ?? new MultiplyNode();
            resultNode.Accept(new GroupingVisitor(_groupingClass));
            return _groupingClass.ToString();
        }

        private class GroupingVisitor : IExpressionNodeVisitor<bool>
        {
            private readonly GroupingClass _groupingClass;

            public GroupingVisitor(GroupingClass groupingClass)
            {
                _groupingClass = groupingClass ?? throw new ArgumentNullException(nameof(groupingClass));
            }

            public bool Visit(ElementNode node)
            {
                _groupingClass.AddNode(node);
                return true;
            }

            public bool Visit(AddNode node)
            {
                foreach (var childNode in node.Expressions)
                {
                    childNode.Accept(this);
                }
                return true;
            }

            public bool Visit(MultiplyNode node)
            {
                throw new NotSupportedException("At this stage shouldn't have MultiplyNode anymore");
            }
        }
    }

    /// <summary>
    /// This class is to do multiply between an expression node and another expression node
    /// </summary>
    private class ExpressionMultiplyVisitor : IExpressionNodeVisitor<ExpressionNode>
    {
        private readonly ExpressionNode _nodeToOperate;

        public ExpressionMultiplyVisitor(ExpressionNode nodeToOperate)
        {
            _nodeToOperate = nodeToOperate ?? throw new ArgumentNullException(nameof(nodeToOperate));
        }

        public ExpressionNode Visit(ElementNode node)
        {
            return _nodeToOperate.Accept(new ElementNodeMultiplyVisitor(node));
        }

        public ExpressionNode Visit(AddNode node)
        {
            var newAddNode = new AddNode();

            foreach (var childExpression in node.Expressions)
            {
                var result = childExpression.Accept(new ExpressionMultiplyVisitor(_nodeToOperate));
                newAddNode.AddElement(result);
            }

            return newAddNode;
        }

        public ExpressionNode Visit(MultiplyNode node)
        {
            var nodeToMultiple = _nodeToOperate; // assign this to continue multiply next node
            foreach (var childExpression in node.Expressions)
            {
                nodeToMultiple = childExpression.Accept(new MultiplyChildVisitor(nodeToMultiple));
            }

            return nodeToMultiple;
        }
    }

    /////Note that in the end, the result is the group of element node only => in Evaluation, we can group and simplify
    /// <summary>
    /// This class is to do multiply between an element node with an Expression
    /// </summary>
    private class ElementNodeMultiplyVisitor : IExpressionNodeVisitor<ExpressionNode>
    {
        private readonly ElementNode _nodeToOperate;

        public ElementNodeMultiplyVisitor(ElementNode nodeToOperate)
        {
            _nodeToOperate = nodeToOperate ?? throw new ArgumentNullException(nameof(nodeToOperate));
        }

        public ExpressionNode Visit(ElementNode node)
        {
            return ElementNodeAnalysis.CombineForMultiply(_nodeToOperate, node);
        }

        public ExpressionNode Visit(AddNode node)
        {
            var newAddNode = new AddNode();

            foreach (var childExpression in node.Expressions)
            {
                var result = childExpression.Accept(new ElementNodeMultiplyVisitor(_nodeToOperate));
                newAddNode.AddElement(result);
            }

            return newAddNode;
        }

        public ExpressionNode Visit(MultiplyNode node)
        {
            ExpressionNode nodeToMultiple = _nodeToOperate; // assign this to continue multiply next node
            foreach (var childExpression in node.Expressions)
            {
                nodeToMultiple = childExpression.Accept(new MultiplyChildVisitor(nodeToMultiple));
            }

            return nodeToMultiple;
        }
    }

    private class MultiplyChildVisitor : IExpressionNodeVisitor<ExpressionNode>
    {
        private readonly ExpressionNode _nodeToMultiple;

        public MultiplyChildVisitor(ExpressionNode nodeToMultiple)
        {
            _nodeToMultiple = nodeToMultiple ?? throw new ArgumentNullException(nameof(nodeToMultiple));
        }

        public ExpressionNode Visit(ElementNode node)
        {
            return _nodeToMultiple.Accept(new ElementNodeMultiplyVisitor(node));
        }

        public ExpressionNode Visit(AddNode node)
        {
            return node.Accept(new ExpressionMultiplyVisitor(_nodeToMultiple));
        }

        public ExpressionNode Visit(MultiplyNode node)
        {
            return node.Accept(new ExpressionMultiplyVisitor(_nodeToMultiple));
        }
    }
}