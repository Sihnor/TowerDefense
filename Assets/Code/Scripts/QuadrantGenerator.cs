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
            List<Vector2Int> WeightPoints = new List<Vector2Int>();
            WeightPoints.Add(new Vector2Int(0, 0));

            for (int i = 1; i <= distance; i++)
            {
                WeightPoints.Add(new Vector2Int(0, i));
                WeightPoints.Add(new Vector2Int(0, i * -1));
                WeightPoints.Add(new Vector2Int(i, 0));
                WeightPoints.Add(new Vector2Int(i * -1, 0));
            }

            return WeightPoints;
        }

        private static List<Vector2Int> CreateSquareWeightMap(int size)
        {
            List<Vector2Int> WeightPoints = new List<Vector2Int>();
            size = 3 + (size - 1) * 2;

            int offset = size / 2;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    WeightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                }
            }

            return WeightPoints;
        }

        private static List<Vector2Int> CreateFieldWeightMap(int size)
        {
            List<Vector2Int> WeightPoints = new List<Vector2Int>();
            WeightPoints.Add(new Vector2Int(0, 0));
            size = 3 + (size - 1) * 2; //   1 = 3,        3 = 7,      5 = 11,       7 = 15

            int offset = size / 2; //       3 = 1,        5 = 2,      7 = 3,        9 = 4

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0) WeightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                        continue;
                    }

                    if ((size / 2) == j) WeightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                    if (i == offset && j % 2 == 1) WeightPoints.Add(new Vector2Int((i - offset), (j - offset)));
                }
            }

            return WeightPoints;
        }
    }



    public class QuadrantGenerator : MonoBehaviour
    {

        #region Variables
        [SerializeField] private GameObject FieldTile;

        List<List<GameObject>> QuadrantTiles = new List<List<GameObject>>();
        List<Vector2Int> WeightPoints = new List<Vector2Int>();
        List<Vector2Int> TargetPoints = new List<Vector2Int>();

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
            // Get A new Target Point
            CreateEndPoint();

            // Create a new Quadrant
            SpawnQuadrant(this.StartQuadrantPosition);

            SpawnTargetPoints();

            SpawnWeightPoints();
            
            // Create A* Grid to navigate through the Quadrant
            CreateAStarGrid();

            // Generate Block Map to block some tiles to get more interesting paths
            BakeWeightMap();

            //List<Vector2Int> road = FindPath(this.StartPoint, this.EndPoint);
            StartCoroutine(FindPath(this.StartPoint, this.EndPoint));
        }

        

        private void CreateEndPoint()
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

        private void SpawnTargetPoints()
        {
            int justified = 1 + (this.QuadrantSize / 5);
            int numTargets = (justified >= this.QuadrantSize / 2) ? justified : Random.Range(justified, this.QuadrantSize / 2);
            bool ignoreDistance = numTargets == justified;

            // First Target

            this.TargetPoints.Add(new Vector2Int(Random.Range(1, this.QuadrantSize - 2), Random.Range(2, this.QuadrantSize - 2)));

            while (this.TargetPoints.Count < numTargets)
            {
                Vector2Int target = new Vector2Int(Random.Range(2, this.QuadrantSize - 2), Random.Range(2, this.QuadrantSize - 2));
                bool add = true;

                foreach (Vector2Int point in this.TargetPoints)
                {
                    if (Vector2Int.Distance(target, point) < 3 && !ignoreDistance) add = false;
                }

                if (add) this.TargetPoints.Add(target);
            }

            return;
            foreach (Vector2Int point in this.TargetPoints)
            {
                this.QuadrantTiles[point.y][point.x].GetComponent<MeshRenderer>().material.color = Color.cyan;
            }
        }

        private void SpawnWeightPoints()
        {
            this.WeightPoints.Clear();
            
            // Corners of the Quadrant
            this.WeightPoints.Add(new Vector2Int(1, 1));
            this.WeightPoints.Add(new Vector2Int(1, this.QuadrantSize - 2));
            this.WeightPoints.Add(new Vector2Int(this.QuadrantSize - 2, 1));
            this.WeightPoints.Add(new Vector2Int(this.QuadrantSize - 2, this.QuadrantSize - 2));

            // Middle inside corners
            this.WeightPoints.Add(new Vector2Int((int)this.QuadrantSize / 2, 1));
            this.WeightPoints.Add(new Vector2Int((int)this.QuadrantSize / 2, this.QuadrantSize - 2));
            this.WeightPoints.Add(new Vector2Int(1, (int)this.QuadrantSize / 2));
            this.WeightPoints.Add(new Vector2Int(this.QuadrantSize - 2, (int)this.QuadrantSize / 2));

            const int startInnerSquare = 3;
            int endInnerSquare = this.QuadrantSize - 3;

            if (endInnerSquare - startInnerSquare <= 3) return;

            // Random Points
            for (int i = 0; i < this.QuadrantSize - 6; i++)
            {
                this.WeightPoints.Add(new Vector2Int(Random.Range(startInnerSquare, endInnerSquare), Random.Range(startInnerSquare, endInnerSquare)));
            }

            return;
            foreach (Vector2Int point in this.WeightPoints)
            {
                this.QuadrantTiles[point.y][point.x].GetComponent<MeshRenderer>().material.color = Color.magenta;
            }
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

        private void BakeWeightMap()
        {
            foreach (Vector2Int point in this.WeightPoints)
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
                    this.QuadrantTiles[i][j].GetComponent<Node>().SetNode( ENodeState.Open, new Vector2Int(j, i));
                }
            }
        }

        
        
        
        List<Vector2Int> RoadPath = new List<Vector2Int>();
        

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
                startNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.gray;
                endNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
                
                Node currentNode = openSet[0];

                foreach (Node node in openSet.Where(node => node.GetFCost() < currentNode.GetFCost()))
                {
                    currentNode = node;
                }
                currentNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;

                openSet.Remove(currentNode);

                if (IsEndNode(currentNode))
                {
                    StopAllCoroutines();
                    Debug.Log("Path found");
                    this.RoadPath = ReconstructPath(currentNode);
                    break;
                }

                List<Node> neighbours = GetOpenNeighbours(currentNode);

                foreach (Node neighbour in neighbours)
                {
                    neighbour.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                }
                
                foreach (Node neighbour in neighbours)
                {
                    //float tentativeGCost = currentNode.GetGCost() + CalculateManhattanDistanceWithWeights(currentNode, neighbor);
                    float tentativeGCost = currentNode.GetGCost() + neighbour.GetCostToEnter();

                    if (openSet.Contains(neighbour) && !(tentativeGCost < neighbour.GetGCost())) continue;
                    
                    neighbour.SetParent(currentNode);
                    neighbour.SetGCost(tentativeGCost);
                    neighbour.SetHCost(CalculateManhattanDistanceWithWeights(neighbour, endNode));
                        
                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
                
                yield return new WaitForSeconds(0.01f);

                foreach (Node neighbour in neighbours)
                {
                    neighbour.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                }

                currentNode.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                
                currentNode.SetTileType(ENodeState.Closed);
                closedSet.Add(currentNode);
            }

        }

        List<Node> GetOpenNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            Func<Node, bool> IsNodeOpen = node => node.TileType == ENodeState.Open;

            // North
            if (node.Position.y - 1 >= 0)
            {
                Node n = this.QuadrantTiles[node.Position.y - 1][node.Position.x].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // South
            if (node.Position.y + 1 < this.QuadrantSize)
            {
                Node n = this.QuadrantTiles[node.Position.y + 1][node.Position.x].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // West
            if (node.Position.x - 1 >= 0)
            {
                Node n = this.QuadrantTiles[node.Position.y][node.Position.x - 1].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // East
            if (node.Position.x + 1 < this.QuadrantSize)
            {
                Node n = this.QuadrantTiles[node.Position.y][node.Position.x + 1].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
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

        private bool IsEndNode(Node node)
        {
            return node.Position == this.EndPoint;
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

            foreach (var VARIABLE in path)
            {
                this.QuadrantTiles[VARIABLE.y][VARIABLE.x].GetComponent<MeshRenderer>().material.color = Color.black;
            }
            return path;

        }

        #endregion


    }
}