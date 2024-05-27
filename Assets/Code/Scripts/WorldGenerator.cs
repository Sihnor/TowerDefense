using System;
using System.Collections.Generic;
using Code.Scripts.Enums;
using Code.Scripts.ScriptableScripts;
using UnityEngine;

namespace Code.Scripts
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject QuadrantGenerator;
        
        [SerializeField] private GameObject StartQuadrant;
        
        [SerializeField] private LevelSettings LevelSettings;
        
        private void Awake()
        {
            this.LevelSettings.SetQuadrantSize(15);   
        }

        private void Start()
        {
            InitWorld();
        }

        private void InitWorld()
        {
            this.QuadrantGenerator.GetComponent<QuadrantGenerator>().GenerateStartQuadrant();
        }
        
        private void CreateStartQuadrant()
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
