using Expression.ExpressionNode;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Expression.Tests")]
namespace Expression.Parser;

public static class Evaluation
{
    public const string VARIABLE = "x";
    

    public static string EvaluateExpression(ExpressionNode.ExpressionNode expressionNode)
    {
        return expressionNode.Accept(new EvaluateExpressionVisitor());
    }

    private class EvaluateExpressionVisitor : IExpressionNodeVisitor<string>
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

    internal class ExpressionOperateMultiplyVisitor : IExpressionNodeVisitor<ExpressionNode.ExpressionNode>
    {
        private readonly ElementNode _nodeToOperate;

        public ExpressionOperateMultiplyVisitor(ElementNode nodeToOperate)
        {
            _nodeToOperate = nodeToOperate ?? throw new ArgumentNullException(nameof(nodeToOperate));
        }

        public ExpressionNode.ExpressionNode Visit(ElementNode node)
        {
            return ElementNodeAnalysis.CombineForMultiply(_nodeToOperate, node);
        }

        public ExpressionNode.ExpressionNode Visit(AddNode node)
        {
            var newAddNode = new AddNode();

            foreach (var childExpression in node.Expressions)
            {
                newAddNode.AddElement(childExpression.Accept(new ExpressionOperateMultiplyVisitor(_nodeToOperate)));
                /*if (childExpression is ElementNode childElementNode) // ndnguyen this break extension ability
                {
                    newAddNode.AddElement(ElementNodeAnalysis.CombineForMultiply(_nodeToOperate, childElementNode));
                }
                else if (childExpression is AddNode childAddNode)
                {
                    newAddNode.AddElement(childAddNode.Accept(new ExpressionOperateMultiplyVisitor(_nodeToOperate)));
                }
                else if (childExpression is MultiplyNode childNode)
                {
                    newAddNode.AddElement(childNode.Accept(new ExpressionOperateMultiplyVisitor(_nodeToOperate)));
                }*/
            }

            return newAddNode;
        }

        public ExpressionNode.ExpressionNode Visit(MultiplyNode node)
        {
            var newNode = new MultiplyNode();

            foreach (var childExpression in node.Expressions)
            {
                newNode.AddElement(childExpression.Accept(new ExpressionOperateMultiplyVisitor(_nodeToOperate)));
                /*if (childExpression is ElementNode childElementNode)
                {
                    newNode.AddElement(ElementNodeAnalysis.CombineForMultiply(_nodeToOperate, childElementNode));
                }
                else if (childExpression is AddNode childAddNode)
                {
                    newNode.AddElement(childAddNode.Accept(new ExpressionOperateMultiplyVisitor(_nodeToOperate)));
                }
                else if (childExpression is MultiplyNode childNode)
                {
                    newNode.AddElement(childNode.Accept(new ExpressionOperateMultiplyVisitor(_nodeToOperate)));
                }*/
            }

            return newNode;
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
        public ExpressionPlusSimplifyVisitor(int? simplifyPlus = null, List<string>? variable= null)
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
    
}