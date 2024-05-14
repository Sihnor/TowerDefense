using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Enums;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts
{
    internal enum EWeightType
    {
        Cross,
        Square,
        Field
    }

    internal readonly struct WeightStruct
    {
        public static List<Vector2Int> GetRandomFigure(int size)
        {
            System.Random random = new System.Random();

            int randomFigure = random.Next(0, 3);
            EWeightType type = (EWeightType)randomFigure;

            return type switch
            {
                EWeightType.Cross => CreateCrossWeightMap(size),
                EWeightType.Square => CreateSquareWeightMap(size),
                EWeightType.Field => CreateFieldWeightMap(size),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        }

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



    public class QuadrantGenerator : MonoBehaviour
    {

        #region Variables

        [SerializeField] private GameObject FieldTile;

        private readonly List<List<GameObject>> QuadrantTiles = new List<List<GameObject>>();
        private readonly List<Vector2Int> RoadPath = new List<Vector2Int>();

        #endregion


        // Can be changed to a scriptable object
        [SerializeField] private int QuadrantSize = 15;


        // Werden durch den vorherigen Quadranten gesetzt
        private Vector2Int StartPoint;
        private EDirection StartDirection;

        private readonly Vector3 StartQuadrantPosition = Vector3.zero;

        // Wird Random gesetzt
        private Vector2Int EndPoint;
        private EDirection TargetDirection;
        
        private void Start()
        {
            StartGenerating(null);
        }

        public void StartGenerating(Quadrant PreviousStart)
        {
            // Get A new Target Point
            CreateEndPoint(PreviousStart);

            // Create a new Quadrant
            SpawnQuadrant(this.StartQuadrantPosition);

            SpawnTargetPoints();

            // Create A* Grid to navigate through the Quadrant
            CreateAStarGrid();

            // Generate Block Map to block some tiles to get more interesting paths
            BakeWeightMap(SpawnWeightPoints());

            StartCoroutine(CreatePath());
        }

        private void CreateEndPoint(Quadrant quadrant)
        {
            this.StartDirection = (EDirection)Random.Range(0, 4);
            this.TargetDirection = (EDirection)Random.Range(0, 4);

            while (this.StartDirection == this.TargetDirection)
            {
                this.TargetDirection = (EDirection)Random.Range(0, 4);
            }

            if (this.StartDirection == EDirection.North) this.StartPoint = new Vector2Int(Random.Range(0, this.QuadrantSize), this.QuadrantSize - 1);
            if (this.StartDirection == EDirection.South) this.StartPoint = new Vector2Int(Random.Range(0, this.QuadrantSize), 0);
            if (this.StartDirection == EDirection.West) this.StartPoint = new Vector2Int(0, Random.Range(0, this.QuadrantSize));
            if (this.StartDirection == EDirection.East) this.StartPoint = new Vector2Int(this.QuadrantSize - 1, Random.Range(0, this.QuadrantSize));

            if (this.TargetDirection == EDirection.North) this.EndPoint = new Vector2Int(Random.Range(0, this.QuadrantSize), this.QuadrantSize - 1);
            if (this.TargetDirection == EDirection.South) this.EndPoint = new Vector2Int(Random.Range(0, this.QuadrantSize), 0);
            if (this.TargetDirection == EDirection.West) this.EndPoint = new Vector2Int(0, Random.Range(0, this.QuadrantSize));
            if (this.TargetDirection == EDirection.East) this.EndPoint = new Vector2Int(this.QuadrantSize - 1, Random.Range(0, this.QuadrantSize));
        }

        private void SpawnQuadrant(Vector3 position)
        {
            for (int i = 0; i < this.QuadrantSize; i++)
            {
                this.QuadrantTiles.Add(new List<GameObject>());

                for (int j = 0; j < this.QuadrantSize; j++)
                {
                    Vector3 spawnPosition = position + new Vector3(j, 0, i);

                    this.QuadrantTiles[i].Add(Instantiate(this.FieldTile, spawnPosition, Quaternion.Euler(0, 0, 0)));
                    this.QuadrantTiles[i][j].GetComponent<Node>().SetWeight(1);
                    this.QuadrantTiles[i][j].GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
        }

        private List<Vector2Int> SpawnTargetPoints()
        {
            List<Vector2Int> TargetPoints = new List<Vector2Int>();

            int justified = 1 + (this.QuadrantSize / 5);
            int numTargets = (justified >= this.QuadrantSize / 2) ? justified : Random.Range(justified, this.QuadrantSize / 2);
            numTargets = 3;
            bool ignoreDistance = numTargets == justified;

            // First Target

            TargetPoints!.Add(new Vector2Int(Random.Range(1, this.QuadrantSize - 2), Random.Range(2, this.QuadrantSize - 2)));

            while (TargetPoints.Count < numTargets)
            {
                Vector2Int target = new Vector2Int(Random.Range(2, this.QuadrantSize - 2), Random.Range(2, this.QuadrantSize - 2));
                bool add = true;

                foreach (Vector2Int point in TargetPoints.Where(point => Vector2Int.Distance(target, point) < 3 && !ignoreDistance))
                {
                    add = false;
                }

                if (add) TargetPoints.Add(target);
            }

            foreach (Vector2Int point in TargetPoints)
            {
                DrawCyanNode(this.QuadrantTiles[point.y][point.x].GetComponent<Node>());
            }
            
            return TargetPoints;
        }

        private void ResetQuadrant(Vector3 position)
        {
            for (int i = 0; i < this.QuadrantSize; i++)
            {
                for (int j = 0; j < this.QuadrantSize; j++)
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

            // Corners of the Quadrant
            weightPoints.Add(new Vector2Int(1, 1));
            weightPoints.Add(new Vector2Int(1, this.QuadrantSize - 2));
            weightPoints.Add(new Vector2Int(this.QuadrantSize - 2, 1));
            weightPoints.Add(new Vector2Int(this.QuadrantSize - 2, this.QuadrantSize - 2));

            // Middle inside corners
            weightPoints.Add(new Vector2Int((int)this.QuadrantSize / 2, 1));
            weightPoints.Add(new Vector2Int((int)this.QuadrantSize / 2, this.QuadrantSize - 2));
            weightPoints.Add(new Vector2Int(1, (int)this.QuadrantSize / 2));
            weightPoints.Add(new Vector2Int(this.QuadrantSize - 2, (int)this.QuadrantSize / 2));

            const int startInnerSquare = 3;
            int endInnerSquare = this.QuadrantSize - 3;

            if (endInnerSquare - startInnerSquare <= 3) return weightPoints;

            // Random Points
            for (int i = 0; i < this.QuadrantSize - 6; i++)
            {
                weightPoints.Add(new Vector2Int(Random.Range(startInnerSquare, endInnerSquare), Random.Range(startInnerSquare, endInnerSquare)));
            }

            return weightPoints;

            foreach (Vector2Int point in weightPoints)
            {
                this.QuadrantTiles[point.y][point.x].GetComponent<MeshRenderer>().material.color = Color.magenta;
            }
        }
        
        private void BakeWeightMap(List<Vector2Int> WeightPoints)
        {
            foreach (Vector2Int point in WeightPoints)
            {
                List<Vector2Int> weightPoints = WeightStruct.GetRandomFigure(Random.Range(3, this.QuadrantSize / 2));

                foreach (Vector2Int weightPoint in weightPoints)
                {
                    int x = point.x + weightPoint.x;
                    int y = point.y + weightPoint.y;

                    if (x < 0 || x >= this.QuadrantSize || y < 0 || y >= this.QuadrantSize) continue;

                    this.QuadrantTiles[y][x].GetComponent<Node>().IncreaseWeight(Random.value * 3);
                }
            }
        }

        private void CreateAStarGrid()
        {
            for (int i = 0; i < this.QuadrantSize; i++)
            {
                for (int j = 0; j < this.QuadrantSize; j++)
                {
                    this.QuadrantTiles[i][j].GetComponent<Node>().SetNode(ENodeState.Open, new Vector2Int(j, i));
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

        private IEnumerator CreatePath()
        {
            while (true)
            {
                bool notFinished = true;
                
                List<Vector2Int> TargetPoints = SpawnTargetPoints();
                this.RoadPath.Clear();
                
                
                
                for (int i = 0; i <= TargetPoints.Count; i++)
                {
                    // Reset the Quadrant
                    for (int j = 0; j < this.QuadrantSize; j++)
                    {
                        for (int k = 0; k < this.QuadrantSize; k++)
                        {
                            // Get the node
                            Node node = this.QuadrantTiles[j][k].GetComponent<Node>();
                            ResetColorNode(node);
                            node.SetTileType(ENodeState.Open);
                        }
                    }
                    
                    // Block the Road Path
                    foreach (Vector2Int point in this.RoadPath)
                    {
                        this.QuadrantTiles[point.y][point.x].GetComponent<Node>().SetTileType(ENodeState.Blocked);
                        DrawBlackNode(this.QuadrantTiles[point.y][point.x].GetComponent<Node>());
                    }

                    IEnumerator findPath;
                
                    // Fist Point
                    if (i == 0)
                    {
                        findPath = FindPath(this.StartPoint, TargetPoints[i]);
                    }
                    // Last Point
                    else if (i == TargetPoints.Count)
                    {
                        findPath = FindPath(TargetPoints[i-1], this.EndPoint);
                    }
                    // Middle Points
                    else
                    {
                        findPath = FindPath(TargetPoints[i - 1], TargetPoints[i]);
                    }

                    yield return findPath;
                    //yield return new WaitForSeconds(5);
                
                    string result = findPath.Current as string;

                    if (result != "No Path Found") continue;
                    notFinished = false;
                    break;
                }  
                if (notFinished)
                {
                    break;
                }
            }
            StopAllCoroutines();

            yield return null;
        }

        private IEnumerator FindPath(Vector2Int start, Vector2Int end)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            Node startNode = this.QuadrantTiles[start.y][start.x].GetComponent<Node>();
            Node endNode = this.QuadrantTiles[end.y][end.x].GetComponent<Node>();

            startNode.SetWeight(0);
            startNode.SetParent(null);

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                DrawGrayNode(startNode);
                DrawBlackNode(endNode);

                Node currentNode = openSet[0];

                foreach (Node node in openSet.Where(node => node.GetFCost() < currentNode.GetFCost()))
                {
                    currentNode = node;
                }

                DrawYellowNode(currentNode);

                openSet.Remove(currentNode);

                if (IsEndNode(currentNode, endNode))
                {
                    this.RoadPath.AddRange(ReconstructPath(currentNode));
                    currentNode.SetTileType(ENodeState.Closed);
                    break;
                }

                List<Node> neighbours = GetOpenNeighbours(currentNode);

                foreach (Node neighbour in neighbours)
                {
                    DrawCyanNode(neighbour);
                }

                foreach (Node neighbour in neighbours)
                {
                    float tentativeGCost = currentNode.GetGCost() + neighbour.GetCostToEnter();

                    if (openSet.Contains(neighbour) && !(tentativeGCost < neighbour.GetGCost())) continue;

                    neighbour.SetParent(currentNode);
                    neighbour.SetGCost(tentativeGCost);
                    neighbour.SetHCost(CalculateManhattanDistanceWithWeights(neighbour, endNode));

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }

                //yield return new WaitForSeconds(0.001f);

                foreach (Node neighbour in neighbours)
                {
                    DrawGreenNode(neighbour);
                }

                DrawRedNode(currentNode);

                currentNode.SetTileType(ENodeState.Closed);
                closedSet.Add(currentNode);
            }

            if (openSet.Count == 0)
            {
                yield return "No Path Found";
            }else
            {
                yield return "Path Found";
            }
        }

        List<Node> GetOpenNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            Func<Node, bool> isNodeOpen = node => node.TileType == ENodeState.Open;

            // North
            if (node.Position.y - 1 >= 0)
            {
                Node n = this.QuadrantTiles[node.Position.y - 1][node.Position.x].GetComponent<Node>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            // South
            if (node.Position.y + 1 < this.QuadrantSize)
            {
                Node n = this.QuadrantTiles[node.Position.y + 1][node.Position.x].GetComponent<Node>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            // West
            if (node.Position.x - 1 >= 0)
            {
                Node n = this.QuadrantTiles[node.Position.y][node.Position.x - 1].GetComponent<Node>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            // East
            if (node.Position.x + 1 < this.QuadrantSize)
            {
                Node n = this.QuadrantTiles[node.Position.y][node.Position.x + 1].GetComponent<Node>();
                if (isNodeOpen(n)) neighbours.Add(n);
            }

            return neighbours;
        }

        private float CalculateManhattanDistanceWithWeights(Node a, Node b)
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

            return indexes.Sum(index => this.QuadrantTiles[index[1]][index[0]].GetComponent<Node>().GetWeight());
        }

        private bool IsEndNode(Node node, Node endNode)
        {
            return node.Position == endNode.Position;
        }

        private List<Vector2Int> ReconstructPath(Node currentNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.GetParent();
            }

            path.Reverse();

            foreach (Vector2Int variable in path)
            {
                DrawBlackNode(this.QuadrantTiles[variable.y][variable.x].GetComponent<Node>());
            }

            return path;

        }
        
        private void DrawGreenNode(Node node)
        {
            node.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        
        private void DrawRedNode(Node node)
        {
            node.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        
        private void DrawYellowNode(Node node)
        {
            node.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        
        private void DrawCyanNode(Node node)
        {
            node.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
        
        private void DrawGrayNode(Node node)
        {
            node.gameObject.GetComponent<MeshRenderer>().material.color = Color.gray;
        }
        
        private void ResetColorNode(Node node)
        {
            node.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        
        private void DrawBlackNode(Node node)
        {
            node.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        #endregion


    }
}