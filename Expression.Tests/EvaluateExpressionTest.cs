using Expression.Model;
using Expression.Operation;

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
            
            var elementNode = new ElementNode("3");
            var resultNode = Evaluation.EvaluateExpression(elementNode);
            Assert.That(resultNode, Is.EqualTo("3"));
        }

        [Test]
        public void TestEvaluate1()
        {
            // input : x => output: x

            var elementNode = new ElementNode("x");
            var resultNode = Evaluation.EvaluateExpression(elementNode);
            Assert.That(resultNode, Is.EqualTo("x"));
        }

        [Test]
        public void TestEvaluate2()
        {
            // input : x+1 => output: x+1
            var nodeChild = new AddNode();
            nodeChild.AddElement(new ElementNode("x"));
            nodeChild.AddElement(new ElementNode("1"));

            var resultNode = Evaluation.EvaluateExpression(nodeChild);
            Assert.That(resultNode, Is.EqualTo("x+1"));
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

            var resultNode = Evaluation.EvaluateExpression(addNode);
            Assert.That(resultNode, Is.EqualTo("x+5"));
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

            var resultNode = Evaluation.EvaluateExpression(addNode);
            Assert.That(resultNode, Is.EqualTo("x+8"));
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

            var resultNode = Evaluation.EvaluateExpression(addNodeParent);
            Assert.That(resultNode, Is.EqualTo("2*x+11"));
        }
        
        // test elementNode * elementNode
        [Test]
        public void TestEvaluate551()
        {
            // input : x*x
            // output: x^2

            var node = new MultiplyNode();
            node.AddElement(new ElementNode("x"));
            node.AddElement(new ElementNode("x"));

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("x^2"));
        }

        [Test]
        public void TestEvaluate552()
        {
            // input : 2*x
            // output: 2*x

            var node = new MultiplyNode();
            node.AddElement(new ElementNode("2"));
            node.AddElement(new ElementNode("x"));

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x"));
        }

        [Test]
        public void TestEvaluate553()
        {
            // input : 2*2
            // output: 4

            var node = new MultiplyNode();
            node.AddElement(new ElementNode("2"));
            node.AddElement(new ElementNode("2"));

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("4"));
        }

        [Test]
        public void TestEvaluate554()
        {
            // input : (2*x)*(x^2)
            // output: 2*x^3

            var node = new MultiplyNode();
            node.AddElement(new ElementNode("2*x"));
            node.AddElement(new ElementNode("x^2"));

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x^3"));
        }

        // test elementNode * AddNode
        [Test]
        public void TestEvaluate555()
        {
            // input : x*(x+1)
            // output: x^2 +x

            var elementNode = new ElementNode("x");

            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("x"));
            addNodeChild.AddElement(new ElementNode("1"));

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(addNodeChild);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("x^2+x"));
        }

        
        [Test]
        public void TestEvaluate556()
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

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(addNodeChild);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("x^3+2*x"));
        }

        [Test]
        public void TestEvaluate557()
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

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(addNodeChild);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x^2+2*x"));
        }

        [Test]
        public void TestEvaluate558()
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

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(addNode);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("x^3+4*x"));
        }

        // test elementNode * Multiply Node
        [Test]
        public void TestEvaluate559()
        {
            // input : (x*(2*x))
            // output: 2x^2

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("2"));
            nodeChild.AddElement(new ElementNode("x"));

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(nodeChild);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x^2"));
        }

        [Test]
        public void TestEvaluate560()
        {
            // input : (x*(x*x))
            // output: x^3+2*x

            var elementNode = new ElementNode("x");

            var nodeChild = new MultiplyNode();
            nodeChild.AddElement(new ElementNode("x"));
            nodeChild.AddElement(new ElementNode("x"));

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(nodeChild);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("x^3"));
        }

        [Test]
        public void TestEvaluate561()
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

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(multiplyNode);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("x^3+2*x^2"));
        }

        [Test]
        public void TestEvaluate562()
        {
            // input : (x*(x*(2*x)))
            // output: 2*x^3

            var elementNode = new ElementNode("x");

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("x"));

            var nodeChild2 = new MultiplyNode();
            nodeChild2.AddElement(new ElementNode("2"));
            nodeChild2.AddElement(new ElementNode("x"));
            multiplyNode.AddElement(nodeChild2);

            var node = new MultiplyNode();
            node.AddElement(elementNode);
            node.AddElement(multiplyNode);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x^3"));
        }

        // test AddNode or Multiply Node to do multiply
        [Test]
        public void TestEvaluate563()
        {
            // input : ((x+1)*(x+2))
            // output: x^2+3*x+2

            var addNodeChild1 = new AddNode();
            addNodeChild1.AddElement(new ElementNode("x"));
            addNodeChild1.AddElement(new ElementNode("1"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild2.AddElement(new ElementNode("2"));

            var node = new MultiplyNode();
            node.AddElement(addNodeChild1);
            node.AddElement(addNodeChild2);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("x^2+3*x+2"));
        }

        [Test]
        public void TestEvaluate564()
        {
            // input : ((2*x)*(x+2))
            // output: 2*x^2+4*x

            var nodeChild1 = new MultiplyNode();
            nodeChild1.AddElement(new ElementNode("2"));
            nodeChild1.AddElement(new ElementNode("x"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild2.AddElement(new ElementNode("2"));

            var node = new MultiplyNode();
            node.AddElement(nodeChild1);
            node.AddElement(addNodeChild2);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x^2+4*x"));
        }

        [Test]
        public void TestEvaluate565()
        {
            // input : ((2*x*x)*(x+2))
            // output: 2*x^3+4*x^2

            var nodeChild1 = new MultiplyNode();
            nodeChild1.AddElement(new ElementNode("2"));
            nodeChild1.AddElement(new ElementNode("x"));
            nodeChild1.AddElement(new ElementNode("x"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild2.AddElement(new ElementNode("2"));

            var node = new MultiplyNode();
            node.AddElement(nodeChild1);
            node.AddElement(addNodeChild2);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x^3+4*x^2"));
        }

        // test multiply of more than 2 elements
        [Test]
        public void TestEvaluate566()
        {
            // input : ((2*x*x)*(x+2))
            // output: 2*x^3+4*x^2

            var nodeChild1 = new MultiplyNode();
            nodeChild1.AddElement(new ElementNode("2"));
            nodeChild1.AddElement(new ElementNode("x"));
            nodeChild1.AddElement(new ElementNode("x"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("x"));
            addNodeChild2.AddElement(new ElementNode("2"));

            var node = new MultiplyNode();
            node.AddElement(nodeChild1);
            node.AddElement(addNodeChild2);

            var resultNode = Evaluation.EvaluateExpression(node);
            Assert.That(resultNode, Is.EqualTo("2*x^3+4*x^2"));
        }

        [Test]
        public void TestEvaluate570()
        {
            // input :((1+(2*x)+1) => output:  2*x+2

            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("2"));
            multiplyNode.AddElement(new ElementNode("x"));

            addNode.AddElement(multiplyNode);
            addNode.AddElement(new ElementNode("1"));

            var resultNode = Evaluation.EvaluateExpression(addNode);
            Assert.That(resultNode, Is.EqualTo("2*x+2"));
        }

        [Test]
        public void TestEvaluate567()
        {
            // input : ((1+(2*x)+1)*(x+1))
            // output:  2*x^2+4*x+2

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

            var resultNode = Evaluation.EvaluateExpression(multiplyNodeOutside);
            Assert.That(resultNode, Is.EqualTo("2*x^2+4*x+2"));
        }


        [Test]
        public void TestEvaluate568()
        {
            // input : ((1+2+(2+(3+x))*(1+(2+x))
            // output:  (x+8)*(x+3) => x^2+11*x+24

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

            var resultNode = Evaluation.EvaluateExpression(multiplyChild);
            Assert.That(resultNode, Is.EqualTo("x^2+11*x+24"));
        }

        [Test]
        public void TestEvaluate569()
        {
            // input : (1*(2*x)*3) => output:  6*x

            var multiplyNodeParent = new MultiplyNode();
            multiplyNodeParent.AddElement(new ElementNode("1"));

            var multiplyNode = new MultiplyNode();
            multiplyNode.AddElement(new ElementNode("2"));
            multiplyNode.AddElement(new ElementNode("x"));

            multiplyNodeParent.AddElement(multiplyNode);
            multiplyNodeParent.AddElement(new ElementNode("3"));

            var resultNode = Evaluation.EvaluateExpression(multiplyNodeParent);
            Assert.That(resultNode, Is.EqualTo("6*x"));
        }

        // test a complicated case
        [Test]
        public void TestEvaluate571()
        {
            // input : ( ((1+x)*(2+((3+x)*2))) * (3+x))
            // output:  2*x^3+16*x^2+38*x+24

            //(1+x)
            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));
            addNode.AddElement(new ElementNode("x"));

            //(2+((3+x)*2))
            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("3"));
            addNodeChild2.AddElement(new ElementNode("x"));

            var multiplyNodeChild2 = new MultiplyNode();
            multiplyNodeChild2.AddElement(addNodeChild2);
            multiplyNodeChild2.AddElement(new ElementNode("2"));

            addNodeChild.AddElement(multiplyNodeChild2);
            
            //((1+x)*(2+((3+x)*2)))
            var multiplyNodeLeft = new MultiplyNode();
            multiplyNodeLeft.AddElement(addNode);
            multiplyNodeLeft.AddElement(addNodeChild);

            //(1+(2+x))
            var addNodeRightChild = new AddNode();
            addNodeRightChild.AddElement(new ElementNode("3"));
            addNodeRightChild.AddElement(new ElementNode("x"));

            //(((1+x)*(2+((3+x)*2)))*((2+x)))
            var multiplyChild = new MultiplyNode();
            multiplyChild.AddElement(multiplyNodeLeft);
            multiplyChild.AddElement(addNodeRightChild);

            var resultNode = Evaluation.EvaluateExpression(multiplyChild);
            Assert.That(resultNode, Is.EqualTo("2*x^3+16*x^2+38*x+24"));
        }

        // test a complicated case 2
        [Test]
        public void TestEvaluate572()
        {
            // input : ((1+x)*(2+((3+x)*2)))
            // output:  2*x^2+10*x+8

            //(1+x)
            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));
            addNode.AddElement(new ElementNode("x"));

            //(2+((3+x)*2))
            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("3"));
            addNodeChild2.AddElement(new ElementNode("x"));

            var multiplyNodeChild2 = new MultiplyNode();
            multiplyNodeChild2.AddElement(addNodeChild2);
            multiplyNodeChild2.AddElement(new ElementNode("2"));

            addNodeChild.AddElement(multiplyNodeChild2);

            //((1+x)*(2+((3+x)*2)))
            var multiplyNodeLeft = new MultiplyNode();
            multiplyNodeLeft.AddElement(addNode);
            multiplyNodeLeft.AddElement(addNodeChild);
            
            var resultNode = Evaluation.EvaluateExpression(multiplyNodeLeft);
            Assert.That(resultNode, Is.EqualTo("2*x^2+10*x+8"));
        }

        // test a complicated case 3
        [Test]
        public void TestEvaluate573()
        {
            // input : ((1+x)*(2+((3+x)*2)))*x)
            // output:  2*x^3+10*x^2+8*x

            //(1+x)
            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));
            addNode.AddElement(new ElementNode("x"));

            //(2+((3+x)*2))
            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("3"));
            addNodeChild2.AddElement(new ElementNode("x"));

            var multiplyNodeChild2 = new MultiplyNode();
            multiplyNodeChild2.AddElement(addNodeChild2);
            multiplyNodeChild2.AddElement(new ElementNode("2"));

            addNodeChild.AddElement(multiplyNodeChild2);

            //((1+x)*(2+((3+x)*2)))
            var multiplyNodeLeft = new MultiplyNode();
            multiplyNodeLeft.AddElement(addNode);
            multiplyNodeLeft.AddElement(addNodeChild);

            //(((1+x)*(2+((3+x)*2)))*x)
            var multiplyChild = new MultiplyNode();
            multiplyChild.AddElement(new ElementNode("x"));
            multiplyChild.AddElement(multiplyNodeLeft);

            var resultNode = Evaluation.EvaluateExpression(multiplyChild);
            Assert.That(resultNode, Is.EqualTo("2*x^3+10*x^2+8*x"));
        }

        // test a complicated case 4
        [Test]
        public void TestEvaluate574()
        {
            // input : ((1+x)*(2+((3+x)*2)))*3)
            // output:  6*x^2+30*x+24

            //(1+x)
            var addNode = new AddNode();
            addNode.AddElement(new ElementNode("1"));
            addNode.AddElement(new ElementNode("x"));

            //(2+((3+x)*2))
            var addNodeChild = new AddNode();
            addNodeChild.AddElement(new ElementNode("2"));

            var addNodeChild2 = new AddNode();
            addNodeChild2.AddElement(new ElementNode("3"));
            addNodeChild2.AddElement(new ElementNode("x"));

            var multiplyNodeChild2 = new MultiplyNode();
            multiplyNodeChild2.AddElement(addNodeChild2);
            multiplyNodeChild2.AddElement(new ElementNode("2"));

            addNodeChild.AddElement(multiplyNodeChild2);

            //((1+x)*(2+((3+x)*2)))
            var multiplyNodeLeft = new MultiplyNode();
            multiplyNodeLeft.AddElement(addNode);
            multiplyNodeLeft.AddElement(addNodeChild);

            //(((1+x)*(2+((3+x)*2)))*3)
            var multiplyChild = new MultiplyNode();
            multiplyChild.AddElement(multiplyNodeLeft);
            multiplyChild.AddElement(new ElementNode("3"));

            var resultNode = Evaluation.EvaluateExpression(multiplyChild);
            Assert.That(resultNode, Is.EqualTo("6*x^2+30*x+24"));
        }

    }
}