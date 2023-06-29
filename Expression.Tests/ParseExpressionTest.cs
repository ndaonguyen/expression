using Expression.ExpressionNode;
using Expression.Parser;

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
            var expressionNode = Parser.Parser.ParseExpression("x");

            var isElementNode = expressionNode is ElementNode;
            Assert.That(isElementNode, Is.True);
            Assert.That(((ElementNode)expressionNode).Value, Is.EqualTo("x"));
        }

        [Test]
        public void TestParser1()
        {
            // input : 3
            // output: ElementNode:3
            var expressionNode = Parser.Parser.ParseExpression("3");

            var isElementNode = expressionNode is ElementNode;
            Assert.That(isElementNode, Is.True);
            Assert.That(((ElementNode)expressionNode).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestParser2()
        {
            // input : x+3
            // output: AddNode: x+3
            var expressionNode = Parser.Parser.ParseExpression("(x+3)");

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
            var expressionNode = Parser.Parser.ParseExpression("((x+3)*(x+1))");

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
        public void TestParserFail1()
        {
            // input : ((x+3)*(x+1)
            // output: fail
            var expressionNode = Parser.Parser.ParseExpression("((x+3)*(x+1)");

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
        public void TestParserFail2()
        {
            // input : x+3)*(x+1)
            // output: fail
            var expressionNode = Parser.Parser.ParseExpression("x+3)*(x+1)");

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
    }
}