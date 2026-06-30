using System.Collections.Generic;
using UnityEngine;

public abstract class WPathfindingBase : MonoBehaviour
{
    [Header("Node Handler")]
    public NodeHandler nHandler;

    // ============================================================
    // MÉTODO PRINCIPAL
    // ============================================================

    public abstract List<WNode> FindPath(WNode startNode, WNode targetNode, WNode[,] grid);

    // ============================================================
    // RECONSTRUCCIÓN DEL CAMINO
    // ============================================================

    protected virtual List<WNode> RetracePath(WNode startNode, WNode endNode)
    {
        List<WNode> path = new List<WNode>();
        WNode currentNode = endNode;

        while (currentNode != null && currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        if (currentNode == startNode)
            path.Add(startNode);

        path.Reverse();
        return path;
    }

    // ============================================================
    // MÉTRICAS DE CALIDAD
    // ============================================================

   /* protected float ComputePathDistance(List<WNode> path)
    {
        if (path == null || path.Count < 2)
            return 0f;

        float total = 0f;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int a = path[i - 1].gridPosition;
            Vector2Int b = path[i].gridPosition;
            total += Vector2Int.Distance(a, b);
        }

        return total;
    }
   */

    // ============================================================
    // PESO MEDIO ADICIONAL DEL TERRENO
    // ============================================================
    /*
    protected float ComputeAverageTerrainExtra(List<WNode> path)
    {
        if (path == null || path.Count < 2)
            return 0f;

        float totalExtra = 0f;
        int count = 0;

        for (int i = 1; i < path.Count; i++)
        {
            float extra = path[i].terrainCost - 1f;
            if (extra > 0f)
            {
                totalExtra += extra;
                count++;
            }
        }

        if (count == 0)
            return 0f;

        return totalExtra / count;
    }
    */
    // ============================================================
    // LOGGING + REPETICIÓN
    // ============================================================
    /*
    protected void LogPathSuccess(
        string algorithmName,
        int nodesAnalizados,
        int nodesRecorridos,
        float totalDistance,
        float avgTerrainExtra,
        long timeMs)
    {
        Debug.Log(
            $"{algorithmName} | Path found. " +
            $"Nodes analyzed: {nodesAnalizados} " +
            $"Nodes in path: {nodesRecorridos} " +
            $"Total distance: {totalDistance:F3} " +
            $"Avg terrain extra: {avgTerrainExtra:F3} " +
            $"Time: {timeMs} ms");

        /*if (gridManager != null && gridManager.generationMode == WGenerationMode.Repeticion)
        {
            gridManager.RegisterPathResult(
                nodesAnalizados,
                totalDistance,
                avgTerrainExtra,
                timeMs
            );
        }
    }

    /*protected void LogPathFailure(
        string algorithmName,
        int nodesAnalizados,
        long timeMs)
    {
        Debug.LogWarning(
            $"{algorithmName} | No path found. " +
            $"Nodes analyzed: {nodesAnalizados} " +
            $"Time: {timeMs} ms");

        if (gridManager != null && gridManager.generationMode == WGenerationMode.Repeticion)
        {
            gridManager.RegisterPathFailure();
        }
    }
    */
    // ============================================================
    // DEBUG VISUAL
    // ============================================================

    private void OnDrawGizmos()
    {
        if (nHandler == null) return;

        List<WNode> path = null;
        WPathTester tester = FindObjectOfType<WPathTester>();
        if (tester != null)
        {
            var f = tester.GetType().GetField(
                "path",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (f != null)
                path = f.GetValue(tester) as List<WNode>;
        }

        if (path == null || path.Count < 2) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
        }
    }
}
