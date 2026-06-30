using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WHeuristics
{
    public static float straightCost = 1f;
    public static float diagonalCost = 1.4142f;


    public static float Manhattan(WNode a, WNode b)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return straightCost * (dx + dy);
    }


    public static float Euclidean(WNode a, WNode b)
    {
        return Vector2Int.Distance(a.gridPosition, b.gridPosition);
    }


    public static float Diagonal(WNode a, WNode b)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return diagonalCost * Mathf.Min(dx, dy) + straightCost * Mathf.Abs(dx - dy);
    }

    public static float ManhattanWeighted(WNode a, WNode b, float minTerrainCost)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

        return straightCost * (dx + dy) * minTerrainCost;
    }

    public static float DiagonalWeighted(WNode a, WNode b, float minTerrainCost)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

        float baseCost = diagonalCost * Mathf.Min(dx, dy)
                       + straightCost * Mathf.Abs(dx - dy);

        return baseCost * minTerrainCost;
    }
}
