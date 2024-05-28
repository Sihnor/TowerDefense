using System;
using System.Collections.Generic;
using Code.Scripts.Enums;
using Code.Scripts.Generation;
using Code.Scripts.ScriptableScripts;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Factory
{
    [UnityEngine.CreateAssetMenu(fileName = "QuadrantFactory", menuName = "ScriptableObject/Factory/QuadrantFactory", order = 0)]
    public class QuadrantFactory : ScriptableObject
    {
        [SerializeField] private LevelSettings LevelSettings;
        
        [SerializeField] private GameObject TilePrefab;
        [SerializeField] private GameObject QuadrantPrefab;
        
        [SerializeField] private EDirection RESTRICTED_DIRECTION = EDirection.South;
        
        /// <summary>
        /// Generate a single exit quadrant
        /// </summary>
        /// <param name="startPosition">Start Position of the new Quadrant</param>
        /// <param name="startDirection">Start Direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Tile of the new Quadrant</param>
        public GameObject GenerateSingleExitQuadrant(Vector2Int startPosition, EDirection startDirection, int previousEndRoadTile)
        {
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection) });
            int endRoadTile = Random.Range(0, 15);

            List<GameObject> tiles = GenerateQuadrant(startPosition);
            GameObject quadrant = InstantiateQuadrant(startPosition, startDirection, previousEndRoadTile, new List<EDirection> {targetDirection}, new List<int> {endRoadTile}, tiles);
            
            CombineTileWithQuadrant(quadrant, tiles);
            
            return quadrant;
        }

        /// <summary>
        /// Generate a double exit quadrant
        /// </summary>
        /// <param name="startPosition">Start Position of the new Quadrant</param>
        /// <param name="startDirection">Start Direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Tile of the new Quadrant</param>
        public GameObject GenerateDoubleExitQuadrant(Vector2Int startPosition, EDirection startDirection, int previousEndRoadTile)
        {
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> {InvertDirection(startDirection)});
            int endRoadTile = Random.Range(0, 15);

            EDirection targetDirection2 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection });
            int endRoadTile2 = Random.Range(0, 15);
            
            List<GameObject> tiles = GenerateQuadrant(startPosition);
            GameObject quadrant = InstantiateQuadrant(startPosition, startDirection, previousEndRoadTile, new List<EDirection> {targetDirection, targetDirection2}, new List<int> {endRoadTile}, tiles);
            
            CombineTileWithQuadrant(quadrant, tiles);
            
            return quadrant;
        }

        /// <summary>
        /// Generate a triple exit quadrant
        /// </summary>
        /// <param name="startPosition">Start Position of the new Quadrant</param>
        /// <param name="startDirection">Start Direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Tile of the new Quadrant</param>
        public GameObject GenerateTripleExitQuadrant(Vector2Int startPosition, EDirection startDirection, int previousEndRoadTile)
        {
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection) });
            int endRoadTile = Random.Range(0, 15);

            EDirection targetDirection2 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection });
            int endRoadTile2 = Random.Range(0, 15);

            EDirection targetDirection3 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection, targetDirection2 });
            int endRoadTile3 = Random.Range(0, 15);
            
            List<GameObject> tiles = GenerateQuadrant(startPosition);
            GameObject quadrant = InstantiateQuadrant(startPosition, startDirection, previousEndRoadTile, new List<EDirection> {targetDirection, targetDirection2, targetDirection3}, new List<int> {endRoadTile}, tiles);
            
            CombineTileWithQuadrant(quadrant, tiles);

            return quadrant;
        }

        /// <summary>
        /// Invert the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private EDirection InvertDirection(EDirection direction)
        {
            return direction switch
            {
                EDirection.North => EDirection.South,
                EDirection.East => EDirection.West,
                EDirection.South => EDirection.North,
                EDirection.West => EDirection.East,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        /// <summary>
        /// Randomize the exit direction
        /// </summary>
        /// <param name="excludeDirection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private EDirection RandomizeExitDirection(ICollection<EDirection> excludeDirection)
        {
            EDirection direction = Random.Range(0, 4) switch
            {
                0 => EDirection.North,
                1 => EDirection.East,
                2 => EDirection.South,
                3 => EDirection.West,
                _ => throw new ArgumentOutOfRangeException()
            };

            while (excludeDirection.Contains(direction) || direction == this.RESTRICTED_DIRECTION)
            {
                direction = Random.Range(0, 4) switch
                {
                    0 => EDirection.North,
                    1 => EDirection.East,
                    2 => EDirection.South,
                    3 => EDirection.West,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return direction;
        }
        
        /// <summary>
        /// Will create the tiles of the Quadrant
        /// </summary>
        /// <param name="startPosition">Start Position of the Tiles</param>
        private List<GameObject> GenerateQuadrant(Vector2Int startPosition)
        {
            List<GameObject> tiles = new List<GameObject>();
            float quadrantSize = this.LevelSettings.GetQuadrantSize();

            for (int i = 0; i < quadrantSize; i++)
            {
                for (int j = 0; j < quadrantSize; j++)
                {
                    Vector2Int position = new Vector2Int(startPosition.x + i, startPosition.y + j);
                    GameObject tile = Instantiate(this.TilePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
                    tiles.Add(tile);
                }
            }
            
            return tiles;
        }
        
        /// <summary>
        /// Instantiate the Quadrant
        /// </summary>
        /// <param name="position"></param>
        /// <param name="startDirection"></param>
        /// <param name="previousEndRoadTile"></param>
        /// <param name="targetDirection"></param>
        /// <param name="endRoadTile"></param>
        /// <param name="tiles"></param>
        /// <returns></returns>
        private GameObject InstantiateQuadrant(Vector2Int position, EDirection startDirection, int previousEndRoadTile, List<EDirection> targetDirection, List<int> endRoadTile, List<GameObject> tiles)
        {
            GameObject newQuadrant = Instantiate(this.QuadrantPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
            newQuadrant.GetComponent<Quadrant>().InitQuadrant(position, startDirection, previousEndRoadTile, targetDirection ,  endRoadTile, this.LevelSettings.GetQuadrantSize());
            
            return newQuadrant;
        }
        
        private void CombineTileWithQuadrant(GameObject quadrant, List<GameObject> tiles)
        {
            foreach (GameObject tile in tiles)
            {
                tile.transform.SetParent(quadrant.transform);
            }
        }
    }
}