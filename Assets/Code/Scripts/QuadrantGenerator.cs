using System;
using Code.Scripts.Enums;
using UnityEngine;

namespace Code.Scripts
{
    public class QuadrantGenerator : MonoBehaviour
    {
        [Header("RoadTiles")]
        [SerializeField] private GameObject RoadTile;
        [SerializeField] private GameObject RoadTileCorner;
        [SerializeField] private GameObject RoadTileT;
        [SerializeField] private GameObject RoadTileCross;
        
        
        [SerializeField] private GameObject FieldTile;
        [SerializeField] private GameObject MountainTile;
        [SerializeField] private GameObject ForestTile;
        [SerializeField] private GameObject WaterTile;
        
        private int QuadrantSize = 10;

        private void Start()
        {
            SpawnQuadrant(Vector3.zero, EDirection.North, 1);
        }

        public void SpawnQuadrant(Vector3 position, EDirection source, int start)
        {
            for (int i = 0; i < this.QuadrantSize; i++)
            {
                for (int j = 0; j < this.QuadrantSize; j++)
                {
                    Vector3 spawnPosition = position + new Vector3(i, 0, j);
                    Instantiate(this.FieldTile, spawnPosition, Quaternion.identity);
                }
            }    
        }

        private void GenerateFieldMap()
        {

        }
    }
}