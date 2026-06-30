using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationPlacer : MonoBehaviour
{
    public Decoration[,] decorationGrid;
    public List<Decoration> decorations;
    public float emptyWeight = 100;
    public int noBlockablesRange = 5;
    public GameObject defendObject;


    void Start()
    {
        
    }

    public Decoration[,] placeDecorations(Tile[,] tiles, PerlinNoise perlin, GameObject[,] placedTiles)
    {
        List<Decoration> allowed;
        decorationGrid = new Decoration[tiles.GetLength(0), tiles.GetLength(1)];

        //Set middle of the grid
        int middleX = Mathf.RoundToInt(decorationGrid.GetLength(0) / 2), middleY = Mathf.RoundToInt(decorationGrid.GetLength(1) / 2);

        //Go through the Grid
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                //Reduce the allowed list into decorations that are possible with the tile and temperature that is in that spot.
                allowed = new List<Decoration>(decorations);
                Tile currentTile = tiles[i, j];
                float temp = perlin.temperature[i, j];
                for(int k = allowed.Count-1; k >= 0; k--)
                {
                    bool isAllowed = allowed[k].compatibleTiles.Contains(currentTile) && allowed[k].minTemp <= temp && allowed[k].maxTemp >= temp;
                    if (!isAllowed)
                    {
                        allowed.RemoveAt(k);
                    }
                }

                //Do not generate decoration if nearing defending point
                Decoration decorationChoice;

                if (i > middleX-noBlockablesRange && i < middleX+noBlockablesRange && j < middleY + noBlockablesRange && j > middleY - noBlockablesRange)
                {
                    decorationChoice = null;
                }else 
                    decorationChoice = chooseWeighted(allowed);


                if (decorationChoice != null)
                {
                    decorationGrid[i, j] = decorationChoice;
                    Instantiate(decorationChoice.decoration, placedTiles[i, j].transform);
                }

                //Add the defendable to the middle
                if(i == middleX && j == middleY)
                {
                    Instantiate(defendObject,new Vector3(i,j,0), Quaternion.identity);
                }
                
            }
        }

        return decorationGrid;
    }

    private Decoration chooseWeighted(List<Decoration> possibles)
    {
        float rollingWeight = 0;
        float[] maxEndWeight = new float[possibles.Count];
        if (possibles.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < possibles.Count; i++)
        {
            rollingWeight += possibles[i].weight;
            maxEndWeight[i] = rollingWeight;
        }
        if (rollingWeight <= 0)
        {
            Debug.LogWarning("weight is 0 wtf");
            return null;
        }
        rollingWeight += emptyWeight; //Adding the chance of no decoration to the list
        float randomFloat = Random.Range(0, rollingWeight);
        //Debug.Log("[" + maxEndWeight[maxEndWeight.Length - 1] + "], " + rollingWeight + " | " + randomFloat);

        for (int i = 0; i < possibles.Count; i++)
        {
            if (randomFloat < maxEndWeight[i])
            {
                return possibles[i];
            }
        }

        return null;
    }

}
