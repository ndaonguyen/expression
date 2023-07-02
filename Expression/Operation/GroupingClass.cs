using System.Text;
using Expression.Model;

namespace Expression.Operation;

public class GroupingClass
{
    private readonly Dictionary<string, ElementNodeAnalysis.NodeModel> _result = new();

    public void AddNode(ElementNode node)
    {
        var analyzeNode = ElementNodeAnalysis.AnalyzeNode(node);
        var nodeKey = analyzeNode.NodeKey();

        if (_result.TryGetValue(nodeKey, out var nodeModel))
        {
            if (!nodeModel.WithVariable)
            {
                var a = 0;
            }
            nodeModel.Counter += analyzeNode.Counter;
        }
        else
        {
            _result.Add(nodeKey, analyzeNode);
        }
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        var re = _result.Values.OrderByDescending(x => x.Power).ToList();
        var allNodes = _result.Values.OrderByDescending(x => x.Power).ThenByDescending(x => x.WithVariable).ToList();
        foreach (var node in allNodes)
        {
            stringBuilder.Append(node);
            stringBuilder.Append("+");
        }

        stringBuilder.Length--; // Remove the last '+'
        return stringBuilder.ToString();
    }

}