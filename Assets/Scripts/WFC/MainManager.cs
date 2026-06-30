using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{

    public WFCManager wfc;
    public PerlinNoise noise;
    public DecorationPlacer deco;
    public FollowCamera cameraScript;
    public EnemySpawner enemySpawner;
    public NightMeatSpawner pickupSpawner;
    public GameObject player, cameraObj;

    public GameObject LoadingScreen;

    //True only to debug WFC
    public bool testWFC = false;
     
    public int width = 75, height = 75;

    void Start()
    {
        
        //Propagation and constraints become unstable (and sometimes impossible!) when maps are higher than 96x96 :(
        //width = Mathf.Min(width, 95);
        //height = Mathf.Min(height, 95);
        int middleX = Mathf.RoundToInt(width / 2), middleY = Mathf.RoundToInt(height / 2);
        if (testWFC)
        {
            wfc.BenchmarkWFC(width, height, 100);
        }
        else
        {
            //world creation
            Tile[,] wfcGrid = wfc.startWFC(width, height);
            Decoration[,] decoGrid = deco.placeDecorations(wfcGrid, noise, wfc.placedTiles);
            cameraScript.height = height;
            cameraScript.width = width;

            //border
            GameObject border = new GameObject("WorldBorder");
            EdgeCollider2D edge;
            edge = border.AddComponent<EdgeCollider2D>();
            edge.points = new Vector2[]
            {
            new Vector2(0-0.5f, 0-0.5f),
            new Vector2(width-0.5f, 0-0.5f),
            new Vector2(width-0.5f, height-0.5f),
            new Vector2(0-0.5f, height-0.5f),
            new Vector2(0-0.5f, 0-0.5f)
            };

            //Player and camera position
            player.transform.position = new Vector2(middleX, middleY + 2);
            cameraObj.transform.position = new Vector3(middleX, middleY + 2, -10);

            //Spawners get set up and started
            enemySpawner.mapWidth = width;
            enemySpawner.mapHeight = height;
            enemySpawner.grid = wfcGrid;
            enemySpawner.decorations = decoGrid;
            enemySpawner.StartSpawning();
            pickupSpawner.mapWidth = width;
            pickupSpawner.mapHeight = height;

            LoadingScreen.SetActive(false);
        }
    }

}
