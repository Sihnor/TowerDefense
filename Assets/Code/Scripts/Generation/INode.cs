using Code.Scripts.Enums;

namespace Code.Scripts.Generation
{
    public interface INode
    {
        ENodeState TileType { get; }

        INode Parent { get; }
        
        public void SetNode(ENodeState tileType, INode parent);

        public void SetTileType(ENodeState tileType);

        public void SetParent(INode parent);

        public INode GetParent();
        
        public ENodeState GetTileType();
    }
}