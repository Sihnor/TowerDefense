using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Enums;
using Code.Scripts.ScriptableScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Generation
{

    #region Weight Struct

    internal enum EWeightTypeOLD
    {
        Cross,
        Square,
        Field
    }

    internal readonly struct WeightStructOLD
    {
        public static List<Vector2Int> GetRandomFigure(int size)
        {
            System.Random random = new System.Random();

            int randomFigure = random.Next(0, 3);
            EWeightTypeOLD type = (EWeightTypeOLD)randomFigure;

            return type switch
            {
                EWeightTypeOLD.Cross => CreateCrossWeightMap(size),
                EWeightTypeOLD.Square => CreateSquareWeightMap(size),
                EWeightTypeOLD.Field => CreateFieldWeightMap(size),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        }

        /// <summary>
        /// Create a cross weight map
        /// X 
        ///
        /// X
        ///
        /// X
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static List<Vector2Int> CreateCrossWeightMap(int distance)
        {
            List<Vector2Int> weightPoints = new List<Vector2Int>();
            weightPoints.Add(new Vector2Int(0, 0));

            for (int i = 1; i <= distance; i++)
            {
                weightPoints.Add(new Vector2Int(0, i));
                weightPoints.Add(new Vector2Int(0, i * -1));
                weightPoints.Add(new Vector2Int(i, 0));
                weightPoints.Add(new Vector2Int(i * -1, 0));
            }

            return weightPoints;
        }

        /// <summary>
        /// Create a square weight map
        /// XXXXX
        /// XXXXX
        /// XXXXX
        /// XXXXX
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static List<Vector2Int> CreateSquareWeightMap(int size)
        {
            List<Vector2Int> weightPoints = new List<Vector2Int>();
            size = 3 + (size - 1) * 2;

            int offset = size / 2;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    weightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                }
            }

            return weightPoints;
        }

        /// <summary>
        /// Create a field weight map
        /// X X X
        ///  X X
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static List<Vector2Int> CreateFieldWeightMap(int size)
        {
            List<Vector2Int> weightPoints = new List<Vector2Int>();
            weightPoints.Add(new Vector2Int(0, 0));
            size = 3 + (size - 1) * 2; //   1 = 3,        3 = 7,      5 = 11,       7 = 15

            int offset = size / 2; //       3 = 1,        5 = 2,      7 = 3,        9 = 4

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0) weightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                        continue;
                    }

                    if ((size / 2) == j) weightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                    if (i == offset && j % 2 == 1) weightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                }
            }

            return weightPoints;
        }
    }

    #endregion

    public class QuadrantGenerator : MonoBehaviour
    {

        #region Variables

        [SerializeField] private GameObject FieldTile;

        private readonly List<List<GameObject>> QuadrantTiles = new List<List<GameObject>>();
        private readonly List<Vector2Int> RoadPath = new List<Vector2Int>();

        #endregion


        // Can be changed to a scriptable object
        [SerializeField] private LevelSettings LevelSettings;

        [SerializeField] private GameObject QuadrantPrefab;


        // Werden durch den vorherigen Quadranten gesetzt
        private Vector2Int StartPoint;
        private EDirection StartDirection;

        private readonly Vector3 StartQuadrantPosition = Vector3.zero;

        // Wird Random gesetzt
        private List<Vector2Int> EndPoints;
        private List<EDirection> TargetDirections;
        private Vector2Int EndPoint;
        private EDirection TargetDirection;

        private void Start()
        {
            //StartGenerating(null);
        }

        public EDirection TESTGenerateInitQuadrant()
        {
            for (int i = 0; i < this.LevelSettings.GetQuadrantSize(); i++)
            {
                for (int j = 0; j < this.LevelSettings.GetQuadrantSize(); j++)
                {
                    Instantiate(this.FieldTile, new Vector3(j, 0, i), Quaternion.Euler(0, 0, 0));
                }
            }
            return Random.Range(0, 3) switch
            {
                0 => EDirection.North,
                1 => EDirection.East,
                2 => EDirection.West,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void TESTGenerateQuadrant()
        {
            
        }

        public Quadrant GenerateStartQuadrant()
        {
            this.StartDirection = (EDirection)Random.Range(0, 4);
            this.TargetDirection = this.StartDirection;

            int center = this.LevelSettings.GetQuadrantSize() / 2;

            this.StartPoint = this.StartDirection switch
            {
                EDirection.North => new Vector2Int(center, center - 2),
                EDirection.South => new Vector2Int(center, center + 2),
                EDirection.West => new Vector2Int(center - 2, center),
                EDirection.East => new Vector2Int(center + 2, center),
                _ => throw new ArgumentOutOfRangeException()
            };

            this.EndPoint = this.TargetDirection switch
            {
                EDirection.North => new Vector2Int(center, 0),
                EDirection.South => new Vector2Int(center, this.LevelSettings.GetQuadrantSize() - 1),
                EDirection.West => new Vector2Int(0, center),
                EDirection.East => new Vector2Int(this.LevelSettings.GetQuadrantSize() - 1, center),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            SpawnQuadrant(Vector3.zero);
            this.CreateAStarGrid();

            StartCoroutine(CreatePath(true));

            return null;
        }

        public void StartGenerating(Quadrant PreviousStart)
        {
            // Get A new Target Point
            //CreateEndPoint(PreviousStart.GetTargetDirection(), PreviousStart.GetEndRoadTile());
            //CreateRandomEndPoint();
            
            // Create a new Quadrant
            SpawnQuadrant(this.StartQuadrantPosition);

            SpawnTargetPoints();

            // Create A* Grid to navigate through the Quadrant
            CreateAStarGrid();

            // Generate Block Map to block some tiles to get more interesting paths
            BakeWeightMap(SpawnWeightPoints());

            StartCoroutine(CreatePath());
        }

        private void CreateEndPoint(EDirection previousTargetDirection, int previousEndPoint)
        {
            this.StartDirection = previousTargetDirection;

            this.StartPoint = this.StartDirection switch
            {
                EDirection.North => new Vector2Int(previousEndPoint,0),
                EDirection.South => new Vector2Int(previousEndPoint, this.LevelSettings.GetQuadrantSize() - 1),
                EDirection.West => new Vector2Int(0, previousEndPoint),
                EDirection.East => new Vector2Int(this.LevelSettings.GetQuadrantSize() - 1, previousEndPoint),
                _ => throw new ArgumentOutOfRangeException(nameof(previousEndPoint), previousEndPoint, null)
            };
        }

        private void CreateRandomEndPoint()
        {
            this.StartDirection = (EDirection)Random.Range(0, 4);
            this.TargetDirection = (EDirection)Random.Range(0, 4);

            while (this.StartDirection == this.TargetDirection)
            {
                this.TargetDirection = (EDirection)Random.Range(0, 4);
            }

            int QuadrantSize = this.LevelSettings.GetQuadrantSize();

            this.StartPoint = this.StartDirection switch
            {
                EDirection.North => new Vector2Int(Random.Range(0, QuadrantSize), QuadrantSize - 1),
                EDirection.South => new Vector2Int(Random.Range(0, QuadrantSize), 0),
                EDirection.West => new Vector2Int(0, Random.Range(0, QuadrantSize)),
                EDirection.East => new Vector2Int(QuadrantSize - 1, Random.Range(0, QuadrantSize)),
                _ => this.StartPoint
            };

            this.EndPoint = this.TargetDirection switch
            {
                EDirection.North => new Vector2Int(Random.Range(0, QuadrantSize), QuadrantSize - 1),
                EDirection.South => new Vector2Int(Random.Range(0, QuadrantSize), 0),
                EDirection.West => new Vector2Int(0, Random.Range(0, QuadrantSize)),
                EDirection.East => new Vector2Int(QuadrantSize - 1, Random.Range(0, QuadrantSize)),
                _ => this.EndPoint
            };
        }

        private void SpawnQuadrant(Vector3 position)
        {
            for (int i = 0; i < this.LevelSettings.GetQuadrantSize(); i++)
            {
                this.QuadrantTiles.Add(new List<GameObject>());

                for (int j = 0; j < this.LevelSettings.GetQuadrantSize(); j++)
                {
                    Vector3 spawnPosition = position + new Vector3(j, 0, i);

                    this.QuadrantTiles[i].Add(Instantiate(this.FieldTile, spawnPosition, Quaternion.Euler(0, 0, 0)));
                    this.QuadrantTiles[i][j].GetComponent<BuildingNode>().SetWeight(1);
                    this.QuadrantTiles[i][j].GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
        }

        private List<Vector2Int> SpawnTargetPoints()
        {
            List<Vector2Int> TargetPoints = new List<Vector2Int>();
            
            int QuadrantSize = this.LevelSettings.GetQuadrantSize();

            int justified = 1 + (QuadrantSize / 5);
            int numTargets = (justified >= QuadrantSize / 2) ? justified : Random.Range(justified, QuadrantSize / 2);
            numTargets = 3;
            bool ignoreDistance = numTargets == justified;

            // First Target

            TargetPoints!.Add(new Vector2Int(Random.Range(1, QuadrantSize - 2), Random.Range(2, QuadrantSize - 2)));

            while (TargetPoints.Count < numTargets)
            {
                Vector2Int target = new Vector2Int(Random.Range(2, QuadrantSize - 2), Random.Range(2, QuadrantSize - 2));
                bool add = true;

                foreach (Vector2Int point in TargetPoints.Where(point => Vector2Int.Distance(target, point) < 3 && !ignoreDistance))
                {
                    add = false;
                }

                if (add) TargetPoints.Add(target);
            }

            foreach (Vector2Int point in TargetPoints)
            {
                DrawCyanNode(this.QuadrantTiles[point.y][point.x].GetComponent<BuildingNode>());
            }

            return TargetPoints;
        }

        private void ResetQuadrant(Vector3 position)
        {
            for (int i = 0; i < this.LevelSettings.GetQuadrantSize(); i++)
            {
                for (int j = 0; j < this.LevelSettings.GetQuadrantSize(); j++)
                {
                    Destroy(this.QuadrantTiles[i][j]);
                }

                this.QuadrantTiles[i].Clear();
            }

            this.QuadrantTiles.Clear();
        }

        private void Update()
        {
            // On Space bar click
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                ResetQuadrant(Vector3.zero);
                Start();
            }
        }

        #region A* Pathfinding Anstz

        private List<Vector2Int> SpawnWeightPoints()
        {
            List<Vector2Int> weightPoints = new List<Vector2Int>();

            int QuadrantSize = this.LevelSettings.GetQuadrantSize();
            
            // Corners of the Quadrant
            weightPoints.Add(new Vector2Int(1, 1));
            weightPoints.Add(new Vector2Int(1, QuadrantSize - 2));
            weightPoints.Add(new Vector2Int(QuadrantSize - 2, 1));
            weightPoints.Add(new Vector2Int(QuadrantSize - 2, QuadrantSize - 2));

            // Middle inside corners
            weightPoints.Add(new Vector2Int(QuadrantSize / 2, 1));
            weightPoints.Add(new Vector2Int(QuadrantSize / 2, QuadrantSize - 2));
            weightPoints.Add(new Vector2Int(1, QuadrantSize / 2));
            weightPoints.Add(new Vector2Int(QuadrantSize - 2, QuadrantSize / 2));

            const int startInnerSquare = 3;
            int endInnerSquare = QuadrantSize - 3;

            if (endInnerSquare - startInnerSquare <= 3) return weightPoints;

            // Random Points
            for (int i = 0; i < QuadrantSize - 6; i++)
            {
                weightPoints.Add(new Vector2Int(Random.Range(startInnerSquare, endInnerSquare), Random.Range(startInnerSquare, endInnerSquare)));
            }

            return weightPoints;
        }

        private void BakeWeightMap(List<Vector2Int> WeightPoints)
        {
            foreach (Vector2Int point in WeightPoints)
            {
                List<Vector2Int> weightPoints = WeightStruct.GetRandomFigure(Random.Range(3, this.LevelSettings.GetQuadrantSize() / 2));

                foreach (Vector2Int weightPoint in weightPoints)
                {
                    int x = point.x + weightPoint.x;
                    int y = point.y + weightPoint.y;

                    if (x < 0 || x >= this.LevelSettings.GetQuadrantSize() || y < 0 || y >= this.LevelSettings.GetQuadrantSize()) continue;

                    this.QuadrantTiles[y][x].GetComponent<BuildingNode>().IncreaseWeight(Random.value * 3);
                }
            }
        }

        private void CreateAStarGrid()
        {
            for (int i = 0; i < this.LevelSettings.GetQuadrantSize(); i++)
            {
                for (int j = 0; j < this.LevelSettings.GetQuadrantSize(); j++)
                {
                    this.QuadrantTiles[i][j].GetComponent<BuildingNode>().SetNode(ENodeState.Open, new Vector2Int(j, i));
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (this.RoadPath.Count == 0) return;

            for (int i = 0; i < this.RoadPath.Count - 1; i++)
            {
                Vector3 start = new Vector3(this.RoadPath[i].x, 0.5f, this.RoadPath[i].y);
                Vector3 end = new Vector3(this.RoadPath[i + 1].x, 0.5f, this.RoadPath[i + 1].y);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(start, end);
            }
        }

        private IEnumerator CreatePath(bool startQuadrant = false)
        {
            while (true)
            {
                bool notFinished = true;

                List<Vector2Int> TargetPoints = SpawnTargetPoints();
                this.RoadPath.Clear();
                
                for (int i = 0; i <= TargetPoints.Count; i++)
                {
                    // Reset the Quadrant
                    for (int j = 0; j < this.LevelSettings.GetQuadrantSize(); j++)
                    {
                        for (int k = 0; k < this.LevelSettings.GetQuadrantSize(); k++)
                        {
                            // Get the node
                            BuildingNode buildingNode = this.QuadrantTiles[j][k].GetComponent<BuildingNode>();
                            ResetColorNode(buildingNode);
                            buildingNode.SetTileType(ENodeState.Open);
                        }
                    }

                    // Block the Road Path
                    foreach (Vector2Int point in this.RoadPath)
                    {
                        this.QuadrantTiles[point.y][point.x].GetComponent<BuildingNode>().SetTileType(ENodeState.Blocked);
                        DrawBlackNode(this.QuadrantTiles[point.y][point.x].GetComponent<BuildingNode>());
                    }

                    IEnumerator findPath;

                    // Fist Point
                    if (i == 0 && !startQuadrant)
                    {
                        findPath = FindPath(this.StartPoint, TargetPoints[i]);
                    }
                    // Last Point
                    else if (i == TargetPoints.Count && !startQuadrant)
                    {
                        findPath = FindPath(TargetPoints[i - 1], this.EndPoint);
                    }
                    else if (startQuadrant)
                    {
                        findPath = FindPath(this.StartPoint, this.EndPoint);
                    }
                    // Middle Points
                    else
                    {
                        findPath = FindPath(TargetPoints[i - 1], TargetPoints[i]);
                    }

                    yield return findPath;
                    //yield return new WaitForSeconds(5);

                    string result = findPath.Current as string;
                    Debug.Log(result);

                    if (result != "No Path Found") continue;
                    notFinished = false;
                    break;
                }

                if (notFinished)
                {
                    break;
                }
                if (startQuadrant)
                {
                    break;
                }
            }

            StopAllCoroutines();

            yield return null;
        }

        private IEnumerator FindPath(Vector2Int start, Vector2Int end)
        {
            List<BuildingNode> openSet = new List<BuildingNode>();
            HashSet<BuildingNode> closedSet = new HashSet<BuildingNode>();

            BuildingNode startBuildingNode = this.QuadrantTiles[start.y][start.x].GetComponent<BuildingNode>();
            BuildingNode endBuildingNode = this.QuadrantTiles[end.y][end.x].GetComponent<BuildingNode>();

            startBuildingNode.SetWeight(0);
            startBuildingNode.SetParent(null);

            openSet.Add(startBuildingNode);

            while (openSet.Count > 0)
            {
                DrawGrayNode(startBuildingNode);
                DrawBlackNode(endBuildingNode);

                BuildingNode currentBuildingNode = openSet[0];

                foreach (BuildingNode node in openSet.Where(node => node.GetFCost() < currentBuildingNode.GetFCost()))
                {
                    currentBuildingNode = node;
                }

                DrawYellowNode(currentBuildingNode);

                openSet.Remove(currentBuildingNode);

                if (IsEndNode(currentBuildingNode, endBuildingNode))
                {
                    this.RoadPath.AddRange(ReconstructPath(currentBuildingNode));
                    currentBuildingNode.SetTileType(ENodeState.Closed);
                    break;
                }

                List<BuildingNode> neighbours = GetOpenNeighbours(currentBuildingNode);

                foreach (BuildingNode neighbour in neighbours)
                {
                    DrawCyanNode(neighbour);
                }

                foreach (BuildingNode neighbour in neighbours)
                {
                    float tentativeGCost = currentBuildingNode.GetGCost() + neighbour.GetCostToEnter();

                    if (openSet.Contains(neighbour) && !(tentativeGCost < neighbour.GetGCost())) continue;

                    neighbour.SetParent(currentBuildingNode);
                    neighbour.SetGCost(tentativeGCost);
                    neighbour.SetHCost(CalculateManhattanDistanceWithWeights(neighbour, endBuildingNode));

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }

                //yield return new WaitForSeconds(0.1f);

                foreach (BuildingNode neighbour in neighbours)
                {
                    DrawGreenNode(neighbour);
                }

                DrawRedNode(currentBuildingNode);

                currentBuildingNode.SetTileType(ENodeState.Closed);
                closedSet.Add(currentBuildingNode);
            }

            if (openSet.Count == 0)
            {
                yield return "No Path Found";
            }
            else
            {
                yield return "Path Found";
            }
        }

        List<BuildingNode> GetOpenNeighbours(BuildingNode buildingNode)
        {
            List<BuildingNode> neighbours = new List<BuildingNode>();

            Func<BuildingNode, bool> isNodeOpen = node => node.TileType == ENodeState.Open;

            // North
            if (buildingNode.Position.y - 1 >= 0)
            {
                BuildingNode n = this.QuadrantTiles[buildingNode.Position.y - 1][buildingNode.Position.x].GetComponent<BuildingNode>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            // South
            if (buildingNode.Position.y + 1 < this.LevelSettings.GetQuadrantSize())
            {
                BuildingNode n = this.QuadrantTiles[buildingNode.Position.y + 1][buildingNode.Position.x].GetComponent<BuildingNode>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            // West
            if (buildingNode.Position.x - 1 >= 0)
            {
                BuildingNode n = this.QuadrantTiles[buildingNode.Position.y][buildingNode.Position.x - 1].GetComponent<BuildingNode>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            // East
            if (buildingNode.Position.x + 1 < this.LevelSettings.GetQuadrantSize())
            {
                BuildingNode n = this.QuadrantTiles[buildingNode.Position.y][buildingNode.Position.x + 1].GetComponent<BuildingNode>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            return neighbours;
        }

        private float CalculateManhattanDistanceWithWeights(BuildingNode a, BuildingNode b)
        {
            List<int[]> indexes = new List<int[]>();

            int minX = Mathf.Min(a.Position.x, b.Position.x);
            int maxX = Mathf.Max(a.Position.x, b.Position.x);
            int minY = Mathf.Min(a.Position.y, b.Position.y);
            int maxY = Mathf.Max(a.Position.y, b.Position.y);

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    indexes.Add(new int[] { i, j });
                }
            }

            return indexes.Sum(index => this.QuadrantTiles[index[1]][index[0]].GetComponent<BuildingNode>().GetWeight());
        }

        private bool IsEndNode(BuildingNode buildingNode, BuildingNode endBuildingNode)
        {
            return buildingNode.Position == endBuildingNode.Position;
        }

        private List<Vector2Int> ReconstructPath(BuildingNode currentBuildingNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            while (currentBuildingNode != null)
            {
                path.Add(currentBuildingNode.Position);
                currentBuildingNode = currentBuildingNode.GetParent();
            }

            path.Reverse();

            foreach (Vector2Int variable in path)
            {
                DrawBlackNode(this.QuadrantTiles[variable.y][variable.x].GetComponent<BuildingNode>());
            }

            return path;
        }

        private void GenerateQuadrant()
        {
        }

        private void DrawGreenNode(BuildingNode buildingNode)
        {
            buildingNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }

        private void DrawRedNode(BuildingNode buildingNode)
        {
            buildingNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        private void DrawYellowNode(BuildingNode buildingNode)
        {
            buildingNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }

        private void DrawCyanNode(BuildingNode buildingNode)
        {
            buildingNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }

        private void DrawGrayNode(BuildingNode buildingNode)
        {
            buildingNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.gray;
        }

        private void ResetColorNode(BuildingNode buildingNode)
        {
            buildingNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        private void DrawBlackNode(BuildingNode buildingNode)
        {
            buildingNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        #endregion


    }
}