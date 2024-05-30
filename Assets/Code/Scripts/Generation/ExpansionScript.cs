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

        public event Action<Vector2Int, EDirection, int> OnExpansion;

        public void InitExpansion(Quadrant quadrant, Vector2Int position, EDirection direction, int roadTile)
        {
            this.Quadrant = quadrant;
            this.Position = position;
            this.Direction = direction;
            this.RoadTile = roadTile;
        }

        public void OnMouseUp()
        {
            OnExpansion?.Invoke(this.Position, this.Direction, this.RoadTile);

            // Remove the expansion and destroy the game object
            this.Quadrant.RemoveExpansion(this);
            Destroy(this.GameObject());
        }
    }
}