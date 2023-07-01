using Expression.ExpressionNode;
using Expression.Parser;

namespace Expression.Tests
{
    public class AnalyzeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AnalyzeTest4()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("2"));
            Assert.That(result.WithVariable, Is.EqualTo(false));
            Assert.That(result.Counter, Is.EqualTo(1));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void AnalyzeTest1()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("x"));
            Assert.That(result.WithVariable, Is.EqualTo(false));
            Assert.That(result.Counter, Is.EqualTo(1));
            Assert.That(result.Power, Is.EqualTo(1));
        }

        [Test]
        public void AnalyzeTest2()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("x^3"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(1));
            Assert.That(result.Power, Is.EqualTo(3));
        }

        [Test]
        public void AnalyzeTest3()
        {
            var result = ElementNodeAnalysis.AnalyzeNode(new ElementNode("2*x^3"));
            Assert.That(result.WithVariable, Is.EqualTo(true));
            Assert.That(result.Counter, Is.EqualTo(2));
            Assert.That(result.Power, Is.EqualTo(3));
        }
    }
}