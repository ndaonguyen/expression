using Expression.ExpressionNode;
using Expression.Parser;

namespace Expression.Tests
{
    public class EvaluateExpressionTest
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void TestEvaluate1()
        {
            // input : x => output: x
            Assert.AreEqual("x", Evaluation.EvaluateExpression(new ElementNode("x")));
        }

        [Test]
        public void TestEvaluate2()
        {
            // input : x+1 => output: x+1
            Assert.AreEqual("x+1", Evaluation.EvaluateExpression(new ElementNode("x+1")));
        }

        [Test]
        public void TestEvaluate3()
        {
            // input : (1+2+(2+x)) => output:  x+5

            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));
            addNode.AddElement(new ElementNode("2"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));
            addNodeChild.AddElement(new ElementNode("x"));

            addNode.AddElement(addNodeChild);

            Assert.AreEqual("x+5", Evaluation.EvaluateExpression(addNode));
        }

        [Test]
        public void TestEvaluate4()
        {
            // input : (1+2+(2+(3+x)) => output:  x+8

            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));
            addNode.AddElement(new ElementNode("2"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("3"));
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(addNodeChild2);

            addNode.AddElement(addNodeChild);

            Assert.AreEqual("x+8", Evaluation.EvaluateExpression(addNode));
        }

        [Test]
        public void TestEvaluate5()
        {
            // input : ((1+2+(2+(3+x))*(1+(2+x)) => output:  (x+8)*(x+3)

            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));
            addNode.AddElement(new ElementNode("2"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("3"));
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(addNodeChild2);

            addNode.AddElement(addNodeChild);

            var addNodeRight = new AddNode();
            addNodeRight.AddElement(new ElementNode("1"));

            var addNodeRightChild = new AddNode();
            addNodeRightChild.AddElement(new ElementNode("2"));
            addNodeRightChild.AddElement(new ElementNode("x"));
            addNodeRight.AddElement(addNodeRightChild);

            var multiplyChild = new MultiplyNode();
            multiplyChild.AddElement(addNode);
            multiplyChild.AddElement(addNodeRight);

            Assert.AreEqual("(x+8)*(x+3)", Evaluation.EvaluateExpression(multiplyChild));
        }

        [Test]
        public void TestEvaluate6()
        {
            // input : (1*(2*x)*3) => output:  x*6

            var multiplyNodeParent = new MultiplyNode();
            multiplyNodeParent.AddElement(new ElementNode("1"));

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("2"));
            multiplyNode.AddElement(new ElementNode("x"));

            multiplyNodeParent.AddElement(multiplyNode);
            multiplyNodeParent.AddElement(new ElementNode("3"));

            Assert.AreEqual("x*6", Evaluation.EvaluateExpression(multiplyNodeParent));
        }

        [Test]
        public void TestEvaluate55()
        {
            // input : ((1+(2*x)+1)*(x+1)) => output:  2*x^2+4*x+2

            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("2"));
            multiplyNode.AddElement(new ElementNode("x"));

            addNode.AddElement(multiplyNode);
            addNode.AddElement(new ElementNode("1"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(new ElementNode("1"));

            var multiplyNodeOutside = new MultiplyNode();
            multiplyNodeOutside.AddElement(addNode);
            multiplyNodeOutside.AddElement(addNodeChild);

            Assert.AreEqual("x+5", Evaluation.EvaluateExpression(multiplyNodeOutside));
        }
    }
}