using System;
using Code.Scripts.Enums;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts.Generation
{
    public class ExpansionScript : MonoBehaviour
    {
        private Quadrant Quadrant;
        private Vector2Int Position;
        private EDirection Direction;
        private int RoadTile;
        private BuildingNode LastBuildingNode;
        private BuildingStruct NewLastBuildingNode;

        public event Action<Vector2Int, EDirection, int, BuildingNode> OnExpansion;

        public void InitExpansion(Quadrant quadrant, Vector2Int position, EDirection direction, int roadTile, BuildingNode lastBuildingNode)
        {
            this.Quadrant = quadrant;
            this.Position = position;
            this.Direction = direction;
            this.RoadTile = roadTile;
            this.LastBuildingNode = lastBuildingNode;
        }
        
        public void InitExpansion(Quadrant quadrant, Vector2Int position, EDirection direction, int roadTile, BuildingStruct lastBuildingNode)
        {
            this.Quadrant = quadrant;
            this.Position = position;
            this.Direction = direction;
            this.RoadTile = roadTile;
            this.NewLastBuildingNode = lastBuildingNode;
        }

        public void OnMouseUp()
        {
            OnExpansion?.Invoke(this.Position, this.Direction, this.RoadTile, this.LastBuildingNode);

            // Remove the expansion and destroy the game object
            this.Quadrant.RemoveExpansion(this);
            Destroy(this.GameObject());
        }
    }
}