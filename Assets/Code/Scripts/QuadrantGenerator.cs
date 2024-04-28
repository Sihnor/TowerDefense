﻿using System.Collections.Generic;
using Code.Scripts.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts
{
    public class QuadrantGenerator : MonoBehaviour
    {

        #region Variables

        [Header("RoadTiles")] [SerializeField] private GameObject RoadTileStraight;
        [SerializeField] private GameObject RoadTileCorner;
        [SerializeField] private GameObject RoadTileT;
        [SerializeField] private GameObject RoadTileCross;
        [SerializeField] private GameObject BLOCK;
        [SerializeField] private GameObject TARGET;
        [SerializeField] private GameObject ROAD;


        [SerializeField] private GameObject FieldTile;
        [SerializeField] private GameObject MountainTile;
        [SerializeField] private GameObject ForestTile;
        [SerializeField] private GameObject WaterTile;

        List<List<GameObject>> QuadrantTiles = new List<List<GameObject>>();
        List<List<Node>> QuadrantNodes = new List<List<Node>>();

        #endregion


        // Can be changed to a scriptable object
        private int QuadrantSize = 20;
        private int CurrentCount = 0;
        private int MinStraightRoad = 3;
        private int MaxStraightRoad = 6;
        private float ChanceToChange = 0.5f;
        private bool ManyCurves = false;


        // Werden durch den vorherigen Quadranten gesetzt
        private Vector2Int StartPoint;
        private EDirection StartDirection;

        // Wird Random gesetzt
        private Vector2Int EndPoint;
        private EDirection TargetDirection;

        private readonly Vector3 StartQuadrantPosition = Vector3.zero;

        private void Start()
        {
            // Get A new Target Point
            CreateRandomTargetPoint();

            // Create a new Quadrant
            SpawnQuadrant(this.StartQuadrantPosition);

            
            //StartAPath();
        }

        private void StartAPath()
        {
            // Create A* Grid to navigate through the Quadrant
            CreateAStarGrid();

            // Generate Block Map to block some tiles to get more interesting paths
            GenerateBlockMap();
            List<Vector2Int> road;
            do
            {
                road = FindPath(this.StartPoint, this.EndPoint);
                
                if (road == null)
                {
                    ResetQuadrant(this.StartQuadrantPosition);
                    SpawnQuadrant(this.StartQuadrantPosition);
                    ResetAStarGrid();
                    GenerateBlockMap();
                }
            } while (road == null);
             

            foreach (Vector2Int node in road)
            {
                Debug.Log("Node: " + node);
                Destroy(this.QuadrantTiles[node.y][node.x]);
                this.QuadrantTiles[node.y][node.x] = Instantiate(this.ROAD, new Vector3(node.x, 0, node.y), Quaternion.Euler(0, 0, 0));
            }
        }

        private void CreateRandomTargetPoint()
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
                }
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

            }
        }

        #region A* Pathfinding Anstz

        private void CreateAStarGrid()
        {
            for (int i = 0; i < this.QuadrantSize; i++)
            {
                this.QuadrantNodes.Add(new List<Node>());

                for (int j = 0; j < this.QuadrantSize; j++)
                {
                    this.QuadrantNodes[i].Add(new Node(ENodeState.Open, new Vector2Int(j, i)));
                }
            }
        }

        private void ResetAStarGrid()
        {
            for (int i = 0; i < this.QuadrantSize; i++)
            {
                for (int j = 0; j < this.QuadrantSize; j++)
                {
                    this.QuadrantNodes[i][j].SetTileType(ENodeState.Open);
                    this.QuadrantNodes[i][j].SetGCost(1000000);
                    this.QuadrantNodes[i][j].SetHCost(1000000);
                    this.QuadrantNodes[i][j].SetParent(null);
                }
            }
        }

        private void GenerateBlockMap(int blockMinSize = 1, int blockMaxSize = 3)
        {
            // Anzahl der Blöcke
            int numBlocks = Random.Range(1, Mathf.RoundToInt((this.QuadrantSize * this.QuadrantSize) / ((blockMaxSize * blockMaxSize))));

            // Erstelle Blöcke
            for (int i = 0; i < numBlocks; i++)
            {
                int rowStart = Random.Range(0, this.QuadrantSize - blockMaxSize + 1);
                int colStart = Random.Range(0, this.QuadrantSize - blockMaxSize + 1);

                int blockWidth = Random.Range(blockMinSize, blockMaxSize + 1);
                int blockHeight = Random.Range(blockMinSize, blockMaxSize + 1);

                // Setze Zellen im Block zu Block
                for (int row = rowStart; row < rowStart + blockHeight; row++)
                {
                    for (int col = colStart; col < colStart + blockWidth; col++)
                    {
                        Destroy(this.QuadrantTiles[row][col]);
                        this.QuadrantTiles[row][col] = Instantiate(this.BLOCK, new Vector3(col, 0, row), Quaternion.Euler(0, 0, 0));
                        this.QuadrantNodes[row][col].SetTileType(ENodeState.Blocked);
                    }
                }
            }
        }

        private List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            Node startNode = this.QuadrantNodes[start.y][start.x];
            Node endNode = this.QuadrantNodes[end.y][end.x];

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            startNode.SetGCost(0);
            startNode.SetHCost(GetManhattanDistance(startNode, endNode));
            startNode.SetParent(null);

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                foreach (Node node in openSet)
                {
                    if (node.GetFCost() < currentNode.GetFCost()) currentNode = node;
                }

                openSet.Remove(currentNode);

                if (IsEndNode(currentNode))
                {
                    Debug.Log("Path found");
                    return ReconstructPath(currentNode);
                }

                List<Node> neighbours = GetOpenNeighbours(currentNode);

                // Check all neighbours of the current node
                foreach (Node neighbor in neighbours)
                {
                    int tentativeGCost = currentNode.GetGCost() + GetManhattanDistance(currentNode, neighbor);

                    if (!openSet.Contains(neighbor) || tentativeGCost < neighbor.GetGCost())
                    {
                        neighbor.SetParent(currentNode);
                        neighbor.SetGCost(tentativeGCost);
                        neighbor.SetHCost(GetManhattanDistance(neighbor, endNode));

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }

                currentNode.SetTileType(ENodeState.Closed);
                closedSet.Add(currentNode);
            }

            return null;
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
            return path;

        }

        List<Node> GetOpenNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            // North
            if (node.Position.y - 1 >= 0)
            {
                Node n = this.QuadrantNodes[node.Position.y - 1][node.Position.x];
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // South
            if (node.Position.y + 1 < this.QuadrantSize)
            {
                Node n = this.QuadrantNodes[node.Position.y + 1][node.Position.x];
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // West
            if (node.Position.x - 1 >= 0)
            {
                Node n = this.QuadrantNodes[node.Position.y][node.Position.x - 1];
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // East
            if (node.Position.x + 1 < this.QuadrantSize)
            {
                Node n = this.QuadrantNodes[node.Position.y][node.Position.x + 1];
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            return neighbours;
        }

        private bool IsNodeOpen(Node node)
        {
            return node.TileType == ENodeState.Open;
        }

        private int GetManhattanDistance(Node a, Node b)
        {
            int x = Mathf.Abs(a.Position.x - b.Position.x);
            int y = Mathf.Abs(a.Position.y - b.Position.y);

            return x + y;

        }

        private bool IsEndNode(Node node)
        {
            return node.Position == this.EndPoint;
        }

        #endregion

        
    }
}