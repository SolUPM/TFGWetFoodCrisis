using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Tile", menuName = "Tile", order = 0)]
public class Tile : ScriptableObject {

    public string tID;
    
    public float weight = 5;

    [Range(0f, 1f)] public float minTemp = 0f;
    [Range(0f, 1f)] public float maxTemp = 1f;

    //public Sprite sprite;
    public Sprite[] sprites;

    public List<Tile> compatibleTilesUp = new List<Tile>();
    public List<Tile> compatibleTilesDown = new List<Tile>();
    public List<Tile> compatibleTilesLeft = new List<Tile>();
    public List<Tile> compatibleTilesRight = new List<Tile>();
    
   private void OnValidate() {
        foreach(Tile tile in compatibleTilesUp){
            if(!tile.compatibleTilesDown.Contains(this)){
                tile.compatibleTilesDown.Add(this);
            }
        }
        foreach(Tile tile in compatibleTilesDown){
            if(!tile.compatibleTilesUp.Contains(this)){
                tile.compatibleTilesUp.Add(this);
            }
        }
        foreach(Tile tile in compatibleTilesLeft){
            if(!tile.compatibleTilesRight.Contains(this)){
                tile.compatibleTilesRight.Add(this);
            }
        }
        foreach(Tile tile in compatibleTilesRight){
            if(!tile.compatibleTilesLeft.Contains(this)){
                tile.compatibleTilesLeft.Add(this);
            }
        }
    }



    public bool isInTempRange(float temp)
    {
        return temp >= minTemp && temp <= maxTemp;
    }

}
