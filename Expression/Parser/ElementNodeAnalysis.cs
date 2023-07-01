using System.Runtime.CompilerServices;
using Expression.ExpressionNode;
using Expression.Interface;

[assembly: InternalsVisibleTo("Expression.Tests")]
namespace Expression.Parser;

public static class ElementNodeAnalysis
{
    /// <summary>
    /// Our elementNode in this context can be x, 2*x or x^2
    /// return Dictionary{
    /// "Key": ADD => Value: Dictionary("x", "1")
    /// "Key": MULTIPLY => Value: Dictionary("x", "1")   
    /// </summary>
    internal static INodeModel AnalyzeNode(ElementNode node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        var nodeModel = new NodeModel();
        var valueStr = node.Value;

        // we only have x, 2*x, or 3*x or <number>*x or 2*x^3
        var components = valueStr.Split('*');
        if (components.Length > 2) throw new InvalidOperationException($"Wrong format for node : {node.Value}");

        if (components.Length == 2)
        {
            if (!components[1].Contains(Evaluation.VARIABLE)) throw new InvalidOperationException($"Wrong format for node : {node.Value}");

            nodeModel.Counter = int.Parse(components[0]);
            nodeModel.WithVariable = true;
        }

        // we can have x^2, or x^3
        var powerComponent = valueStr.Split('^');
        if (powerComponent.Length > 2) throw new InvalidOperationException($"Wrong format for node : {node.Value}");

        if (powerComponent.Length == 2)
        {
            if (!powerComponent[0].Contains(Evaluation.VARIABLE)) throw new InvalidOperationException($"Wrong format for node : {node.Value}");

            nodeModel.Power = int.Parse(powerComponent[1]);
            nodeModel.WithVariable = true;
        }

        return nodeModel;
    }

    internal class NodeModel : INodeModel
    {
        /// <summary>
        /// If the elementNode is 2*x^2 => counter is 2, power is 2
        ///                         x^3 => counter is 1, power is 3
        /// </summary>

        public NodeModel()
        {
            Counter = 1;
            Power = 1;
            WithVariable = false;
        }

        public int Counter { get; set; }
        public int Power { get; set; }
        public bool WithVariable { get; set; }

        public bool EqualsBase(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (NodeModel)obj;
            return WithVariable == other.WithVariable && Power == other.Power;
        }

        public TryResults<INodeModel> TryAddModel(INodeModel model)
        {
            if (EqualsBase(model))
            {
                return TryResults<INodeModel>.Succeed(new NodeModel
                {
                    Counter = Counter + model.Counter,
                    Power = Power,
                    WithVariable = WithVariable
                });
            }

            return TryResults<INodeModel>.Fail("Not the same base, not able to add together");
        }

        public INodeModel MultiplyModel(INodeModel model)
        {
            return new NodeModel()
            {
                Counter = Counter * model.Counter,
                Power = Power + model.Power,
                WithVariable = WithVariable || model.WithVariable,
            };
        }

        public override string ToString()
        {
            if (WithVariable)
            {
                var counterStr = Counter == 1 ? string.Empty : $"{Counter}*";
                var powerStr = Power == 1 ? string.Empty : $"^{Power}";
                    
                return $"{counterStr}{Evaluation.VARIABLE}{powerStr}";
            }

            return Counter.ToString();
        }
    }

    internal static TryResults<ElementNode> TryCombineForAdd(ElementNode node1, ElementNode node2)
    {
        if (node1 == null) throw new ArgumentNullException(nameof(node1));
        if (node2 == null) throw new ArgumentNullException(nameof(node2));

        var nodeModel1 = AnalyzeNode(node1);
        var nodeModel2 = AnalyzeNode(node2);
        var addResult = nodeModel1.TryAddModel(nodeModel2);
        if (addResult.Success)
        {
            return TryResults<ElementNode>.Succeed(new ElementNode(addResult.Value!.ToString()));
        }

        return TryResults<ElementNode>.Fail("Can't group");
    }

    internal static ElementNode CombineForMultiply(ElementNode node1, ElementNode node2)
    {
        if (node1 == null) throw new ArgumentNullException(nameof(node1));
        if (node2 == null) throw new ArgumentNullException(nameof(node2));

        var nodeModel1 = AnalyzeNode(node1);
        var nodeModel2 = AnalyzeNode(node2);
        var multiplyResult = nodeModel1.MultiplyModel(nodeModel2);
        return new ElementNode(multiplyResult.ToString()!);
    }
}

