using Expression.Parser;

namespace Expression.Interface;

public interface INodeModel
{ 
    int Counter { get; } 
    int Power { get; } 
    bool WithVariable { get; }

    TryResults<INodeModel> TryAddModel(INodeModel model);
    public INodeModel MultiplyModel(INodeModel model);
}