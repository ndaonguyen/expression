using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NCalc;
using System;

class Program
{
    public class ExpressionSimplifier
    {
        public string SimplifyExpression(string inputExpression)
        {
            string simplifiedExpression = SimplifyHelper(inputExpression);
            return simplifiedExpression;
        }

        private string SimplifyHelper(string expression)
        {
            // Base case: if the expression does not contain any operator, it is already simplified
            if (!expression.Contains("+") && !expression.Contains("*"))
                return expression;

            // Find the outermost parentheses
            int openBracketIndex = expression.LastIndexOf('(');
            string simplifiedSubExpression = expression;
            if (openBracketIndex != -1)
            {
                int closeBracketIndex = expression.IndexOf(')', openBracketIndex);

                // Extract the sub-expression inside the outermost parentheses
                var subExpression = expression.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
                // Simplify the sub-expression
                simplifiedSubExpression = SimplifyHelper(subExpression);
            }
            

            // Replace the sub-expression (including parentheses) with its simplified form
            string simplifiedExpression = expression.Replace("(" + expression + ")", simplifiedSubExpression);

            // Recursively simplify the expression until no parentheses are left
            return SimplifyHelper(simplifiedExpression);
        }
    }

    static void Main()
    {
       

        /*// Define the variable 'x' and assign a value to it
        int x = 2;

        try
        {
            // Evaluate the expression that references 'x'
            object result = CSharpScript.EvaluateAsync("(x + 1 + 1)").Result;
            Console.WriteLine(result);
        }
        catch (CompilationErrorException ex)
        {
            Console.WriteLine("Compilation Error: " + ex.Message);
        }*/

        /*string input = "x+1+1+1";
        string modifiedInput = input.Replace("x", "0");
        Expression expression = new Expression(modifiedInput);

        try
        {
            object result = expression.Evaluate();
            string output = input.Replace("x", result.ToString());
            Console.WriteLine(output);
        }
        catch (EvaluationException ex)
        {
            Console.WriteLine("Evaluation Error: " + ex.Message);
        }*/
    }
}