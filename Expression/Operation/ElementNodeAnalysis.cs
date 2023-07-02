using System.Runtime.CompilerServices;
using Expression.Model;

[assembly: InternalsVisibleTo("Expression.Tests")]
namespace Expression.Operation;

public static class ElementNodeAnalysis
{
    /// <summary>
    /// Our elementNode in this context can be x, 2*x or x^2
    /// </summary>
    internal static NodeModel AnalyzeNode(ElementNode node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        var nodeModel = new NodeModel();
        var valueStr = node.Value;
        
        if (valueStr.Contains('*') || valueStr.Contains('^'))
        {
            // we only have x, 2*x, or 3*x or <number>*x or 2*x^3
            var components = valueStr.Split('*');
            if (components.Length > 2) throw new InvalidOperationException($"Wrong format for node : {node.Value}");

            if (components.Length == 2)
            {
                if (!components[1].Contains(Evaluation.VARIABLE)) throw new InvalidOperationException($"Wrong format for node : {node.Value}");

                nodeModel.Counter = int.Parse(components[0]);
                nodeModel.WithVariable = true;
            }

            // we can have x^2, or x^3 or 2*x^3
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
       
        // If the node length is 1 => either variable or number only
        // no *, no ^
        if (valueStr == Evaluation.VARIABLE)
        {
            nodeModel.WithVariable = true;
        }
        else
        {
            nodeModel.Counter = int.Parse(valueStr);
        }

        return nodeModel;
    }

    internal class NodeModel
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

        public NodeModel MultiplyModel(NodeModel model)
        {
            int resultPower;
            if (WithVariable && model.WithVariable)
            {
                resultPower = Power + model.Power;
            } 
            else if (!WithVariable && !model.WithVariable)
            {
                resultPower = 1;
            }
            else
            {
                resultPower = Power * model.Power; 
            }

            return new NodeModel
            {
                Counter = Counter * model.Counter,
                Power = resultPower,
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

        public string NodeKey()
        {
            const string NO_VARIABLE_KEY = "NO_VARIABLE";
            return WithVariable == false
                ? NO_VARIABLE_KEY
                : (Power == 1 ? Evaluation.VARIABLE : $"{Evaluation.VARIABLE}^{Power}");
        }
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

