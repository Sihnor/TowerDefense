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
    [CreateAssetMenu(fileName = "QuadrantFactory", menuName = "ScriptableObject/Factory/QuadrantFactory", order = 0)]
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

            Vector2Int startPoint = targetDirection switch
            {
                EDirection.North => new Vector2Int(endRoadTile, endRoadTile + 2),
                EDirection.South => new Vector2Int(endRoadTile, endRoadTile - 2),
                EDirection.West => new Vector2Int(endRoadTile - 2, endRoadTile),
                EDirection.East => new Vector2Int(endRoadTile + 2, endRoadTile),
                _ => throw new ArgumentOutOfRangeException()
            };

            Vector2Int endPoint = targetDirection switch
            {
                EDirection.North => new Vector2Int(endRoadTile, this.LevelSettings.GetQuadrantSize() - 1),
                EDirection.South => new Vector2Int(endRoadTile, 0),
                EDirection.West => new Vector2Int(0, endRoadTile),
                EDirection.East => new Vector2Int(this.LevelSettings.GetQuadrantSize() - 1, endRoadTile),
                _ => throw new ArgumentOutOfRangeException()
            };

            // Create the tiles of the quadrant
            List<List<GameObject>> tiles = GenerateQuadrant(startPosition);

            // Create path
            var roadTilesPosition = AStarAlgorithm.CreateFirstPath(tiles, this.LevelSettings.GetQuadrantSize(), startPoint, endPoint);

            Vector2Int lastRoadTile = roadTilesPosition.Last();

            // Instantiate the quadrant
            GameObject quadrant = InstantiateQuadrant(
                startPosition, EDirection.North, 0, new List<EDirection> { targetDirection },
                new List<int> { endRoadTile }, tiles[lastRoadTile.x][lastRoadTile.y].GetComponent<BuildingNode>());
            
            CleanUpTiles(tiles, roadTilesPosition);
            
            CombineTileWithQuadrant(quadrant, tiles);

            foreach (var roadTile in roadTilesPosition)
            {
                tiles[roadTile.x][roadTile.y].GetComponent<Renderer>().material.color = Color.black;
            }

            return quadrant;
        }

        /// <summary>
        /// Generate a single exit quadrant
        /// </summary>
        /// <param name="worldPosition">Start Position of the new Quadrant</param>
        /// <param name="previousEndDirection">Start Direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Tile of the new Quadrant</param>
        /// <param name="lastBuildingNode">Last Note of the previous Quadrant</param>
        public GameObject GenerateSingleExitQuadrant(Vector2Int worldPosition, EDirection previousEndDirection, int previousEndRoadTile, BuildingNode lastBuildingNode)
        {
            EDirection startDirection = InvertDirection(previousEndDirection);
            int startRoadTile = previousEndRoadTile;
            Vector2Int startPoint = CalculateQuadrantPoint(startDirection, startRoadTile);

            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { startDirection });
            int endRoadTile = Random.Range(1, this.LevelSettings.GetQuadrantSize());
            Vector2Int endPoint = CalculateQuadrantPoint(targetDirection, endRoadTile);

            List<List<GameObject>> tiles = GenerateQuadrant(worldPosition);

            List<Vector2Int> roadPath = AStarAlgorithm.CreatePath(tiles, this.LevelSettings.GetQuadrantSize(), startPoint, endPoint);

            // Set the parent of the fist node from the last Node of the previous Quadrant
            tiles[roadPath.First().x][roadPath.First().y].GetComponent<BuildingNode>().SetParent(lastBuildingNode);

            Vector2Int lastRoadTile = roadPath.Last();

            GameObject quadrant = InstantiateQuadrant(
                worldPosition, startDirection, startRoadTile, new List<EDirection> { targetDirection },
                new List<int> { endRoadTile }, tiles[lastRoadTile.x][lastRoadTile.y].GetComponent<BuildingNode>());

            CleanUpTiles(tiles, roadPath);
            
            CombineTileWithQuadrant(quadrant, tiles);

            // make all road black
            foreach (var roadTile in roadPath)
            {
                tiles[roadTile.x][roadTile.y].GetComponent<Renderer>().material.color = Color.black;
            }

            return quadrant;
        }

        /// <summary>
        /// Generate a double exit quadrant
        /// </summary>
        /// <param name="worldPosition">Start Position of the new Quadrant</param>
        /// <param name="startDirection">Start Direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Tile of the new Quadrant</param>
        public GameObject GenerateDoubleExitQuadrant(Vector2Int worldPosition, EDirection startDirection, int previousEndRoadTile)
        {
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection) });
            int endRoadTile = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            EDirection targetDirection2 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection });
            int endRoadTile2 = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            List<List<GameObject>> tiles = GenerateQuadrant(worldPosition);
            GameObject quadrant = InstantiateQuadrant(worldPosition, startDirection, previousEndRoadTile, new List<EDirection> { targetDirection, targetDirection2 }, new List<int> { endRoadTile, endRoadTile2 }, 
                null);

            CombineTileWithQuadrant(quadrant, tiles);

            return quadrant;
        }

        /// <summary>
        /// Generate a triple exit quadrant
        /// </summary>
        /// <param name="worldPosition">Start Position of the new Quadrant</param>
        /// <param name="startDirection">Start Direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Tile of the new Quadrant</param>
        public GameObject GenerateTripleExitQuadrant(Vector2Int worldPosition, EDirection startDirection, int previousEndRoadTile)
        {
            EDirection targetDirection = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection) });
            int endRoadTile = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            EDirection targetDirection2 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection });
            int endRoadTile2 = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            EDirection targetDirection3 = RandomizeExitDirection(new List<EDirection> { InvertDirection(startDirection), targetDirection, targetDirection2 });
            int endRoadTile3 = Random.Range(1, this.LevelSettings.GetQuadrantSize());

            List<List<GameObject>> tiles = GenerateQuadrant(worldPosition);
            GameObject quadrant = InstantiateQuadrant(worldPosition, startDirection, previousEndRoadTile, new List<EDirection> { targetDirection, targetDirection2, targetDirection3 },
                new List<int> { endRoadTile, endRoadTile2, endRoadTile3 }, null);

            CombineTileWithQuadrant(quadrant, tiles);

            return quadrant;
        }

        /// <summary>
        /// Invert the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static EDirection InvertDirection(EDirection direction)
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
        /// <param name="worldPosition">Start Position of the Tiles</param>
        private List<List<GameObject>> GenerateQuadrant(Vector2Int worldPosition)
        {
            List<List<GameObject>> tileList = new List<List<GameObject>>();
            float quadrantSize = this.LevelSettings.GetQuadrantSize();

            for (int i = 0; i < quadrantSize; i++)
            {
                tileList.Add(new List<GameObject>());

                for (int j = 0; j < quadrantSize; j++)
                {
                    Vector2Int position = new Vector2Int(worldPosition.x + i, worldPosition.y + j);
                    GameObject tile = Instantiate(this.TilePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
                    tile.GetComponent<BuildingNode>().SetNode(ENodeState.Open, new Vector2Int(i, j));
                    tile.name = $"Tile {i} {j}";
                    tileList[i].Add(tile);
                }
            }

            return tileList;
        }

        /// <summary>
        /// Instantiate the Quadrant
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="startDirection">Start direction of the new Quadrant</param>
        /// <param name="previousEndRoadTile">Start Road of the new Quadrant</param>
        /// <param name="targetDirection">Target direction of the new Quadrant</param>
        /// <param name="endRoadTile">End Road of the new Quadrant</param>
        /// <param name="lastBuildingNode"></param>
        /// <returns></returns>
        private GameObject InstantiateQuadrant(Vector2Int worldPosition, EDirection startDirection, int previousEndRoadTile, List<EDirection> targetDirection, List<int> endRoadTile, BuildingNode lastBuildingNode)
        {
            GameObject newQuadrant = Instantiate(this.QuadrantPrefab, new Vector3(worldPosition.x, 0, worldPosition.y), Quaternion.identity);
            newQuadrant.GetComponent<Quadrant>().InitQuadrant(worldPosition, startDirection, previousEndRoadTile, targetDirection, endRoadTile, this.LevelSettings.GetQuadrantSize(), lastBuildingNode);

            return newQuadrant;
        }

        /// <summary>
        /// To combine the tiles with the Quadrant so the hierarchy is sorted
        /// </summary>
        /// <param name="quadrant"></param>
        /// <param name="tileMatrix"></param>
        private void CombineTileWithQuadrant(GameObject quadrant, IEnumerable<List<GameObject>> tileMatrix)
        {
            foreach (GameObject tile in tileMatrix.SelectMany(tileList => tileList))
            {
                tile.transform.SetParent(quadrant.transform);
            }
        }

        private Vector2Int CalculateQuadrantPoint(EDirection targetDirection, int endRoadTile)
        {
            return targetDirection switch
            {
                EDirection.North => new Vector2Int(endRoadTile, this.LevelSettings.GetQuadrantSize() - 1),
                EDirection.South => new Vector2Int(endRoadTile, 0),
                EDirection.West => new Vector2Int(0, endRoadTile),
                EDirection.East => new Vector2Int(this.LevelSettings.GetQuadrantSize() - 1, endRoadTile),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void CleanUpTiles(List<List<GameObject>> tiles, ICollection<Vector2Int> roadPoints)
        {
            foreach (List<GameObject> tileList in tiles)
            {
                foreach (GameObject tile in tileList)
                {
                    BuildingNode build = tile.GetComponent<BuildingNode>();
                    Node finish = tile.GetComponent<Node>();
                    
                    finish.SetPosition(build.Position);
                    finish.SetParent(build.gameObject.GetComponent<Node>().GetParent());
                    finish.SetTileType(build.GetTileType());
                    Destroy(build);

                    if (roadPoints.Contains(build.Position)) continue;
                    
                    finish.SetTileType(ENodeState.Open);
                    finish.SetParent(null);
                }
            }
        }
    }
}