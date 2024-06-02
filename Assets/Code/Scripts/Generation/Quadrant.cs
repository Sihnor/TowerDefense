using System;
using System.Collections.Generic;
using Code.Scripts.Enums;
using UnityEngine;

namespace Code.Scripts.Generation
{
    public class Quadrant : MonoBehaviour
    {
        [SerializeField] private GameObject ExpansionPrefab;

        private Vector2Int WorldPosition;
        private EDirection StartDirection;
        private int StartRoadTile;

        private List<EDirection> TargetDirections = new List<EDirection>();
        private List<int> EndRoadTiles = new List<int>();
        private readonly List<ExpansionScript> Expansions = new List<ExpansionScript>();


        public void InitQuadrant(Vector2Int worldPosition, EDirection startDirection, int startRoadTile, List<EDirection> targetDirections, List<int> endRoadTiles, int quadrantSize, BuildingNode lastBuildingNode)
        {
            this.WorldPosition = worldPosition;
            this.StartDirection = startDirection;
            this.StartRoadTile = startRoadTile;
            this.TargetDirections = targetDirections;
            this.EndRoadTiles = endRoadTiles;

            CreateExpansionObjects(quadrantSize, lastBuildingNode);
        }

        /// <summary>
        /// Create the events for the expansions of the quadrant for the WorldGenerator
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void CreateExpansionObjects(int quadrantSize, BuildingNode lastBuildingNode)
        {
            for (int i = 0; i < this.TargetDirections.Count; i++)
            {
                #region Expansion Object Creation and Positioning

                const int offset = 2;

                Vector2Int objectPosition = this.WorldPosition;
                objectPosition += this.TargetDirections[i] switch
                {
                    EDirection.North => new Vector2Int(quadrantSize / 2, quadrantSize + offset),
                    EDirection.East => new Vector2Int(quadrantSize + offset, quadrantSize / 2),
                    EDirection.South => new Vector2Int(quadrantSize / 2, -offset),
                    EDirection.West => new Vector2Int(-offset, quadrantSize / 2),
                    _ => throw new ArgumentOutOfRangeException()
                };

                GameObject expansion = Instantiate(this.ExpansionPrefab, new Vector3(objectPosition.x, 0, objectPosition.y), Quaternion.identity);

                #endregion

                #region Next Quadrant Positioning

                Vector2Int newQuadrantPosition = this.WorldPosition;
                newQuadrantPosition += this.TargetDirections[i] switch
                {
                    EDirection.North => new Vector2Int(0, quadrantSize),
                    EDirection.East => new Vector2Int(quadrantSize, 0),
                    EDirection.South => new Vector2Int(0, -quadrantSize),
                    EDirection.West => new Vector2Int(-quadrantSize, 0),
                    _ => throw new ArgumentOutOfRangeException()
                };

                ExpansionScript expansionScript = expansion.GetComponent<ExpansionScript>();
                expansionScript.InitExpansion(this, newQuadrantPosition, this.TargetDirections[i], this.EndRoadTiles[i], lastBuildingNode);
                this.Expansions.Add(expansionScript);

                #endregion
            }
        }

        /// <summary>
        /// Function for the WorldGenerator to get the expansions of the Quadrant
        /// </summary>
        /// <returns></returns>
        public List<ExpansionScript> GetExpansions()
        {
            return this.Expansions;
        }

        /// <summary>
        /// To remove the expansion from the Quadrant
        /// </summary>
        /// <param name="expansionScript"></param>
        public void RemoveExpansion(ExpansionScript expansionScript)
        {
            this.Expansions.Remove(expansionScript);
        }

        public Vector2Int GetStartTile()
        {
            return this.WorldPosition;
        }

        public EDirection GetStartDirection()
        {
            return this.StartDirection;
        }

        public int GetStartRoadTile()
        {
            return this.StartRoadTile;
        }

        public List<EDirection> GetTargetDirections()
        {
            return this.TargetDirections;
        }

        public List<int> GetEndRoadTiles()
        {
            return this.EndRoadTiles;
        }


    }
}