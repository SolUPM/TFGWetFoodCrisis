using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int enemiesPerWave = 5;
    public float timeBetweenSpawns = 0.5f;
    public float timeBetweenWaves = 5f;

    private int waveNum = 1;
    [Header("---")]

    public List<GameObject> enemyPrefabs;
    public GameObject destination;
    [Header("Enemies to add or remove in between waves")]
    public List<WaveChange> waveChanges;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    //References
    public TimeManager dayNight;
    public TiletoNodeTranslator TtN;
    public NodeHandler nHandler;
    public Tile[,] grid;
    public Decoration[,] decorations;

    [HideInInspector]
    public int mapWidth;
    [HideInInspector]
    public int mapHeight;

    private bool turnedOff = false;

    public void StartSpawning()
    {
        destination = GameObject.FindGameObjectWithTag("Defend");
        StartCoroutine(SpawnLoop());
        DefendObject.onDeath += turnOff;
    }

    private IEnumerator SpawnLoop()
    {
        while (!turnedOff)
        {

            if (dayNight.isNight)
            {
                CleanFarEnemies();
                yield return new WaitUntil(() => !dayNight.isNight);
            }


            yield return StartCoroutine(SpawnWave());

            float elapsed = 0f;
            while (elapsed < timeBetweenWaves && !dayNight.isNight)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            waveUp();
        }
    }

    private IEnumerator SpawnWave()
    {
        int attempts = 0;
        for (int i = 0; i < enemiesPerWave; i++)
        {
            if (!SpawnEnemy())
            {
                i--;
                attempts++;
                if (attempts > 20) break; // give up
            }
            else attempts = 0;

            if (dayNight.isNight) break;
            yield return new WaitForSeconds(timeBetweenSpawns);

            
        }
    }

    private bool SpawnEnemy()
    {
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        MovingUnit type = prefab.GetComponent<EnemyScript>().type;

        Vector2 spawnPos = GetRandomEdgePosition(type);
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        EnemyScript es = enemy.GetComponent<EnemyScript>();

        setUpEnemy(es);

        if (!es.StartPathfinding(es.goTo))
        {
            Destroy(enemy);
            return false;
        }

        spawnedEnemies.Add(enemy);
        return true;
    }

    private void setUpEnemy(EnemyScript es)
    {
        WNode[,] PFGrid = TtN.getNodes(grid, decorations, es.type);
        es.grid = PFGrid;
        es.nHandler = nHandler;
        es.moveSpeed = es.type.baseSpeed;
        es.goTo = new Vector2Int(Mathf.RoundToInt(destination.transform.position.x), Mathf.RoundToInt(destination.transform.position.y));
        es.destination = destination;
        es.pathfinder.nHandler = nHandler;
        es.hp = es.type.hp;
        es.onDeath = (g) => spawnedEnemies.Remove(g);

    }

    private void waveUp()
    {
        waveNum++;
        enemiesPerWave = Mathf.FloorToInt(25 - (25 - 5) / (1 + waveNum * 0.05f));
        foreach (WaveChange change in waveChanges)
        {
            if (change.wave == waveNum)
            {
                foreach (GameObject g in change.toAdd)
                {
                    enemyPrefabs.Add(g);
                   
                }
                foreach (GameObject g in change.toRemove)
                {
                    if (enemyPrefabs.Contains(g))
                    {
                        enemyPrefabs.Remove(g);
                    }
                }
            }
        }
    }

    private Vector2 GetRandomEdgePosition(MovingUnit type)
    {
        Vector2 pos;
        int attempts = 0;
        do
        {
            int edge = Random.Range(0, 4);
            pos = edge switch
            {
                0 => new Vector2(Random.Range(0, mapWidth), mapHeight),
                1 => new Vector2(Random.Range(0, mapWidth), 0),
                2 => new Vector2(0, Random.Range(0, mapHeight)),
                3 => new Vector2(mapWidth, Random.Range(0, mapHeight)),
                _ => Vector2.zero
            };
            attempts++;
        }
        while (!type.flies &&
               Physics2D.OverlapPoint(pos, LayerMask.GetMask("Blocker")) != null &&
               attempts < 20);

        return pos;
    }

    private void CleanFarEnemies()
    {
        float threshold = 12f;
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject e = spawnedEnemies[i];
            if (e == null)
            {
                spawnedEnemies.RemoveAt(i);
                continue;
            }
            if (Vector2.Distance(e.transform.position, destination.transform.position) > threshold)
            {
                e.GetComponent<EnemyScript>().StartCoroutine(e.GetComponent<EnemyScript>().RunAway());
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    private void turnOff()
    {
        turnedOff = true;
    }

    [System.Serializable]
    public class WaveChange
    {
        public int wave;
        public List<GameObject> toAdd;
        public List<GameObject> toRemove;
    }
}
