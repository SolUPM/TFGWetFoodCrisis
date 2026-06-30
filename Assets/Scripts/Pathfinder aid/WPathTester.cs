using System.Collections.Generic;
using UnityEngine;

public class WPathTester : MonoBehaviour
{
    [Header("Referencias")]
    public NodeHandler nHandler;
    public WPathfindingBase pathfinder;
    public WNode[,] grid;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public bool ableToMove = true;


    private List<WNode> path;
    private int currentIndex = 0;

    /*void OnEnable()
    {
        if (gridManager != null && !gridManager.manualMode)
            gridManager.OnGridGenerated += StartPathfinding;
    }

    void OnDisable()
    {
        if (gridManager != null && !gridManager.manualMode)
            gridManager.OnGridGenerated -= StartPathfinding;
    }*/

    public virtual void MoveFrame()
    {
        // Movimiento del agente
        if (path != null && currentIndex < path.Count && ableToMove)
        {
            Vector3 targetPos = path[currentIndex].worldPosition;
            float weight = nHandler.GetNodeAt(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)), grid).terrainCost;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, (moveSpeed * Time.deltaTime) / weight);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                currentIndex++;
                if (currentIndex >= path.Count)
                    Debug.Log("Destino alcanzado.");
            }
        }

    }
    
    public bool StartPathfinding(Vector2Int toWhere, bool snapStart = true)
    {
        Debug.Log("PathTester.StartPathfinding() ejecutado");

        if (pathfinder == null || grid == null)
        {
            Debug.LogError("Faltan referencias en el inspector.");
            return false;
        }

        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        WNode startNode = nHandler.GetNodeAt(pos, grid);
        WNode endNode = nHandler.GetNodeAt(toWhere, grid);


        if (startNode == null || endNode == null)
        {
            Debug.LogError("Start o End node inválido.");
            return false;
        }

        path = pathfinder.FindPath(startNode, endNode, grid);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No se encontró un camino.");
            return false;
        }

        currentIndex = 0;
        Debug.Log("Camino encontrado con " + path.Count + " nodos.");
        if(snapStart)
            transform.position = path[0].worldPosition;
        return true;
    }

    void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.cyan;
        foreach (WNode node in path)
            Gizmos.DrawSphere(node.worldPosition, 0.2f);
    }
}
