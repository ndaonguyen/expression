using Expression.Model;
using Expression.Operation;

namespace Expression.Tests
{
    public class ParseExpressionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestParser()
        {
            // input : x
            // output: ElementNode: x
            var expressionNode = Parser.TryParseExpression("x").Value!;

            var isElementNode = expressionNode is ElementNode;
            Assert.That(isElementNode, Is.True);
            Assert.That(((ElementNode)expressionNode).Value, Is.EqualTo("x"));
        }

        [Test]
        public void TestParser1()
        {
            // input : 3
            // output: ElementNode:3
            var expressionNode = Parser.TryParseExpression("3").Value!;

            var isElementNode = expressionNode is ElementNode;
            Assert.That(isElementNode, Is.True);
            Assert.That(((ElementNode)expressionNode).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestParser2()
        {
            // input : x+3
            // output: AddNode: x+3
            var expressionNode = Parser.TryParseExpression("(x+3)").Value!;

            var isAddNode = expressionNode is AddNode;
            Assert.IsTrue(isAddNode);

            var addNode = expressionNode as AddNode;
            Assert.That(addNode!.Expressions.Count(), Is.EqualTo(2));
            Assert.That(((ElementNode)addNode.Expressions.First()).Value, Is.EqualTo("x"));
            Assert.That(((ElementNode)addNode.Expressions.ToList()[1]).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestParser3()
        {
            // input : ((x+3)*(x+1))
            // output: multiple node of 2 add nodes (x+3) and (x+1)
            var expressionNode = Parser.TryParseExpression("((x+3)*(x+1))").Value!;

            var isCorrectNode = expressionNode is MultiplyNode;
            Assert.IsTrue(isCorrectNode);

            var parentNode = expressionNode as MultiplyNode;
            Assert.That(parentNode!.Expressions.Count(), Is.EqualTo(2));

            var firstAddNode = (AddNode)parentNode.Expressions.First();
            Assert.That(((ElementNode)firstAddNode.Expressions.First()).Value, Is.EqualTo("x"));
            Assert.That(((ElementNode)firstAddNode.Expressions.ToList()[1]).Value, Is.EqualTo("3"));


            var secondAddNode = (AddNode)parentNode.Expressions.ToList()[1];
            Assert.That(((ElementNode)secondAddNode.Expressions.First()).Value, Is.EqualTo("x"));
            Assert.That(((ElementNode)secondAddNode.Expressions.ToList()[1]).Value, Is.EqualTo("1"));
        }

        [Test]
        public void TestParser4()
        {
            // input : ((1+2+1)*(1+1)*x)
            // output: multiple node of 3 nodes (1+2+1) and (1+1) and x
            var expressionNode = Parser.TryParseExpression("((1+2+1)*(1+1)*x)").Value!;

            var isCorrectNode = expressionNode is MultiplyNode;
            Assert.IsTrue(isCorrectNode);

            var parentNode = expressionNode as MultiplyNode;
            Assert.That(parentNode!.Expressions.Count(), Is.EqualTo(3));

            var firstNode = (AddNode)parentNode.Expressions.First();
            Assert.That(((ElementNode)firstNode.Expressions.First()).Value, Is.EqualTo("1"));
            Assert.That(((ElementNode)firstNode.Expressions.ToList()[1]).Value, Is.EqualTo("2"));
            Assert.That(((ElementNode)firstNode.Expressions.ToList()[2]).Value, Is.EqualTo("1"));

            var secondNode = (AddNode)parentNode.Expressions.ToList()[1];
            Assert.That(((ElementNode)secondNode.Expressions.First()).Value, Is.EqualTo("1"));
            Assert.That(((ElementNode)secondNode.Expressions.ToList()[1]).Value, Is.EqualTo("1"));

            var thirdNode = (ElementNode)parentNode.Expressions.ToList()[2];
            Assert.That(thirdNode.Value, Is.EqualTo("x"));
        }

        [Test]
        public void TestParser5()
        {
            // input : (1+(2*x)+1)
            // output: add node of 3 nodes 1 and (2*x) and 1
            var expressionNode = Parser.TryParseExpression("(1+(2*x)+1)").Value!;

            var isCorrectNode = expressionNode is AddNode;
            Assert.IsTrue(isCorrectNode);

            var parentNode = expressionNode as AddNode;
            Assert.That(parentNode!.Expressions.Count(), Is.EqualTo(3));

            var firstAddNode = (ElementNode)parentNode.Expressions.First();
            Assert.That(firstAddNode.Value, Is.EqualTo("1"));

            var secondAddNode = (MultiplyNode)parentNode.Expressions.ToList()[1];
            Assert.That(((ElementNode)secondAddNode.Expressions.First()).Value, Is.EqualTo("2"));
            Assert.That(((ElementNode)secondAddNode.Expressions.ToList()[1]).Value, Is.EqualTo("x"));

            var thirdNode = (ElementNode)parentNode.Expressions.ToList()[2];
            Assert.That(thirdNode.Value, Is.EqualTo("1"));
        }

        [Test]
        public void TestParser6()
        {
            // input : ((1+(2*x)+1)*(x+1))
            // output: multiply node of 2 nodes
            //          1st node: Add node of 3 node: 1, 2*x and 1
            //          2nd node: Add node of 2 node: x and 1
            var expressionNode = Parser.TryParseExpression("((1+(2*x)+1)*(x+1))").Value!;

            var isCorrectNode = expressionNode is MultiplyNode;
            Assert.IsTrue(isCorrectNode);

            var parentNode = expressionNode as MultiplyNode;
            Assert.That(parentNode!.Expressions.Count(), Is.EqualTo(2));

            var firstNode = (AddNode)parentNode.Expressions.First();
            Assert.That(((ElementNode)firstNode.Expressions.First()).Value, Is.EqualTo("1"));

            var firstNodeChild = (MultiplyNode)firstNode.Expressions.ToList()[1];
            Assert.That(((ElementNode)firstNodeChild.Expressions.First()).Value, Is.EqualTo("2"));
            Assert.That(((ElementNode)firstNodeChild.Expressions.ToList()[1]).Value, Is.EqualTo("x"));

            Assert.That(((ElementNode)firstNode.Expressions.ToList()[2]).Value, Is.EqualTo("1"));

            var secondNode = (AddNode)parentNode.Expressions.ToList()[1];
            Assert.That(((ElementNode)secondNode.Expressions.First()).Value, Is.EqualTo("x"));
            Assert.That(((ElementNode)secondNode.Expressions.ToList()[1]).Value, Is.EqualTo("1"));
        }


        [Test]
        public void TestParserFail1()
        {
            // input : ((x+3)*(x+1)
            // output: fail
            var expressionNodeResult = Parser.TryParseExpression("((x+3)*(x+1)");
            Assert.IsTrue(expressionNodeResult.Failed);
        }

        [Test]
        public void TestParserFail2()
        {
            // input : x+3)*(x+1)
            // output: fail
            var expressionNodeResult = Parser.TryParseExpression("x+3)*(x+1)");
            Assert.IsTrue(expressionNodeResult.Failed);
        }

        [Test]
        public void TestParserFail3()
        {
            // input : (((x+3)*))(x+1)
            // output: fail
            var expressionNodeResult = Parser.TryParseExpression("(((x+3)*))(x+1)");
            Assert.IsTrue(expressionNodeResult.Failed);
        }
    }
}