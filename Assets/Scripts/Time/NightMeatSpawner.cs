using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NightMeatSpawner : MonoBehaviour
{

    public List<GameObject> spawnables;
    public List<GameObject> spawned = new List<GameObject>();
    private float maxValue;

    public TimeManager dayNight;

    [Header("Spawning parameters")]
    public float spawnTime = 0.75f;
    public int maxSpawnables = 15;

    [HideInInspector]
    public int mapWidth;
    [HideInInspector]
    public int mapHeight;

    private float counter = 0;


    private GameObject poofParticles;

    private GameObject playerRef;





    // Start is called before the first frame update
    void Start()
    {
        poofParticles = Resources.Load<GameObject>("PoofParticles");
        playerRef = GameObject.FindGameObjectWithTag("Player");
        maxValue = spawnables.Max(s => s.GetComponent<Pickupable>().value);

        DefendObject.onDeath += turnOff;
    }

    // Update is called once per frame
    void Update()
    {
        if (dayNight.isNight)
        {
            counter += Time.deltaTime;
            if(counter >= spawnTime)
            {
                counter = 0;
                if (spawned.Count < maxSpawnables)
                {
                    //int randomChoice = Random.Range(0, spawnables.Count);
                    Vector3 spawnPoint = choosePlace();
                    GameObject g = Instantiate(ChooseByDistance(spawnPoint), spawnPoint, Quaternion.identity);
                    Pickupable p = g.GetComponent<Pickupable>();
                    p.runParticles = poofParticles;
                    p.playerRef = playerRef;
                    p.onPickup = (p) => spawned.Remove(g);
                    spawned.Add(g);
                }
            }
        }
        else
        {
            if(spawned.Count > 0)
            {
                ClearSpawned();
            }
        }
    }

    private GameObject ChooseByDistance(Vector2 spawnPos)
    {
        Vector2 center = new Vector2(mapWidth / 2f, mapHeight / 2f);
        float distanceRatio = Vector2.Distance(spawnPos, center) / Vector2.Distance(Vector2.zero, center);
        // distanceRatio: 0 = center, 1 = corner

        // Filter and weight by value vs distance
        float totalWeight = 0f;
        float[] weights = new float[spawnables.Count];

        for (int i = 0; i < spawnables.Count; i++)
        {
            float value = spawnables[i].GetComponent<Pickupable>().value;
            // Higher value tiles get boosted weight further from center
            // Lower value tiles get boosted weight near center
            float distanceMatch = 1f - Mathf.Abs(distanceRatio - (value / maxValue));
            weights[i] = Mathf.Max(0.01f, distanceMatch);
            totalWeight += weights[i];
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < spawnables.Count; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return spawnables[i];
        }
        return spawnables[0];
    }

    private Vector3 choosePlace()
    {
        Vector3 pos;
        int attempts = 0;
        do
        {
            pos = new Vector3(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            attempts++;
            if (attempts > 100)
            {
                Debug.LogWarning("Too many failed attempts when choosing position for Pickup.");
                break;
            }
        } while (Physics2D.OverlapPoint(pos, ~LayerMask.GetMask("Pickup")) != null || pickupIn(pos));
        return pos;
    }

    private void ClearSpawned()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            GameObject e = spawned[i];
            if (e == null)
            {
                spawned.RemoveAt(i);
                continue;
            }
            spawned.RemoveAt(i);
            Pickupable p = e.GetComponent<Pickupable>();
            p.Dissapear();
                        
        }
    }

    private bool pickupIn(Vector3 pos)
    {
        foreach(GameObject g in spawned)
        {
            if (g.transform.position == pos)
            {
                return true;
            }
        }
        return false;
    }

    private void turnOff()
    {
        this.enabled = false;
    }
}
