using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Enums;
using Code.Scripts.Generation;
using Code.Scripts.ScriptableScripts;
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
        /// Generate the start quadrant of the world
        /// </summary>
        /// <param name="startPosition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GameObject GenerateStartQuadrant(Vector2Int startPosition)
        {
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { this.RESTRICTED_DIRECTION });
            int endRoadTile = this.LevelSettings.GetQuadrantSize() / 2;

            Vector2Int endPoint = targetDirection switch
            {
                EDirection.North => new Vector2Int(endRoadTile, this.LevelSettings.GetQuadrantSize() - 1),
                EDirection.South => new Vector2Int(endRoadTile, 0),
                EDirection.West => new Vector2Int(0, endRoadTile),
                EDirection.East => new Vector2Int(this.LevelSettings.GetQuadrantSize() - 1, endRoadTile),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Vector2Int startPoint = targetDirection switch
            {
                EDirection.North => new Vector2Int(endRoadTile, endRoadTile + 2),
                EDirection.South => new Vector2Int(endRoadTile, endRoadTile - 2),
                EDirection.West => new Vector2Int(endRoadTile - 2, endRoadTile),
                EDirection.East => new Vector2Int(endRoadTile + 2, endRoadTile),
                _ => throw new ArgumentOutOfRangeException()
            };

            List<List<GameObject>> tiles = GenerateQuadrant(startPosition);
            GameObject quadrant = InstantiateQuadrant(startPosition, EDirection.North, 0, new List<EDirection> { targetDirection }, new List<int> { 0 }, tiles);

            CombineTileWithQuadrant(quadrant, tiles);
            
            AStarAlgorithm.CreateFirstPath(tiles, this.LevelSettings.GetQuadrantSize(), startPoint, endPoint);
            
            return quadrant;
        }

        /// <summary>
        /// Generate a single exit quadrant
        /// </summary>
        /// <param name="startPosition">Start Position of the new Quadrant</param>
        /// <param name="startDirection">Start Direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Tile of the new Quadrant</param>
        public GameObject GenerateSingleExitQuadrant(Vector2Int startPosition, EDirection startDirection, int previousEndRoadTile)
        {
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection) });
            int endRoadTile = Random.Range(1, this.LevelSettings.GetQuadrantSize());
            Vector2Int endPoint = CalculateEndPoint(targetDirection, endRoadTile);

            List<List<GameObject>> tiles = GenerateQuadrant(startPosition);
            GameObject quadrant = InstantiateQuadrant(startPosition, startDirection, previousEndRoadTile, new List<EDirection> { targetDirection }, new List<int> { endRoadTile }, tiles);

            CombineTileWithQuadrant(quadrant, tiles);
            
            //AStarAlgorithm.CreateFirstPath(tiles, this.LevelSettings.GetQuadrantSize(), startPosition, endPoint);
            
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
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection) });
            int endRoadTile = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            EDirection targetDirection2 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection });
            int endRoadTile2 = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            List<List<GameObject>> tiles = GenerateQuadrant(startPosition);
            GameObject quadrant = InstantiateQuadrant(startPosition, startDirection, previousEndRoadTile, new List<EDirection> { targetDirection, targetDirection2 }, new List<int> { endRoadTile },
                tiles);

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
            int endRoadTile = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            EDirection targetDirection2 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection });
            int endRoadTile2 = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            EDirection targetDirection3 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection, targetDirection2 });
            int endRoadTile3 = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            List<List<GameObject>> tiles = GenerateQuadrant(startPosition);
            GameObject quadrant = InstantiateQuadrant(startPosition, startDirection, previousEndRoadTile, new List<EDirection> { targetDirection, targetDirection2, targetDirection3 },
                new List<int> { endRoadTile }, tiles);

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
        private List<List<GameObject>> GenerateQuadrant(Vector2Int startPosition)
        {
            List<List<GameObject>> tileList = new List<List<GameObject>>();
            float quadrantSize = this.LevelSettings.GetQuadrantSize();

            for (int i = 0; i < quadrantSize; i++)
            {
                tileList.Add(new List<GameObject>());

                for (int j = 0; j < quadrantSize; j++)
                {
                    Vector2Int position = new Vector2Int(startPosition.x + i, startPosition.y + j);
                    GameObject tile = Instantiate(this.TilePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
                    tile.GetComponent<Node>().SetNode(ENodeState.Open, new Vector2Int(i, j));
                    tileList[i].Add(tile);
                }
            }

            return tileList;
        }

        /// <summary>
        /// Instantiate the Quadrant
        /// </summary>
        /// <param name="position"></param>
        /// <param name="startDirection">Start direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Road of the new Quadrant</param>
        /// <param name="targetDirection">Target direction of the new Quadrant</param>
        /// <param name="endRoadTile">End Road of the new Quadrant</param>
        /// <param name="tiles"></param>
        /// <returns></returns>
        private GameObject InstantiateQuadrant(Vector2Int position, EDirection startDirection, int previousEndRoadTile, List<EDirection> targetDirection, List<int> endRoadTile, List<List<GameObject>> 
                tiles)
        {
            GameObject newQuadrant = Instantiate(this.QuadrantPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
            newQuadrant.GetComponent<Quadrant>().InitQuadrant(position, startDirection, previousEndRoadTile, targetDirection, endRoadTile, this.LevelSettings.GetQuadrantSize());

            return newQuadrant;
        }

        /// <summary>
        /// To combine the tiles with the Quadrant so the hierarchy is sorted
        /// </summary>
        /// <param name="quadrant"></param>
        /// <param name="tileMatrix"></param>
        private void CombineTileWithQuadrant(GameObject quadrant, List<List<GameObject>> tileMatrix)
        {
            foreach (GameObject tile in tileMatrix.SelectMany(tileList => tileList))
            {
                tile.transform.SetParent(quadrant.transform);
            }
        }

        private Vector2Int CalculateEndPoint(EDirection targetDirection, int endRoadTile)
        {
            return targetDirection switch
            {
                EDirection.North => new Vector2Int(endRoadTile, 0),
                EDirection.South => new Vector2Int(endRoadTile, this.LevelSettings.GetQuadrantSize() - 1),
                EDirection.West => new Vector2Int(0, endRoadTile),
                EDirection.East => new Vector2Int(this.LevelSettings.GetQuadrantSize() - 1, endRoadTile),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

    }
}