using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseWaveCreator : MonoBehaviour
{
    [SerializeField]
    public int height = 20;
    [SerializeField]
    public int width = 20;

    public scTile[][] grid;

    public List<scTile> debugGrid;

    public List<Tile> allTiles;


    // Start is called before the first frame update
    void Start()
    {
        foreach(scTile t in debugGrid)
        {
            t.possibleTiles = new List<Tile>(allTiles);
            t.fallbackAllTiles = new List<Tile>(allTiles);

        }
        foreach (scTile t in debugGrid)
        {
            t.InitializeVisuals(allTiles.Count);
            t.UpdateVisuals();

        }

        debugGrid[0].showPossibilities();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
