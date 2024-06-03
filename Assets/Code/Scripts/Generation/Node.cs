using Code.Scripts.Enums;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts.Generation
{
    public class Node : MonoBehaviour, INode, IWorldProperties, IStackable
    {
        public float Height { get; private set; } = 0;

        public Vector2Int Coordinates { get; private set; } = Vector2Int.zero;

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


        public Vector2Int GetCoordinates()
        {
            return this.Coordinates;
        }

        public void SetPosition(Vector2Int position)
        {
            this.Coordinates = position;
        }

        #endregion

        #region IStackable Implementation

        public IStackable MeinPlace { get; set; }

        public Vector3 GetPosition()
        {
            if (this.MeinPlace == null)
            {
                return this.transform.position;
            }
            if (this.MeinPlace.MeinPlace == null)
            {
                return this.MeinPlace.GetPosition();
            }
            if (this.MeinPlace.MeinPlace.MeinPlace == null)
            {
                return this.MeinPlace.MeinPlace.GetPosition();
            }
            
            return Vector3.zero;
        }

        #endregion
    }
}
