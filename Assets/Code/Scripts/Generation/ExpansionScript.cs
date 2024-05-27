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

        public event Action<Vector2Int, EDirection, int> OnExpansion;

        public void InitExpansion(Quadrant quadrant, Vector2Int position, EDirection direction)
        {
            this.Quadrant = quadrant;
            this.Position = position;
            this.Direction = direction;
        }

        public void OnMouseUp()
        {
            int roadTile = this.Quadrant.GetStartRoadTile();

            OnExpansion?.Invoke(this.Position, this.Direction, roadTile);

            // Remove the expansion and destroy the game object
            this.Quadrant.RemoveExpansion(this);
            Destroy(this.GameObject());
        }
    }
}