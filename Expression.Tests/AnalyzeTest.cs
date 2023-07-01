using Expression.ExpressionNode;
using Expression.Parser;
using static Expression.Parser.ElementNodeAnalysis;

namespace Expression.Tests
{
    public class AnalyzeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AnalyzeTest1()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("2"));
            Assert.That(result.WithVariable, Is.EqualTo(false));
            Assert.That(result.Counter, Is.EqualTo(2));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void AnalyzeTest2()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("x"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(1));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void AnalyzeTest3()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("2*x"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(2));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void AnalyzeTest4()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("12*x"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(12));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void AnalyzeTest5()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("x^3"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(1));
            Assert.That(result.Power, Is.EqualTo(3));
        }

        [Test]
        public void AnalyzeTest6()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("x^31"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(1));
            Assert.That(result.Power, Is.EqualTo(31));
        }

        [Test]
        public void AnalyzeTest7()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("2*x^3"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(2));
            Assert.That(result.Power, Is.EqualTo(3));
        }

        [Test]
        public void AnalyzeTest8()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("20*x^30"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(20));
            Assert.That(result.Power, Is.EqualTo(30));
        }


        // test multiply node
        [Test]
        public void TestMultiplyNode1()
        {
            // node x multiply node 2 => expect nodeModel with counter is 1, power is 1 and with variable
            var node1 = new NodeModel()
            {
                Counter = 1,
                Power = 1,
                WithVariable = true
            };

            var node2 = new NodeModel()
            {
                Counter = 2,
                Power = 1,
                WithVariable = false
            };

            var result = node1.MultiplyModel(node2);

            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(2));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void TestMultiplyNode2()
        {
            // node 2 multiply node 2 => expect nodeModel with counter is 4, power is 1 and with variable is false
            var node1 = new NodeModel()
            {
                Counter = 2,
                Power = 1,
                WithVariable = false
            };

            var node2 = new NodeModel()
            {
                Counter = 2,
                Power = 1,
                WithVariable = false
            };

            var result = node1.MultiplyModel(node2);

            Assert.That(result.WithVariable, Is.EqualTo(false));
            Assert.That(result.Counter, Is.EqualTo(4));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void TestMultiplyNode3()
        {
            // node 2*x multiply node x => expect nodeModel with counter is 2, power is 2 and with variable is true
            var node1 = new NodeModel()
            {
                Counter = 2,
                Power = 1,
                WithVariable = true
            };

            var node2 = new NodeModel()
            {
                Counter = 1,
                Power = 1,
                WithVariable = true
            };

            var result = node1.MultiplyModel(node2);

            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(2));
            Assert.That(result.Power, Is.EqualTo(2));
        }

        [Test]
        public void TestMultiplyNode4()
        {
            // node x multiply node x => expect nodeModel with counter is 1, power is 2 and with variable is true
            var node1 = new NodeModel()
            {
                Counter = 1,
                Power = 1,
                WithVariable = true
            };

            var node2 = new NodeModel()
            {
                Counter = 1,
                Power = 1,
                WithVariable = true
            };

            var result = node1.MultiplyModel(node2);

            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(1));
            Assert.That(result.Power, Is.EqualTo(2));
        }

        [Test]
        public void TestMultiplyNode5()
        {
            // node 2*x^2 multiply node x => expect nodeModel with counter is 2, power is 3 and with variable is true
            var node1 = new NodeModel()
            {
                Counter = 2,
                Power = 2,
                WithVariable = true
            };

            var node2 = new NodeModel()
            {
                Counter = 1,
                Power = 1,
                WithVariable = true
            };

            var result = node1.MultiplyModel(node2);

            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(2));
            Assert.That(result.Power, Is.EqualTo(3));
        }
    }
}