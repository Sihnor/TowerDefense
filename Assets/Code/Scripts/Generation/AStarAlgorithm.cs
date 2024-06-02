using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Code.Scripts.Enums;
using Random = UnityEngine.Random;

namespace Code.Scripts.Generation
{
    internal enum EWeightTypeNew
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
            EWeightTypeNew type = (EWeightTypeNew)randomFigure;

            return type switch
            {
                EWeightTypeNew.Cross => CreateCrossWeightMap(size),
                EWeightTypeNew.Square => CreateSquareWeightMap(size),
                EWeightTypeNew.Field => CreateFieldWeightMap(size),
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

    public static class AStarAlgorithm
    {
        /// <summary>
        /// Create the target points for the pathfinding algorithm
        /// </summary>
        /// <param name="quadrantSize"></param>
        /// <returns></returns>
        private static List<Vector2Int> CreateTargetPoints(int quadrantSize)
        {
            List<Vector2Int> TargetPoints = new List<Vector2Int>();

            int justified = 1 + (quadrantSize / 5);
            int numTargets = (justified >= quadrantSize / 2) ? justified : Random.Range(justified, quadrantSize / 2);
            numTargets = 3;
            bool ignoreDistance = numTargets == justified;

            // First Target
            TargetPoints!.Add(new Vector2Int(Random.Range(1, quadrantSize - 2), Random.Range(2, quadrantSize - 2)));

            while (TargetPoints.Count < numTargets)
            {
                Vector2Int target = new Vector2Int(Random.Range(2, quadrantSize - 2), Random.Range(2, quadrantSize - 2));
                bool add = true;

                foreach (Vector2Int point in TargetPoints.Where(point => Vector2Int.Distance(target, point) < 3 && !ignoreDistance))
                {
                    add = false;
                }

                if (add) TargetPoints.Add(target);
            }

            return TargetPoints;
        }

        /// <summary>
        /// Create the weight points for the pathfinding algorithm
        /// </summary>
        /// <param name="quadrantSize"></param>
        /// <returns></returns>
        private static List<Vector2Int> CreateWeightPoints(int quadrantSize)
        {
            List<Vector2Int> weightPoints = new List<Vector2Int>();

            // Corners of the Quadrant
            weightPoints.Add(new Vector2Int(1, 1));
            weightPoints.Add(new Vector2Int(1, quadrantSize - 2));
            weightPoints.Add(new Vector2Int(quadrantSize - 2, 1));
            weightPoints.Add(new Vector2Int(quadrantSize - 2, quadrantSize - 2));

            // Middle inside corners
            weightPoints.Add(new Vector2Int(quadrantSize / 2, 1));
            weightPoints.Add(new Vector2Int(quadrantSize / 2, quadrantSize - 2));
            weightPoints.Add(new Vector2Int(1, quadrantSize / 2));
            weightPoints.Add(new Vector2Int(quadrantSize - 2, quadrantSize / 2));

            const int startInnerSquare = 3;
            int endInnerSquare = quadrantSize - 3;

            if (endInnerSquare - startInnerSquare <= 3) return weightPoints;

            // Random Points
            for (int i = 0; i < quadrantSize - 6; i++)
            {
                weightPoints.Add(new Vector2Int(Random.Range(startInnerSquare, endInnerSquare), Random.Range(startInnerSquare, endInnerSquare)));
            }

            return weightPoints;
        }

        /// <summary>
        /// Bake the weight map for the pathfinding algorithm with random figures
        /// </summary>
        /// <param name="quadrantTiles"></param>
        /// <param name="quadrantSize"></param>
        /// <param name="weightPointsCenter"></param>
        private static void BakeWeightMap(IReadOnlyList<List<GameObject>> quadrantTiles, int quadrantSize, List<Vector2Int> weightPointsCenter)
        {
            foreach (Vector2Int center in weightPointsCenter)
            {
                List<Vector2Int> weightStructures = WeightStruct.GetRandomFigure(Random.Range(3, quadrantSize / 2));

                foreach (Vector2Int weightPoint in weightStructures)
                {
                    int x = center.x + weightPoint.x;
                    int y = center.y + weightPoint.y;

                    if (x < 0 || x >= quadrantSize || y < 0 || y >= quadrantSize) continue;

                    quadrantTiles[y][x].GetComponent<Node>().IncreaseWeight(Random.value * 3);
                }
            }
        }

        /// <summary>
        /// Create the first path for the pathfinding algorithm for Start-Quadrant
        /// </summary>
        /// <param name="quadrantTiles"></param>
        /// <param name="quadrantSize"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static List<Vector2Int> CreateFirstPath(List<List<GameObject>> quadrantTiles, int quadrantSize, Vector2Int startPoint, Vector2Int endPoint)
        {
            List<Vector2Int> roadPath = FindPath(quadrantTiles, quadrantSize, startPoint, endPoint);

            return roadPath;
        }

        /// <summary>
        /// Create the path for the pathfinding algorithm for every other Quadrant
        /// </summary>
        /// <param name="quadrantTiles"></param>
        /// <param name="quadrantSize"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public static List<Vector2Int> CreatePath(List<List<GameObject>> quadrantTiles, int quadrantSize, Vector2Int startPoint, Vector2Int endPoint)
        {
            List<Vector2Int> roadPath = new List<Vector2Int>();
            
            BakeWeightMap(quadrantTiles, quadrantSize, CreateWeightPoints(quadrantSize));
            
            bool recalculatePath;
            do
            {
                recalculatePath = false;
                roadPath.Clear();
                ClearPathfindingData(quadrantTiles);
                BlockRoadPath(quadrantTiles, roadPath);

                List<Vector2Int> targetPoints = CreateTargetPoints(quadrantSize);
                
                for (int i = 0; i < targetPoints.Count; i++)
                {
                    List<Vector2Int> newRoadPath;

                    if (i == 0)
                    {
                        newRoadPath = FindPath(quadrantTiles, quadrantSize, startPoint, targetPoints[i]);
                    }
                    else if (i == targetPoints.Count - 1)
                    {
                        newRoadPath = FindPath(quadrantTiles, quadrantSize, targetPoints[i - 1], endPoint);
                    }
                    else
                    {
                        newRoadPath = FindPath(quadrantTiles, quadrantSize, targetPoints[i - 1], targetPoints[i]);
                    }

                    if (newRoadPath == null)
                    {
                        recalculatePath = true;
                        break;
                    }                   
                    
                    roadPath.AddRange(newRoadPath);
                }
                
            } while (recalculatePath);
            
            return roadPath;
        }

        /// <summary>
        /// Find the path between the start and end point inside the quadrant via the A* algorithm
        /// </summary>
        /// <param name="quadrantTiles"></param>
        /// <param name="quadrantSize"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static List<Vector2Int> FindPath(IReadOnlyList<List<GameObject>> quadrantTiles, int quadrantSize, Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> roadPath = new List<Vector2Int>();
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            
            Node startNode = quadrantTiles[start.x][start.y].GetComponent<Node>();
            Node endNode = quadrantTiles[end.x][end.y].GetComponent<Node>();

            startNode.SetWeight(1);

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                // Find the node with the lowest F-Cost
                foreach (Node node in openSet.Where(node => node.GetFCost() < currentNode.GetFCost()))
                {
                    currentNode = node;
                }

                openSet.Remove(currentNode);

                // Check if the current node is the end node, target reached
                if (IsEndNode(currentNode, endNode))
                {
                    roadPath.AddRange(ReconstructPath(currentNode));
                    currentNode.SetTileType(ENodeState.Closed);
                    return roadPath;
                }

                List<Node> neighbours = GetOpenNeighbours(quadrantTiles, quadrantSize, currentNode);

                // Calculate the G-Cost, H-Cost and set the parent for the neighbours
                foreach (Node neighbour in neighbours)
                {
                    float tentativeGCost = currentNode.GetGCost() + neighbour.GetCostToEnter();

                    if (openSet.Contains(neighbour) && !(tentativeGCost < neighbour.GetGCost())) continue;

                    neighbour.SetParent(currentNode);
                    neighbour.SetGCost(tentativeGCost);
                    neighbour.SetHCost(CalculateManhattanDistanceWithWeights(quadrantTiles, neighbour, endNode));

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }

                currentNode.SetTileType(ENodeState.Closed);
                closedSet.Add(currentNode);
            }

            return null;
        }

