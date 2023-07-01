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
        public void TestEvaluate()
        {
            // input : 3 => output: 3
            Assert.That(Evaluation.EvaluateExpression(new ElementNode("3")), Is.EqualTo("3"));
        }


        [Test]
        public void TestEvaluate1()
        {
            // input : x => output: x
            Assert.That(Evaluation.EvaluateExpression(new ElementNode("x")), Is.EqualTo("x"));
        }

        [Test]
        public void TestEvaluate2()
        {
            // input : x+1 => output: x+1
            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(new ElementNode("1"));

            Assert.That(Evaluation.EvaluateExpression(addNodeChild), Is.EqualTo("x+1"));
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

            Assert.That(Evaluation.EvaluateExpression(addNode), Is.EqualTo("x+5"));
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

            Assert.That(Evaluation.EvaluateExpression(addNode), Is.EqualTo("x+8"));
        }

        [Test]
        public void TestEvaluate5()
        {
            // input : ((1+2+(2+(3+x))+(x+2+1)) => output:  x+11+x

            var addNode1 = new AddNode();
            addNode1.AddElement(new ElementNode("1"));
            addNode1.AddElement(new ElementNode("2"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("3"));
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(addNodeChild2);

            addNode1.AddElement(addNodeChild);

            var addNode2 = new AddNode();
            addNode2.AddElement(new ElementNode("x"));
            addNode2.AddElement(new ElementNode("2"));
            addNode2.AddElement(new ElementNode("1"));

            var addNodeParent = new AddNode();
            addNodeParent.AddElement(addNode1);
            addNodeParent.AddElement(addNode2);

            Assert.That(Evaluation.EvaluateExpression(addNodeParent), Is.EqualTo("2*x+11"));
        }

        [Test]
        public void TestEvaluate6()
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

            Assert.That(Evaluation.EvaluateExpression(multiplyChild), Is.EqualTo("(x+8)*(x+3)"));
        }

        [Test]
        public void TestEvaluate7()
        {
            // input : (1*(2*x)*3) => output:  x*6

            var multiplyNodeParent = new MultiplyNode();
            multiplyNodeParent.AddElement(new ElementNode("1"));

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("2"));
            multiplyNode.AddElement(new ElementNode("x"));

            multiplyNodeParent.AddElement(multiplyNode);
            multiplyNodeParent.AddElement(new ElementNode("3"));

            Assert.That(Evaluation.EvaluateExpression(multiplyNodeParent), Is.EqualTo("x*6"));
        }

        [Test]
        public void TestEvaluate8()
        {
            // input :((1+(2*x)+1) => output:  2*x+2

            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("2"));
            multiplyNode.AddElement(new ElementNode("x"));

            addNode.AddElement(multiplyNode);
            addNode.AddElement(new ElementNode("1"));

            Assert.That(Evaluation.EvaluateExpression(addNode), Is.EqualTo("2*x+2"));
        }

        [Test]
        public void TestEvaluate9()
        {
            // input :(x*(x+1)) 
            // output: x*x+x

            
        }

        [Test]
        public void TestEvaluate10()
        {
            // input :((x+2)*(x+1)) 
            // output: x*x+3*x+2


        }

        /*
        [Test]
        public void TestEvaluate11()
        { 
            // input :((2*x)*(x+1)) 
            // output: 2x*x+2*x


        }

        [Test]
        public void TestEvaluate11()
        {
            // input :(x*(2*x+1)) 
            // output: 2x*x+2*x


        }


        [Test]
        public void TestEvaluate11()
        {
            // input :(x*(((x+1)*(x+2))+1) 
            // output: x*x*x+3*x*x+3*x


        }*/

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

            Assert.That(Evaluation.EvaluateExpression(multiplyNodeOutside), Is.EqualTo("x+5"));
        }


        // ignore these, im testing

        // test elementNode * elementNode


        [Test]
        public void TestEvaluate221()
        {
            // input : x*x
            // output: x^2

            var elementNode = new ElementNode("x");
            var resultNode = elementNode.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^2"));
        }

        [Test]
        public void TestEvaluate222()
        {
            // input : 2*x
            // output: 2*x

            var elementNode1 = new ElementNode("x");
            var elementNode2 = new ElementNode("2");
            var resultNode = elementNode1.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode2));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("2*x"));
        }

        [Test]
        public void TestEvaluate223()
        {
            // input : 2*2
            // output: 4

            var elementNode1 = new ElementNode("2");
            var elementNode2 = new ElementNode("2");
            var resultNode = elementNode1.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode2));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("4"));
        }

        [Test]
        public void TestEvaluate224()
        {
            // input : (2*x)*(x^2)
            // output: 2*x^3

            var elementNode1 = new ElementNode("2*x");
            var elementNode2 = new ElementNode("x^2");
            var resultNode = elementNode1.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode2));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("2*x^3"));
        }

        // test elementNode * AddNode
        [Test]
        public void TestEvaluate225()
        {
            // input : x*(x+1)
            // output: x^2 +x

            var elementNode = new ElementNode("x");

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(new ElementNode("1"));

            var resultNode = addNodeChild.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^2+x"));
        }

        // test elementNode * AddNode
        [Test]
        public void TestEvaluate226()
        {
            // input : x*(x+1)
            // output: x^2 +x

            var elementNode = new ElementNode("x");

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(new ElementNode("1"));

            var resultNode = addNodeChild.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^2+x"));
        }

        [Test]
        public void TestEvaluate227()
        {
            // input : (x*((x*x)+2))
            // output: x^3+2*x

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("x"));
            nodeChild.AddElement(new ElementNode("x"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(nodeChild);
            addNodeChild.AddElement(new ElementNode("2"));

            var resultNode = addNodeChild.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^3+2*x"));
        }

        [Test]
        public void TestEvaluate228()
        {
            // input : (x*((2*x)+2))
            // output: x^3+2*x

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("2"));
            nodeChild.AddElement(new ElementNode("x"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(nodeChild);
            addNodeChild.AddElement(new ElementNode("2"));

            var resultNode = addNodeChild.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("2*x^2+2*x"));
        }

        [Test]
        public void TestEvaluate229()
        {
            // input : (x*((x*x+2)+2))
            // output: x^3+4*x

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("x"));
            nodeChild.AddElement(new ElementNode("x"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(nodeChild);
            addNodeChild2.AddElement(new ElementNode("2"));

            var addNode = new AddNode();
            addNode.AddElement(addNodeChild2);
            addNode.AddElement(new ElementNode("2"));

            var resultNode = addNode.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^3+4*x")); // currently output x^3+2*x+2*x
        }

        // test elementNode * Multiply Node
        [Test]
        public void TestEvaluate330()
        {
            // input : (x*(2*x))
            // output: 2x^2

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("2"));
            nodeChild.AddElement(new ElementNode("x"));

            var resultNode = nodeChild.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("2*x^2"));
        }

        [Test]
        public void TestEvaluate331()
        {
            // input : (x*(x*x))
            // output: x^3+2*x

            var elementNode = new ElementNode("x");

            var node = new MultiplyNode();
            node.AddElement(new ElementNode("x"));
            node.AddElement(new ElementNode("x"));

            var resultNode = node.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^3"));
        }

        [Test]
        public void TestEvaluate332()
        {
            // input : (x*(x*(x+2)))
            // output: x^3+2*x^2

            var elementNode = new ElementNode("x");

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("x"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild2.AddElement(new ElementNode("2"));
            multiplyNode.AddElement(addNodeChild2);

            var resultNode = multiplyNode.Accept(new Evaluation.ElementNodeMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^3+2*x^2"));
        }


        // Test others
        [Test]
        public void TestEvaluate111()
        {
            // input : x*(x+1)
            // output: x^2 +x

            var elementNode = new ElementNode("x");

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(new ElementNode("1"));

            var resultNode = addNodeChild.Accept(new Evaluation.ExpressionMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^2+x"));
        }

        [Test]
        public void TestEvaluate112()
        {
            // input : x*x
            // output: x^2

            var elementNode = new ElementNode("x");
            var resultNode = elementNode.Accept(new Evaluation.ExpressionMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^2"));
        }

        [Test]
        public void TestEvaluate113()
        {
            // input : x*(2*x)
            // output: 2x^2

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("2"));
            nodeChild.AddElement(new ElementNode("x"));

            var resultNode = nodeChild.Accept(new Evaluation.ExpressionMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("2*x^2"));
        }

        [Test]
        public void TestEvaluate114()
        {
            // input : (x*((x*x)+2))
            // output: x^3+2*x

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("x"));
            nodeChild.AddElement(new ElementNode("x"));

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(nodeChild);
            addNodeChild.AddElement(new ElementNode("2"));

            var resultNode = addNodeChild.Accept(new Evaluation.ExpressionMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^3+2*x"));
        }


        [Test]
        public void TestEvaluate115()
        {
            // input : (x*((x*x+2)+2))
            // output: x^3+4*x

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("x"));
            nodeChild.AddElement(new ElementNode("x"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(nodeChild);
            addNodeChild2.AddElement(new ElementNode("2"));

            var addNode = new AddNode();
            addNode.AddElement(addNodeChild2);
            addNode.AddElement(new ElementNode("2"));

            var resultNode = addNode.Accept(new Evaluation.ExpressionMultiplyVisitor(elementNode));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("x^3+4*x")); // currently output x^3+2*x+2*x
        }


        [Test]
        public void TestEvaluate1114()
        {
            // input : ((x+1)*(x+2))
            // output: x^2+3*x+2

            var addNodeChild1 = new AddNode();
            addNodeChild1.AddElement(new ElementNode("x"));
            addNodeChild1.AddElement(new ElementNode("1"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild2.AddElement(new ElementNode("2"));

            var resultNode = addNodeChild1.Accept(new Evaluation.ExpressionMultiplyVisitor(addNodeChild2));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("2*x^2")); // currently output: (x^2+2*x)+(x+2)
        }

        [Test]
        public void TestEvaluate1115()
        {
            // input : ((x+1)*(x+2))
            // output: x^2+3*x+2

            var addNodeChild1 = new AddNode();
            addNodeChild1.AddElement(new ElementNode("x"));
            addNodeChild1.AddElement(new ElementNode("1"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild2.AddElement(new ElementNode("2"));

            var resultNode = addNodeChild1.Accept(new Evaluation.ExpressionMultiplyVisitor(addNodeChild2));
            Assert.That(Evaluation.EvaluateExpression(resultNode), Is.EqualTo("2*x^2"));
        }
    }
}