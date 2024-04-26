using System;
using System.Collections.Generic;
using Code.Scripts.Enums;
using UnityEngine;

namespace Code.Scripts
{
    public class WorldGenerator : MonoBehaviour
    {
        private List<Quadrant> EndTiles = new List<Quadrant>();
        [SerializeField] private GameObject QuadrantGenerator;

        private void Start()
        {
            InitWorld();
        }

        private void InitWorld()
        {
            
        }

        private void SubscribeFromExpansion(Quadrant finalQuadrant)
        {
            
        }

        private void RemoveFinalizedQuadrant()
        {
            
        }

        private void UnsubscribeFromExpansion()
        {
            
        }
    }
}
