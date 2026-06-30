using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHandler : MonoBehaviour
{
    private WNode[,] grid;
    [HideInInspector]
    private int width, height;


    private void SetGrid(WNode[,] nodes)
    {
        grid = nodes;
        width = nodes.GetLength(0);
        height = nodes.GetLength(1);
    }

    public WNode GetNodeAt(Vector2Int pos, WNode[,] nodes)
    {
        SetGrid(nodes);
        if (grid == null) return null;
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) return null;
        return grid[pos.x, pos.y];
    }
    private WNode GetNodeAt(Vector2Int pos)
    {
        if (grid == null) return null;
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) return null;
        return grid[pos.x, pos.y];
    }

    public List<WNode> GetNeighbours(WNode node, WNode[,] nodes)
    {
        SetGrid(nodes);
        List<WNode> neighbours = new List<WNode>();

        Vector2Int[] directions =
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 1),
        };

        foreach (var dir in directions)
        {
            WNode neighbour = GetNodeAt(node.gridPosition + dir);
            if (neighbour != null)
                neighbours.Add(neighbour);
        }

        return neighbours;
    }


    

    public List<WNode> GetAllNodes(WNode[,] nodes)
    {
        SetGrid(nodes);
        List<WNode> allNodes = new List<WNode>();
        if (grid == null) return allNodes;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (grid[x, y] != null)
                    allNodes.Add(grid[x, y]);

        return allNodes;
    }
}
