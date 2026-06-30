using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 5;
    public int height = 5;

    public GameObject tilePrefab;
    public Tile[] allTiles;

    private scTile[,] grid;

    void Start()
    {
        GenerateGrid();
        InitializeWFC();
    }

    void GenerateGrid()
    {
        grid = new scTile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                tileObj.name = $"Tile_{x}_{y}";
                tileObj.transform.parent = transform;

                scTile tile = tileObj.GetComponent<scTile>();
                grid[x, y] = tile;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                scTile tile = grid[x, y];

                tile.nbLEFT = (x > 0) ? grid[x - 1, y] : null;
                tile.nbRIGHT = (x < width - 1) ? grid[x + 1, y] : null;
                tile.nbDOWN = (y > 0) ? grid[x, y - 1] : null;
                tile.nbUP = (y < height - 1) ? grid[x, y + 1] : null;
            }
        }
    }

    void InitializeWFC()
    {
        foreach (scTile t in grid)
        {
            t.possibleTiles = new System.Collections.Generic.List<Tile>(allTiles);
            t.InitializeVisuals(allTiles.Length);
            t.UpdateVisuals();
        }
    }
}