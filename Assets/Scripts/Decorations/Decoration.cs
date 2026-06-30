using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Decoration", menuName = "Decoration", order = 0)]
public class Decoration : ScriptableObject
{
    public string decID;

    public float weight = 5;

    [Range(0f, 1f)] public float minTemp = 0f;
    [Range(0f, 1f)] public float maxTemp = 1f;

    public GameObject decoration;

    public List<Tile> compatibleTiles = new List<Tile>();


    public bool isInTempRange(float temp)
    {
        return temp >= minTemp && temp <= maxTemp;
    }
}

