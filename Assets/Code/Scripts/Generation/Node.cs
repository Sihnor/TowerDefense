using Code.Scripts.Enums;
using UnityEngine;

namespace Code.Scripts.Generation
{
    public class Node : MonoBehaviour, INode, IWorldProperties
    {
        public float Height { get; private set; } = 0;

        public Vector2Int Position { get; private set; } = Vector2Int.zero;

        public ENodeState TileType { get; private set; } = ENodeState.Open;

        public INode Parent { get; private set; } = null;

        #region INodes Implementation

        public void SetNode(ENodeState tileType, INode parent)
        {
            this.TileType = tileType;
            this.Parent = parent;
        }

        public void SetTileType(ENodeState tileType)
        {
            this.TileType = tileType;
        }

        public void SetParent(INode parent)
        {
            this.Parent = parent;
        }

        public INode GetParent()
        {
            return this.Parent;
        }

        public ENodeState GetTileType()
        {
            return this.TileType;
        }
        
        #endregion

        #region IWorldProperties Implementation

        public float GetHeight()
        {
            return this.Height;
        }
        
        public void SetHeight(float factor)
        {
            this.Height = factor;
        }
        
        public void AddHeight(float factor)
        {
            this.Height += factor;
        }
        
        public void MultiplyHeight(float factor)
        {
            this.Height *= factor;
        }
        
        public Vector2Int GetPosition()
        {
            return this.Position;
        }
        
        public void SetPosition(Vector2Int position)
        {
            this.Position = position;
        }
        
        #endregion
        
    }
}