using Code.Scripts.Enums;
using Code.Scripts.Generation;
using UnityEngine;

namespace Code.Scripts
{
    public class BuildingNode : MonoBehaviour, INode
    {
        public ENodeState TileType { get; private set; }

        public Vector2Int Position { get; private set; }

        [SerializeField] private float CostToEnter = 1;
        [SerializeField] private float gCost = 2; // Gibt an, wie weit der Startknoten von diesem Knoten entfernt ist
        [SerializeField] private float hCost = 2; // Gibt an, wie weit der Zielknoten von diesem Knoten entfernt ist
        [SerializeField] private float fCost; // Gibt an, wie weit der Startknoten von diesem Knoten entfernt ist
        [SerializeField] private float Weight = 1;

        public INode Parent { get; private set; }


        #region INode Implementation
        
        public void SetNode(ENodeState tileType, Vector2Int position)
        {
            this.TileType = tileType;
            this.Position = position;
        }
        
        public INode GetParent()
        {
            return this.Parent;
        }

        public ENodeState GetTileType()
        {
            return this.TileType;
        }
        
        public void SetParent(INode parent)
        {
            this.Parent = parent;
        }
        
        
        public void SetNode(ENodeState tileType, INode parent)
        {
            this.TileType = tileType;
            this.Parent = parent as BuildingNode;
        }

        #endregion

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

            this.CostToEnter = this.gCost * this.Weight;
        }
        
        public float GetCostToEnter()
        {
            return this.CostToEnter;
        }
        
        public void SetParent(BuildingNode parent)
        {
            this.Parent = parent;
        }

        public BuildingNode(ENodeState tileType, Vector2Int position)
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
            return this.gCost + this.Weight;
        }
        
        public void SetGCost(float gCost)
        {
            this.gCost = gCost;
        }
        
        public void IncreaseGCost(float gCost)
        {
            this.gCost += gCost;
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
            return this.gCost + this.hCost;
        }
    }
}