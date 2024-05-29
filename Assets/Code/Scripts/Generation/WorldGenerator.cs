using System;
using Code.Scripts.Enums;
using Code.Scripts.Factory;
using Code.Scripts.ScriptableScripts;
using UnityEngine;

namespace Code.Scripts.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private QuadrantFactory QuadrantFactory;
        
        private void Start()
        {
            InitWorld();
        }

        private void InitWorld()
        {
            var temp = this.QuadrantFactory.GenerateStartQuadrant(new Vector2Int(0, 0));
            //var temp = this.QuadrantFactory.GenerateSingleExitQuadrant(new Vector2Int(0, 0), EDirection.North, 0);
            //var temp = this.QuadrantFactory.GenerateDoubleExitQuadrant(new Vector2Int(0, 0), EDirection.North, 0);
            //var temp = this.QuadrantFactory.GenerateTripleExitQuadrant(new Vector2Int(0, 0), EDirection.North, 0);

            foreach (ExpansionScript expansionScript in temp.GetComponent<Quadrant>().GetExpansions())
            {
                expansionScript.OnExpansion += OnExpansion;
            } 
        }

        private void OnExpansion(Vector2Int position, EDirection direction, int roadTile)
        {
            var temp = this.QuadrantFactory.GenerateSingleExitQuadrant(position, direction, roadTile);
            
            foreach (ExpansionScript expansionScript in temp.GetComponent<Quadrant>().GetExpansions())
            {
                expansionScript.OnExpansion += OnExpansion;
            }
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
