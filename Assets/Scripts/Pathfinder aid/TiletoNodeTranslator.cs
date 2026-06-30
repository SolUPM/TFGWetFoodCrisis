using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiletoNodeTranslator : MonoBehaviour
{
    /*
     * S = SNOW
     * G = GRASS
     * D = DESERT
     * L = LAKE
     * P = PATH
     * B = BLOCKED
     * X = IMPASSABLE
     * H = HALF LAKE
     * 
     */

    public List<WNode> nodes = new List<WNode>();

    public WNode[,] getNodes(Tile[,] grid, Decoration[,] decos, MovingUnit type)
    {
        WNode[,] gridList = new WNode[grid.GetLength(0), grid.GetLength(1)];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Decoration currentDeco = decos[i, j]; 
                Tile currentTile = grid[i, j];
                float cost = type.CostPerType[currentTile.tID];
                if(currentDeco != null && type.CostDecos.ContainsKey(currentDeco.decID))
                {
                    cost += type.CostDecos[currentDeco.decID];
                }
                WNode insertion = new WNode(cost >= 0, new Vector2Int(i, j), new Vector3(i, j, 0), cost);
                gridList[i, j] = insertion;
            }
        }
        return gridList;
    }
}
