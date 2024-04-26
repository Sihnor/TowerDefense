using System.Collections.Generic;
using Code.Scripts.Enums;
using UnityEngine;

namespace Code.Scripts
{
    public class Quadrant : MonoBehaviour
    {
        private EDirection StartDirection;
        private List<EDirection> TargetDirections = new List<EDirection>();
        
        private int StartRoadTile;
        private List<int> EndRoadTiles = new List<int>();

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
    }
}