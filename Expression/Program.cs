using Expression.Operation;

namespace Expression;

class Program
{
    static void Main()
    {
        var expr1 = "3";
        var tryResult1 = Parser.TryParseExpression(expr1);
        var exprNode1 = tryResult1.Value;
        var resultExpr1 = Evaluation.EvaluateExpression(exprNode1!);
        Console.WriteLine($"Input {expr1}. Output: {resultExpr1}");

        var expr2 = "x";
        var tryResult2 = Parser.TryParseExpression(expr2);
        var exprNode2 = tryResult2.Value;
        var resultExpr2 = Evaluation.EvaluateExpression(exprNode2!);
        Console.WriteLine($"Input {expr2}. Output: {resultExpr2}");

        var expr3 = "(x+1)";
        var tryResult3 = Parser.TryParseExpression(expr3);
        var exprNode3 = tryResult3.Value;
        var resultExpr3 = Evaluation.EvaluateExpression(exprNode3!);
        Console.WriteLine($"Input {expr3}. Output: {resultExpr3}");

        var expr4 = "((1+2+1)*(1+1)*x)";
        var tryResult4 = Parser.TryParseExpression(expr4);
        var exprNode4 = tryResult4.Value;
        var resultExpr4 = Evaluation.EvaluateExpression(exprNode4!);
        Console.WriteLine($"Input {expr4}. Output: {resultExpr4}");

        var expr5 = "(1+(2*x)+1)";
        var tryResult5 = Parser.TryParseExpression(expr5);
        var exprNode5 = tryResult5.Value;
        var resultExpr5 = Evaluation.EvaluateExpression(exprNode5!);
        Console.WriteLine($"Input {expr5}. Output: {resultExpr5}");

        var expr6 = "((1+(2*x)+1)*(x+1))";
        var tryResult6 = Parser.TryParseExpression(expr6);
        var exprNode6 = tryResult6.Value;
        var resultExpr6 = Evaluation.EvaluateExpression(exprNode6!);
        Console.WriteLine($"Input {expr6}. Output: {resultExpr6}");
    }
}