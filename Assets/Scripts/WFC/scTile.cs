using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//showcaseTile
public class scTile : MonoBehaviour
{
    [SerializeField]
    private Tile myTile;
    public scTile nbUP, nbDOWN, nbLEFT, nbRIGHT;

    public List<Tile> possibleTiles;

    [HideInInspector]
    public List<Tile> fallbackAllTiles;

    private bool imNotTile => myTile == null;

    private List<SpriteRenderer> possibilityRenderers = new List<SpriteRenderer>();

    public Tile tileSelection;


    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }

    public void showPossibilities()
    {
        if (imNotTile)
        {
            bool changed = findPossibles();

            UpdateVisuals();

            if (changed)
            {
                UpdateNeigbours();

            }
        }
        else
        {
            foreach (var sr in possibilityRenderers)
                sr.enabled = false;
        }
    }

    public void InitializeVisuals(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject g = new GameObject("possibleTile_" + i);
            g.transform.SetParent(transform);
            g.transform.localPosition = Vector2.zero;
            g.transform.localScale = Vector3.one;

            SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
            sr.enabled = false;

            possibilityRenderers.Add(sr);
        }
    }

    public void applyTile()
    {
        SetTile(tileSelection);
        UpdateNeigbours();
    }

    private void SetTile(Tile tile)
    {
        if (tile != null)
        {
            myTile = tile;


            possibleTiles.Clear();
            possibleTiles.Add(tile);

            GetComponent<SpriteRenderer>().sprite = tile.sprites[0];
        }
        else
        {
            Debug.Log("null tile!");
            myTile = null;

            possibleTiles = new List<Tile>(fallbackAllTiles);

            GetComponent<SpriteRenderer>().sprite = null;
        }

        UpdateVisuals();

        showPossibilities();
    }


    private void UpdateNeigbours()
    {
        if(nbUP != null)
            nbUP.showPossibilities();

        if (nbDOWN != null)
            nbDOWN.showPossibilities();

        if (nbLEFT != null)
            nbLEFT.showPossibilities();

        if (nbRIGHT != null)
            nbRIGHT.showPossibilities();

    }

    public void UpdateVisuals()
    {
        int count = possibleTiles.Count;

        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));

        float margin = 0.05f;
        float usableSize = 1f - margin * 2f;


        float subCellSize = usableSize / gridSize;


        for (int i = 0; i < possibilityRenderers.Count; i++)
        {
            if (i < count)
            {
                var sr = possibilityRenderers[i];
                sr.enabled = true;
                sr.sprite = possibleTiles[i].sprites[0];

                int row = i / gridSize;
                int col = i % gridSize;

                float offsetX = -0.5f + margin + col * subCellSize + subCellSize * 0.5f;
                float offsetY = -0.5f + margin + row * subCellSize + subCellSize * 0.5f;

                sr.transform.localPosition = new Vector2(offsetX, offsetY);
                sr.transform.localScale = Vector3.one * subCellSize * 0.93f;
            }
            else
            {
                possibilityRenderers[i].enabled = false;
            }
        }
    }

    private bool findPossibles()
    {
        int before = possibleTiles.Count;
        if (imNotTile)
        {
            if (nbUP != null)
            {
                if (nbUP.myTile != null)
                {
                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!nbUP.myTile.compatibleTilesDown.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ": Removing " + possibleTiles[i] + "because UP doesnt have it");
                            possibleTiles.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    List<Tile> sumPosibility = new List<Tile>();
                    foreach(Tile t in nbUP.possibleTiles)
                    {
                        foreach(Tile c in t.compatibleTilesDown)
                            if(!sumPosibility.Contains(c))
                                sumPosibility.Add(c);
                    }
                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!sumPosibility.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ":Removing " + possibleTiles[i] + "because UP POSSIBILITY doesnt have it");
                            possibleTiles.RemoveAt(i);
                        }
                    }
                }
            }
            if (nbDOWN != null)
            {
                if (nbDOWN.myTile != null)
                {
                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!nbDOWN.myTile.compatibleTilesUp.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ":Removing " + possibleTiles[i] + "because DOWN doesnt have it");
                            possibleTiles.RemoveAt(i);

                        }
                    }
                }
                else
                {
                    List<Tile> sumPosibility = new List<Tile>();
                    foreach (Tile t in nbDOWN.possibleTiles)
                    {
                        foreach (Tile c in t.compatibleTilesUp)
                            if (!sumPosibility.Contains(c))
                                sumPosibility.Add(c);
                    }
                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!sumPosibility.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ":Removing " + possibleTiles[i] + "because DOWN POSSIBILITY doesnt have it");
                            possibleTiles.RemoveAt(i);
                        }
                    }
                }
            }

            if (nbLEFT != null)
            {
                if (nbLEFT.myTile != null)
                {

                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!nbLEFT.myTile.compatibleTilesRight.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ":Removing " + possibleTiles[i] + "because LEFT doesnt have it");
                            possibleTiles.RemoveAt(i);
                        }
                    }

                }
                else
                {
                    List<Tile> sumPosibility = new List<Tile>();
                    foreach (Tile t in nbLEFT.possibleTiles)
                    {
                        foreach (Tile c in t.compatibleTilesRight)
                            if (!sumPosibility.Contains(c))
                                sumPosibility.Add(c);
                    }
                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!sumPosibility.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ":Removing " + possibleTiles[i] + "because LEFT POSSIBILITY doesnt have it");
                            possibleTiles.RemoveAt(i);
                        }
                    }
                }
            }
            if (nbRIGHT != null)
            {
                if (nbRIGHT.myTile != null)
                {

                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!nbRIGHT.myTile.compatibleTilesLeft.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ":Removing " + possibleTiles[i] + "because RIGHT doesnt have it");
                            possibleTiles.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    List<Tile> sumPosibility = new List<Tile>();
                    foreach (Tile t in nbRIGHT.possibleTiles)
                    {
                        foreach (Tile c in t.compatibleTilesLeft)
                            if (!sumPosibility.Contains(c))
                                sumPosibility.Add(c);
                    }
                    for (int i = possibleTiles.Count - 1; i >= 0; i--)
                    {
                        if (!sumPosibility.Contains(possibleTiles[i]))
                        {
                            Debug.Log(name + ":Removing " + possibleTiles[i] + "because RIGHT POSSIBILITY doesnt have it");
                            possibleTiles.RemoveAt(i);
                        }
                    }
                }
            }
        }

        bool changed = possibleTiles.Count != before;
        return changed;
    }
}
