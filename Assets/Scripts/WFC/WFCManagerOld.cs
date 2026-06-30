using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCManagerOld : MonoBehaviour
{
    [SerializeField]
    public int height = 20;
    [SerializeField]
    public int width = 20;
    public Vector2 start = Vector2.zero;
    private Tile [,] grid;
    
    public List<Tile> Tiles = new List<Tile>();
    
    private List<Vector2Int> toCollapse = new List<Vector2Int>();

    private Vector2Int[] directions = {
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0)
    };

    private void Start() {
        startWFC();
    }

    public void startWFC()
    {
        grid = new Tile[width,height];

        CollapseWFC();
    }


    private void CollapseWFC()
    {
        toCollapse.Clear();

        toCollapse.Add(new Vector2Int(width/2,height/2));

        Vector2Int currentCollapse = toCollapse[0];

        while(toCollapse.Count > 0)
        {
        
            int x = currentCollapse.x;
            int y = currentCollapse.y;
            if(grid[x, y] != null)
            {
                toCollapse.Remove(currentCollapse);

                if (!TryGetLowestEntropy(toCollapse, out currentCollapse))
                {
                    break;
                }
                continue;
            }


            List<Tile> potentialTiles = new List<Tile>(Tiles);

            for(int i = 0; i < directions.Length; i++)
            {
                
                Vector2Int neighbourPos = new Vector2Int(x + directions[i].x, y + directions[i].y);

                if (inGrid(neighbourPos))
                {
                    Tile neighbourTile = grid[neighbourPos.x, neighbourPos.y];

                    if(neighbourTile != null)
                    {
                        //Directions = UP, DOWN, RIGHT, LEFT --- then compatible DOWN UP LEFT RIGHT
                         switch (i)
                        {
                            case 0:
                                removeNonValid(potentialTiles, neighbourTile.compatibleTilesDown);
                                break;
                            case 1:
                                removeNonValid(potentialTiles, neighbourTile.compatibleTilesUp);
                                break;
                            case 2:
                                removeNonValid(potentialTiles, neighbourTile.compatibleTilesLeft);
                                break;
                            case 3:
                                removeNonValid(potentialTiles, neighbourTile.compatibleTilesRight);
                                break;
                            
                        }
                    }
                    else
                    {
                        if(!toCollapse.Contains(neighbourPos)) toCollapse.Add(neighbourPos);
                    }
                }
            }

            if(potentialTiles.Count < 1)
            {
                Debug.LogError(
                   $"Contradiction at ({x},{y}). " +
                   $"Neighbours:\n" +
                   $"Up: {GetTileName(x, y + 1)}\n" +
                   $"Down: {GetTileName(x, y - 1)}\n" +
                   $"Left: {GetTileName(x - 1, y)}\n" +
                   $"Right: {GetTileName(x + 1, y)}"
                );  
                PlaceTile(x, y, Tiles[0]); //grid[x,y] = Tiles[0];
            }
            else
            {
                Tile chosen = chooseWeightedRandomTile(potentialTiles);
                PlaceTile(x, y, chosen); //grid[x,y] = chosen;
            }

            toCollapse.Remove(currentCollapse);

            toCollapse.RemoveAll(pos => grid[pos.x, pos.y] != null);

            if (!TryGetLowestEntropy(toCollapse, out currentCollapse))
            {
                break;
            }
        }
    }

    public void PlaceTile(int x, int y, Tile type)
    {
        grid[x,y] = type;
        GameObject g = new GameObject();
        SpriteRenderer sprite = g.AddComponent<SpriteRenderer>() as SpriteRenderer;
        //sprite.sprite = type.sprite; OLD SPRITE SYSTEM
        Instantiate(g, new Vector2(x, y), Quaternion.identity);
    }

    private bool TryGetLowestEntropy(List<Vector2Int> list, out Vector2Int coord)
    {
        int lowestEntr = int.MaxValue;
        coord = Vector2Int.zero;
        bool found = false;

        foreach (Vector2Int pos in list)
        {
            int entropy = getEntropy(pos);
            if (entropy < lowestEntr)
            {
                lowestEntr = entropy;
                coord = pos;
                found = true;
            }
        }

        return found;
    }


    private Tile chooseWeightedRandomTile(List<Tile> tiles)
    {
        float rollingWeight = 0;
        float[] maxEndWeight = new float[tiles.Count];
        if(tiles.Count == 1)
        {
            return tiles[0];
        }
        
        for(int i = 0; i < tiles.Count; i++)
        {
            rollingWeight += tiles[i].weight;
            maxEndWeight[i] = rollingWeight;
        }
        if (rollingWeight <= 0)
        {
            Debug.LogWarning("weight is 0 wtf");
            return tiles[0];
        }
        float randomFloat = Random.Range(0, rollingWeight);

        for(int i = 0; i < tiles.Count; i++)
        {
            if(randomFloat < maxEndWeight[i])
            {
                return tiles[i];
            }
        }

        Debug.LogWarning("WARNING: WEIGHTED RANDOM DIDNT WORK");
        return tiles[0];
    }

    private void removeNonValid(List<Tile> potentialTiles, List<Tile> validTiles)
    {
        for(int i = potentialTiles.Count - 1; i > -1; i--){
            if (!validTiles.Contains(potentialTiles[i]))
            {
                potentialTiles.RemoveAt(i);
            }
        }
    }

    private bool inGrid(Vector2Int coords)
    {
        return coords.x > -1 && coords.x < width && coords.y > -1 && coords.y < height;
    }

    private int getEntropy(Vector2Int coords)
    {
        if(grid[coords.x, coords.y] != null)
        {
            //Theres a tile here so never choose this
            return int.MaxValue;
        }

        List<Tile> potentialTiles = new List<Tile>(Tiles);

        for(int i = 0; i<directions.Length ; i++)
        {
            Vector2Int pos = coords + directions[i];
            if(!inGrid(pos)) continue; //if neighour is OOB, SKIP!!!

            Tile posTile = grid[pos.x, pos.y];
            if(posTile == null) continue; //if neighbour is empty, SKIP!!!

            //Directions = UP, DOWN, RIGHT, LEFT --- then compatible DOWN UP LEFT RIGHT
            switch (i)
            {
                case 0:
                    removeNonValid(potentialTiles, posTile.compatibleTilesDown);
                    break;
                case 1:
                    removeNonValid(potentialTiles, posTile.compatibleTilesUp);
                    break;
                case 2:
                    removeNonValid(potentialTiles, posTile.compatibleTilesLeft);
                    break;
                case 3:
                    removeNonValid(potentialTiles, posTile.compatibleTilesRight);
                    break;
                
            }
        }

        return potentialTiles.Count;
    }

    string GetTileName(int x, int y)
    {
        if (!inGrid(new Vector2Int(x, y))) return "OOB";
        return grid[x, y] ? grid[x, y].name : "Empty";
    }
}
