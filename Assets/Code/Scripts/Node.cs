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
        
        private int gCost = 1000000;
        private int hCost = 1000000;
        private Node Parent;
  
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
        
        public int GetGCost()
        {
            return this.gCost;
        }
        
        public void SetGCost(int gCost)
        {
            this.gCost = gCost;
        }
        
        public int GetHCost()
        {
            return this.hCost;
        }
        
        public void SetHCost(int hCost)
        {
            this.hCost = hCost;
        }
        
        public int GetFCost()
        {
            return this.gCost + this.hCost;
        }
    }
}