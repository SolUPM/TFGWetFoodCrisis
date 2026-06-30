using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class WTheta : WPathfindingBase
{

    [Header("Parámetros")]
    public bool allowDiagonals = true;

    [Header("Pesos")]
    public bool useTerrainCost = true;
    public float minTerrainCost = 1f;


    public override List<WNode> FindPath(WNode startNode, WNode targetNode, WNode[,] grid)
    {
        if (nHandler == null)
        {
            UnityEngine.Debug.LogError("Theta* necesita GridManager.");
            return null;
        }

        if (startNode == null || targetNode == null || !startNode.walkable || !targetNode.walkable)
        {
            UnityEngine.Debug.LogWarning("Nodo inválido.");
            return null;
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        int nodesAnalizados = 0;

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
            WNode current = GetLowestFCost(openList);
            openList.Remove(current);
            closedList.Add(current);

            nodesAnalizados++;

            if (current == targetNode)
            {
                stopwatch.Stop();

                List<WNode> path = RetracePath(startNode, targetNode);

                float thetaCost = current.gCost;
                float realCost = CalculateRealPathCost(path, grid);
                //float terrainCostOnly = CalculateTerrainCostOnly(path, grid);

                string prefix = useTerrainCost ? "[Theta* Weighted] " : "[Theta* Classic] ";

                UnityEngine.Debug.Log(prefix + $"Path found. Nodes analyzed: {nodesAnalizados} | Path nodes: {path.Count}");
                UnityEngine.Debug.Log(prefix + $"Theta* Cost: {thetaCost}");
                UnityEngine.Debug.Log(prefix + $"Real Cost: {realCost}");
                //UnityEngine.Debug.Log(prefix + $"Terrain Cost Only: {terrainCostOnly}");


                return path;
            }

            foreach (WNode neighbour in nHandler.GetNeighbours(current, grid))
            {
                if (!neighbour.walkable || closedList.Contains(neighbour))
                    continue;

                Vector2Int cPos = current.gridPosition;
                Vector2Int nPos = neighbour.gridPosition;

                int dx = nPos.x - cPos.x;
                int dy = nPos.y - cPos.y;

                //  NO PERMITIR CORNER CUTTING (como tu referencia)
                if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1)
                {
                    WNode side1 = nHandler.GetNodeAt(new Vector2Int(cPos.x + dx, cPos.y), grid);
                    WNode side2 = nHandler.GetNodeAt(new Vector2Int(cPos.x, cPos.y + dy), grid);

                    if ((side1 != null && !side1.walkable) || (side2 != null && !side2.walkable))
                        continue;
                }

                if (!openList.Contains(neighbour))
                    openList.Add(neighbour);

                WNode parent = current.parent ?? current;

                float newG;

                if (LineOfSight(parent, neighbour, grid))
                {
                    newG = parent.gCost + (useTerrainCost
                        ? ComputeLineCostReal(parent, neighbour, grid)
                        : ComputeLineCostClassic(parent, neighbour));
                }
                else
                {
                    newG = current.gCost + (useTerrainCost
                        ? ComputeLineCostReal(current, neighbour, grid)
                        : ComputeLineCostClassic(current, neighbour));
                }

                if (newG < neighbour.gCost)
                {
                    neighbour.parent = LineOfSight(parent, neighbour, grid) ? parent : current;
                    neighbour.gCost = newG;
                    neighbour.hCost = GetHeuristic(neighbour, targetNode);
                }
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.LogWarning(
            (useTerrainCost ? "[Theta* Weighted] " : "[Theta* Classic] ") +
            $"No path found. Nodes analyzed: {nodesAnalizados} | Time: {stopwatch.ElapsedMilliseconds} ms"
        );

        return null;
    }

    // ------------------------------
    // HEURÍSTICA
    // ------------------------------
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

    // ------------------------------
    // COSTE CLÁSICO (Theta* original)
    // ------------------------------
    private float ComputeLineCostClassic(WNode a, WNode b)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

        float stepCost = (dx == 1 && dy == 1)
            ? WHeuristics.diagonalCost
            : WHeuristics.straightCost;

        return stepCost + (useTerrainCost ? b.terrainCost : 0f);
    }

    // ------------------------------
    // COSTE REAL (todos los nodos intermedios)
    // ------------------------------
    private float ComputeLineCostReal(WNode a, WNode b, WNode[,] grid)
    {
        Vector2Int posA = a.gridPosition;
        Vector2Int posB = b.gridPosition;

        int x0 = posA.x;
        int y0 = posA.y;
        int x1 = posB.x;
        int y1 = posB.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        float totalCost = 0f;

        while (true)
        {
            WNode current = nHandler.GetNodeAt(new Vector2Int(x0, y0), grid);
            if (current == null || !current.walkable)
                return Mathf.Infinity;

            if (!(x0 == posA.x && y0 == posA.y))
            {
                bool diagonal = (x0 - posA.x != 0) && (y0 - posA.y != 0);
                totalCost += diagonal ? WHeuristics.diagonalCost : WHeuristics.straightCost;

                if (useTerrainCost)
                    totalCost += current.terrainCost;
            }

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }

        return totalCost;
    }

    // ------------------------------
    // COSTE REAL DEL CAMINO FINAL
    // ------------------------------
    private float CalculateRealPathCost(List<WNode> path, WNode[,] grid)
    {
        float total = 0f;

        for (int i = 1; i < path.Count; i++)
            total += ComputeLineCostReal(path[i - 1], path[i], grid);

        return total;
    }

    // ------------------------------
    // COSTE SOLO DE TERRENO (debug extra)
    // ------------------------------
    /*
    private float CalculateTerrainCostOnly(List<WNode> pat)
    {
        float total = 0f;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int posA = path[i - 1].gridPosition;
            Vector2Int posB = path[i].gridPosition;

            int x0 = posA.x;
            int y0 = posA.y;
            int x1 = posB.x;
            int y1 = posB.y;

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = dx - dy;

            while (true)
            {
                WNode current = nHandler.GetNodeAt(new Vector2Int(x0, y0));

                if (current == null || !current.walkable)
                    return Mathf.Infinity;

                if (!(x0 == posA.x && y0 == posA.y))
                    total += current.terrainCost;

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        return total;
    }*/

    // ------------------------------
    // LINE OF SIGHT (con anti-corner-cutting)
    // ------------------------------
    private bool LineOfSight(WNode a, WNode b, WNode[,] grid)
    {
        Vector2Int posA = a.gridPosition;
        Vector2Int posB = b.gridPosition;

        int x0 = posA.x;
        int y0 = posA.y;
        int x1 = posB.x;
        int y1 = posB.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            WNode node = nHandler.GetNodeAt(new Vector2Int(x0, y0), grid);
            if (node == null || !node.walkable)
                return false;

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;

            int oldX = x0;
            int oldY = y0;

            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }

            //  Anti-corner-cutting en la línea
            if (x0 != oldX && y0 != oldY)
            {
                WNode n1 = nHandler.GetNodeAt(new Vector2Int(x0, oldY), grid);
                WNode n2 = nHandler.GetNodeAt(new Vector2Int(oldX, y0), grid);

                if ((n1 != null && !n1.walkable) || (n2 != null && !n2.walkable))
                    return false;
            }
        }

        return true;
    }

    private WNode GetLowestFCost(List<WNode> nodes)
    {
        WNode best = nodes[0];
        float bestCost = best.fCost;

        foreach (WNode n in nodes)
        {
            if (n.fCost < bestCost)
            {
                best = n;
                bestCost = n.fCost;
            }
        }
        return best;
    }
}
