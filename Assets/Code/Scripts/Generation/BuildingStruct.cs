using Code.Scripts.Enums;
using UnityEngine;
using UnityEngine.Rendering;

namespace Code.Scripts.Generation
{
    public struct BuildingStruct : INode
    {
        private ENodeState NodeState;
        
        private int X;
        private int Y;
        
        private float CostToEnter;
        private float gCost;
        private float hCost;
        private float fCost;
        private float Width;
        private float Weight;


       
        public void SetPosition(Vector2Int position)
        {
            this.X = position.x;
            this.Y = position.y;
        }
   
        public void IncreaseWeight(float amount)
        {
            this.Weight += amount;
        }

        public ENodeState TileType { get; set; }

        public INode Parent { get; private set; }

        public void SetNode(ENodeState tileType, INode parent)
        {
            this.NodeState = tileType;
            this.Parent = parent;
        }

        public void SetTileType(ENodeState tileType)
        {
            this.NodeState = tileType;
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
            return this.NodeState;
        }

        public void SetWeight(float weight)
        {
            this.Weight = weight;
        }

        public void SetGCost(float gCost)
        {
            this.gCost = gCost;
        }

        public float GetGCost()
        {
            return this.gCost;
        }

        public void SetHCost(float hCost)
        {
            this.hCost = hCost;
        }

        public float GetHCost()
        {
            return this.hCost;
        }

        public void SetFCost(float fCost)
        {
            this.fCost = fCost;
        }

        public float GetFCost()
        {
            return this.fCost;
        }

        public int GetX()
        {
            return this.X;
        }

        public int GetY()
        {
            return this.Y;
        }

        public Vector2Int GetPosition()
        {
            return new Vector2Int(this.X, this.Y);
        }

        public float GetCostToEnter()
        {
            this.CostToEnter = this.gCost * this.Weight;
            return this.CostToEnter;
        }

        public float GetWeight()
        {
            return this.Weight;
        }
    }

}