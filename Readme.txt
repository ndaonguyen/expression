Please run Program to see input and output. You can try to input more and see how the output looks like.

Beside that, to have a better understanding about the project, please take a look at the unit tests.
_ ParserExpressionTest : is to test Parser (input a string => output Expression).  
_ EvaluateExpressTest: is to test Evaluation ( input a Expression => output the expression evaluation).In these unit test, I also give some extremely complicated expression as well.
_ AnalyzeTest: this is to test ElementNodeAnalysis which is supporting only.

For this project, in the future, if we want to have Minus operation or Divide operation, we can add to the Model folder new classes ( same as AddNode and MultiplyNode). Then add to the IExpressionNodeVisitor 2 Visits for the new Models. Once we try to build, the program will be fail because it requires you to implement those Visitor to fullfill the new 2 Models. So in this case, we don't forget to implement the new changes for the new Models.
