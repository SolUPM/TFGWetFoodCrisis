using UnityEngine;

public class WNode
{
    public bool walkable;
    public Vector2Int gridPosition;
    public Vector3 worldPosition;

    public bool isJumpPoint;


    public float gCost;
    public float hCost;

    // Coste adicional del terreno según el tipo de casilla
    public float terrainCost;

    public float fCost => gCost + hCost;

    public WNode parent;

    // Dirección desde el padre (solo para JPS)
    public Vector2Int direction;

    public WNode(bool walkable, Vector2Int gridPos, Vector3 worldPos, float terrainCost)
    {
        this.walkable = walkable;
        this.gridPosition = gridPos;
        this.worldPosition = worldPos;

        this.gCost = Mathf.Infinity;
        this.hCost = 0f;
        this.parent = null;

        this.direction = Vector2Int.zero;
        this.isJumpPoint = false;

        this.terrainCost = terrainCost;
    }
}
