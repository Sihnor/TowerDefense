using Code.Scripts.Enums;
using UnityEngine;
using UnityEngine.Rendering;

namespace Code.Scripts
{
    public enum ENodeState
    {
        Open, // Noch nicht besucht
        Closed, // Besucht
        Blocked, // Blockiert
    }
    
    public class Node
    {
        public ENodeState TileType;
        public Vector2Int Position;
        
        private float gCost = 1000000;
        private float hCost = 1000000;
        private float Weight = 1;
        private Node Parent;
        
        public float GetWeight()
        {
            return this.Weight;
        }
        
        public void SetWeight(float weight)
        {
            this.Weight = weight;
        }
        
        public void IncreaseWeight(float weight)
        {
            this.Weight += weight;
        }
        
        public Node GetParent()
        {
            return this.Parent;
        }
        
        public void SetParent(Node parent)
        {
            this.Parent = parent;
        }

        public Node(ENodeState tileType, Vector2Int position)
        {
            this.TileType = tileType;
            this.Position = position;
        }
        
        public void SetTileType(ENodeState tileType)
        {
            this.TileType = tileType;
        }
        
        public float GetGCost()
        {
            return this.gCost;
        }
        
        public void SetGCost(float gCost)
        {
            this.gCost = gCost;
        }
        
        public float GetHCost()
        {
            return this.hCost;
        }
        
        public void SetHCost(float hCost)
        {
            this.hCost = hCost;
        }
        
        public float GetFCost()
        {
            return GetGCost() + this.hCost;
        }
    }
}