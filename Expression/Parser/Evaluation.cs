﻿using Expression.ExpressionNode;
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
                var nodeResult =  childNode.Accept(this);
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

    public class EvaluationExpressionVisitor : IExpressionNodeVisitor<string>
    {
        private readonly SimplifyClass _simplifyClass;
        public EvaluationExpressionVisitor()
        {
            _simplifyClass = new SimplifyClass();
        }

        public string Visit(ElementNode node)
        {
            return node.ToString()!;
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
                    : result.Accept(new ExpressionMultiplyVisitor(childNode));
            }

            var resultNode = result ?? new MultiplyNode();
            return resultNode.Accept(new PrintExpressionVisitor());
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
                }*/
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
}