﻿using Expression.ExpressionNode;
using System.Text;

namespace Expression.Parser;

public abstract class Evaluation
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
            /*var simplifyResult = 0;
            var result = new StringBuilder();
            foreach (var childNode in node.Expressions)
            {
                var simplifyChildResult = childNode.Accept(_simplifyVisitor);
                if (simplifyChildResult.Success)
                {
                    simplifyResult += simplifyChildResult.Value;
                }
                else
                {
                    result.Append(childNode.Accept(this));
                    result.Append("+");
                }
            }

            result.Append(simplifyResult.ToString());
            //result.Length--; // Remove the last '+'
            return result.ToString();*/

            var resultSimplify = node.Accept(new ExpressionPlusSimplifyVisitor());
            var result = new StringBuilder();
            if (resultSimplify.Variable != null)
            {
                result.Append(resultSimplify.Variable!);
                result.Append("+");
            }

            result.Append(resultSimplify.SimplifyPlus.ToString());
            return result.ToString();
        }

        public string Visit(MultiplyNode node)
        {
            /*var result = new StringBuilder();
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
            return result.ToString();*/

            var resultSimplify = node.Accept(new ExpressionMultiplySimplifyVisitor());
            var result = new StringBuilder();
            if (resultSimplify.Variable != null)
            {
                result.Append(resultSimplify.Variable!);
                result.Append("*");
            }

            result.Append(resultSimplify.SimplifyMultiply.ToString());
            return result.ToString();
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
        public ExpressionPlusSimplifyVisitor(int? simplifyPlus = null, string? variable= null)
        {
            SimplifyPlus = simplifyPlus ?? 0;
            Variable = variable;
        }

        public string? Variable { get; private set; }
        public int? SimplifyPlus { get; private set; }

        public ExpressionPlusSimplifyVisitor Visit(ElementNode node)
        {
            var value = node.Value;
            if (value == VARIABLE)
            {
                Variable = VARIABLE;
                return this;
            }

            if (int.TryParse(value, out var intValue))
            {
                SimplifyPlus += intValue;
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
        public ExpressionMultiplySimplifyVisitor(int? simplifyMultiply = null, string? variable = null)
        {
            SimplifyMultiply = simplifyMultiply ?? 1;
            Variable = variable;
        }

        public string? Variable { get; private set; }
        public int? SimplifyMultiply { get; private set; }

        public ExpressionMultiplySimplifyVisitor Visit(ElementNode node)
        {
            var value = node.Value;
            if (value == VARIABLE)
            {
                Variable = VARIABLE;
                return this;
            }

            if (int.TryParse(value, out var intValue))
            {
                SimplifyMultiply *= intValue;
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