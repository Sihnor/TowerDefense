using System;
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
    
    public class Node : MonoBehaviour
    {
        public ENodeState TileType;
        public Vector2Int Position;

        [SerializeField] private float CostToEnter = 1;
        [SerializeField] private float gCost = 2; // Gibt an, wie weit der Startknoten von diesem Knoten entfernt ist
        [SerializeField] private float hCost = 2; // Gibt an, wie weit der Zielknoten von diesem Knoten entfernt ist
        [SerializeField] private float fCost; // Gibt an, wie weit der Startknoten von diesem Knoten entfernt ist
        [SerializeField] private float Weight = 1;
        [SerializeField] private Node Parent;


        private void Update()
        {
            this.fCost = GetFCost();
        }

        public void SetNode(ENodeState tileType, Vector2Int position)
        {
            this.TileType = tileType;
            this.Position = position;
        }
        
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

            this.CostToEnter = GetGCost() * this.Weight;
        }
        
        public float GetCostToEnter()
        {
            return this.CostToEnter;
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
            return GetGCost() + this.hCost;
        }
    }
}