        /// <summary>
        /// Get the open neighbours of the current node inside the quadrant (only horizontal and vertical)
        /// </summary>
        /// <param name="quadrantTiles"></param>
        /// <param name="quadrantSize"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static List<Node> GetOpenNeighbours(IReadOnlyList<List<GameObject>> quadrantTiles, int quadrantSize, Node node)
        {
            if (quadrantSize <= 0) throw new ArgumentOutOfRangeException(nameof(quadrantSize));
            List<Node> neighbours = new List<Node>();

            // North
            if (node.Position.x - 1 >= 0)
            {
                Node n = quadrantTiles[node.Position.x - 1][node.Position.y].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // South
            if (node.Position.x + 1 < quadrantSize)
            {
                Node n = quadrantTiles[node.Position.x + 1][node.Position.y].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // West
            if (node.Position.y - 1 >= 0)
            {
                Node n = quadrantTiles[node.Position.x][node.Position.y - 1].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            // East
            if (node.Position.y + 1 < quadrantSize)
            {
                Node n = quadrantTiles[node.Position.x][node.Position.y + 1].GetComponent<Node>();
                if (IsNodeOpen(n)) neighbours.Add(n);
            }

            return neighbours;

            bool IsNodeOpen(Node checkNode) => checkNode.TileType == ENodeState.Open;
        }

        /// <summary>
        /// Calculate the Manhattan distance between two nodes with weights inside the quadrant (only for the A* algorithm)
        /// </summary>
        /// <param name="quadrantTiles"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static float CalculateManhattanDistanceWithWeights(IReadOnlyList<List<GameObject>> quadrantTiles, Node a, Node b)
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

            return indexes.Sum(index => quadrantTiles[index[1]][index[0]].GetComponent<Node>().GetWeight());
        }

        /// <summary>
        /// Check if the current node is the end node via the position
        /// </summary>
        /// <param name="node"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        private static bool IsEndNode(Node node, Node endNode)
        {
            return node.Position == endNode.Position;
        }

        /// <summary>
        /// Reconstruct the path from the end node to the start node
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private static IEnumerable<Vector2Int> ReconstructPath(Node currentNode)
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

        /// <summary>
        /// Clear the pathfinding data from the nodes
        /// </summary>
        private static void ClearPathfindingData(IEnumerable<List<GameObject>> quadrantTiles)
        {
            foreach (List<GameObject> tiles in quadrantTiles)
            {
                foreach (GameObject tile in tiles)
                {
                    tile.GetComponent<Node>().SetTileType(ENodeState.Open);
                }
            }
        }
        
        /// <summary>
        /// Block the tiles of the road path
        /// </summary>
        /// <param name="quadrantTiles"></param>
        /// <param name="roadPath"></param>
        private static void BlockRoadPath(IReadOnlyList<List<GameObject>> quadrantTiles, List<Vector2Int> roadPath)
        {
            foreach (Vector2Int path in roadPath)
            {
                quadrantTiles[path.x][path.y].GetComponent<Node>().SetTileType(ENodeState.Closed);
            }
        }
    }
}