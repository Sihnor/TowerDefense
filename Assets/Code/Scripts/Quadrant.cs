using System.Collections.Generic;
using Code.Scripts.Enums;
using UnityEngine;

namespace Code.Scripts
{
    public class Quadrant : MonoBehaviour
    {
        private EDirection StartDirection;
        private int StartRoadTile;
        
        private List<EDirection> TargetDirections = new List<EDirection>();
        private List<int> EndRoadTiles = new List<int>();
        
        public EDirection GetStartDirection()
        {
            throw new System.NotImplementedException();
        }
        
        public Vector2Int GetStartTile()
        {
            throw new System.NotImplementedException();
        }

        private void ShowExpandDirection()
        {
            throw new System.NotImplementedException();
        }
        
        private void OnExpandDirection()
        {
            throw new System.NotImplementedException();
        }
        
        public bool IsFinalized()
        {
            throw new System.NotImplementedException();
        }
        
        public EDirection GetTargetDirection()
        {
            throw new System.NotImplementedException();
        }
        
        public int GetEndRoadTile()
        {
            throw new System.NotImplementedException();
        }
    }
}