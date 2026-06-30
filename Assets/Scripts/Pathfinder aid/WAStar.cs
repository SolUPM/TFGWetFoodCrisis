using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class WAStar : WPathfindingBase
{

    [Header("Configuracion AStar")]
    public bool allowDiagonals = true;

    [Header("Weighted A*")]
    public bool useTerrainCost = true;

    [Header("Heuristic Settings")]
    public float minTerrainCost = 1f;

    public override List<WNode> FindPath(WNode startNode, WNode targetNode, WNode[,] grid)
    {
        if (startNode == null || targetNode == null || !startNode.walkable || !targetNode.walkable)
        {
            UnityEngine.Debug.LogWarning("Start or target node invalid or not walkable");
            return null;
        }

        int nodesAnalizados = 0;
        int nodesRecorridos = 0;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Reset nodes
        foreach (WNode n in nHandler.GetAllNodes(grid))
        {
            n.gCost = Mathf.Infinity;
            n.hCost = 0f;
            n.parent = null;
        }

        List<WNode> openList = new List<WNode>();
        HashSet<WNode> closedList = new HashSet<WNode>();

        startNode.gCost = 0f;
        startNode.hCost = GetHeuristic(startNode, targetNode);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            WNode currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost ||
                   (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            nodesAnalizados++;

            if (currentNode == targetNode)
            {
                stopwatch.Stop();

                List<WNode> path = RetracePath(startNode, targetNode);
                nodesRecorridos = path.Count;

                float totalCost = CalculatePathCost(path);

                UnityEngine.Debug.Log(
                    (useTerrainCost ? "[Weighted A*] " : "[A*] ") +
                    "Path found. Nodes analyzed: " + nodesAnalizados +
                    " | Nodes in path: " + nodesRecorridos +
                    " | Total Cost: " + totalCost +
                    " | Time: " + stopwatch.ElapsedMilliseconds + " ms");

                return path;
            }

            foreach (WNode neighbour in nHandler.GetNeighbours(currentNode, grid))
            {
                int dx = neighbour.gridPosition.x - currentNode.gridPosition.x;
                int dy = neighbour.gridPosition.y - currentNode.gridPosition.y;

                if (!allowDiagonals)
                {
                    if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1)
                        continue;
                }

                if (!neighbour.walkable || closedList.Contains(neighbour))
                    continue;

                // corner cutting control
                if (dx != 0 && dy != 0)
                {
                    WNode side1 = nHandler.GetNodeAt(
                        new Vector2Int(currentNode.gridPosition.x + dx, currentNode.gridPosition.y),
                        grid);

                    WNode side2 = nHandler.GetNodeAt(
                        new Vector2Int(currentNode.gridPosition.x, currentNode.gridPosition.y + dy),
                        grid);

                    bool side1Blocked = side1 == null || !side1.walkable;
                    bool side2Blocked = side2 == null || !side2.walkable;

                    if (side1Blocked && side2Blocked)
                        continue;

                    if (side1Blocked || side2Blocked)
                    {
                        WNode intermediate = side1Blocked ? side2 : side1;

                        if (intermediate != null && intermediate.walkable && !closedList.Contains(intermediate))
                        {
                            float intermediateCost = currentNode.gCost + WHeuristics.straightCost;

                            if (useTerrainCost)
                                intermediateCost += intermediate.terrainCost;

                            if (intermediateCost < intermediate.gCost || !openList.Contains(intermediate))
                            {
                                intermediate.gCost = intermediateCost;

                                intermediate.hCost = GetHeuristic(intermediate, targetNode);

                                intermediate.parent = currentNode;

                                if (!openList.Contains(intermediate))
                                    openList.Add(intermediate);
                            }
                        }

                        continue;
                    }
                }

                int dxStep = Mathf.Abs(dx);
                int dyStep = Mathf.Abs(dy);

                float stepCost = (dxStep == 1 && dyStep == 1)
                    ? WHeuristics.diagonalCost
                    : WHeuristics.straightCost;

                float newMovementCost = currentNode.gCost + stepCost;

                //  Weighted A*: añadimos el coste del terreno
                if (useTerrainCost)
                    newMovementCost += neighbour.terrainCost;

                if (newMovementCost < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCost;

                    neighbour.hCost = GetHeuristic(neighbour, targetNode);

                    neighbour.parent = currentNode;

                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }

        stopwatch.Stop();

        UnityEngine.Debug.LogWarning(
            (useTerrainCost ? "[Weighted A*] " : "[A*] ") +
            "No path found. Nodes analyzed: " + nodesAnalizados +
            " | Time: " + stopwatch.ElapsedMilliseconds + " ms");

        return null;
    }

    // HEURÍSTICA
    private float GetHeuristic(WNode a, WNode b)
    {
        if (useTerrainCost)
        {
            return allowDiagonals
                ? WHeuristics.DiagonalWeighted(a, b, minTerrainCost)
                : WHeuristics.ManhattanWeighted(a, b, minTerrainCost);
        }
        else
        {
            return allowDiagonals
                ? WHeuristics.Diagonal(a, b)
                : WHeuristics.Manhattan(a, b);
        }
    }

    // -----------------------------------------
    // Calcula el coste total del camino
    // -----------------------------------------
    private float CalculatePathCost(List<WNode> path)
    {
        float total = 0f;

        for (int i = 1; i < path.Count; i++)
        {
            WNode a = path[i - 1];
            WNode b = path[i];

            int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
            int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

            float stepCost = (dx == 1 && dy == 1)
                ? WHeuristics.diagonalCost
                : WHeuristics.straightCost;

            total += stepCost + b.terrainCost;
        }

        return total;
    }
}
