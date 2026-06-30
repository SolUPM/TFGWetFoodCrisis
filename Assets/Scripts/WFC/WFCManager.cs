using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WFCManager : MonoBehaviour
{
    [SerializeField]
    public int height = 20;
    [SerializeField]
    public int width = 20;
    private Tile[,] grid;

    public PerlinNoise noiseGen;

    public List<Tile> Tiles = new List<Tile>();
    private List<Tile>[,] wave;

    public GameObject[,] placedTiles;


    [HideInInspector] public int lastAttempts;
    [HideInInspector] public bool usedFallback;
    [HideInInspector] public bool totalFailure;
    [ContextMenu("Find Missing Compatibilities")]
    public void FindMissingCompatibilities()
    {
        foreach (Tile a in Tiles)
        {
            foreach (Tile b in a.compatibleTilesUp)
                if (!b.compatibleTilesDown.Contains(a))
                    Debug.LogError($"MISSING: {b.name} down should have {a.name}");

            foreach (Tile b in a.compatibleTilesDown)
                if (!b.compatibleTilesUp.Contains(a))
                    Debug.LogError($"MISSING: {b.name} up should have {a.name}");

            foreach (Tile b in a.compatibleTilesLeft)
                if (!b.compatibleTilesRight.Contains(a))
                    Debug.LogError($"MISSING: {b.name} right should have {a.name}");

            foreach (Tile b in a.compatibleTilesRight)
                if (!b.compatibleTilesLeft.Contains(a))
                    Debug.LogError($"MISSING: {b.name} left should have {a.name}");
        }
        Debug.Log("Compatibility check done!");
    }
    private Vector2Int[] directions = {
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0)
    };

    private void Start()
    {
        //startWFC();
    }

    public Tile[,] startWFC(int w, int h, bool testing = false)
    {
        if (testing)
        {
            usedFallback = false;
            totalFailure = false;
        }

        float startTime = Time.realtimeSinceStartup;
        width = w; height = h;
        int noAttempts = 0;
        for (int attempt = 0; attempt < 15; attempt++)
        {
            grid = new Tile[width, height];
            wave = new List<Tile>[width, height];

            if (placedTiles != null)
                for (int i = 0; i < placedTiles.GetLength(0); i++)
                    for (int j = 0; j < placedTiles.GetLength(1); j++)
                        if (placedTiles[i, j] != null)
                            Destroy(placedTiles[i, j]);

            placedTiles = new GameObject[width, height];
            noiseGen.Generate(width, height); // new noise each attempt

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    float temp = noiseGen.temperature[x, y];
                    wave[x, y] = new List<Tile>();
                    foreach (Tile tile in Tiles)
                        if (tile.isInTempRange(temp))
                            wave[x, y].Add(tile);
                    if (wave[x, y].Count == 0)
                        Debug.LogError($"No valid tiles at ({x},{y}) for temp {temp}");
                }

            if (CollapseWFC())
            {
                if (testing)
                {
                    lastAttempts = attempt + 1;
                }else
                    RenderTiles();
                return grid;
            }

            Debug.LogWarning($"Attempt {attempt + 1} failed, new noise...");
            noAttempts = attempt + 1;
        }

        Debug.LogError($"WFC fallido despuÈs de 15 intentos, {Time.realtimeSinceStartup - startTime:F3}s");
        if (testing) usedFallback = true;
        return fallBackStart(w, h);
    }

    private Tile[,] fallBackStart(int w, int h)
    {
        Debug.Log("HIZE FALLBACK WOOOWWW");
        usedFallback = true;
        float startTime = Time.realtimeSinceStartup;
        width = w; height = h;

        for (int attempt = 0; attempt < 10; attempt++)
        {
            grid = new Tile[width, height];
            wave = new List<Tile>[width, height];

            if (placedTiles != null)
                for (int i = 0; i < placedTiles.GetLength(0); i++)
                    for (int j = 0; j < placedTiles.GetLength(1); j++)
                        if (placedTiles[i, j] != null)
                            Destroy(placedTiles[i, j]);

            placedTiles = new GameObject[width, height];
            noiseGen.offset = new Vector2(-654, 123);
            noiseGen.randomOffset = false;
            noiseGen.Generate(width, height); // preset noise 

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    float temp = noiseGen.temperature[x, y];
                    wave[x, y] = new List<Tile>();
                    foreach (Tile tile in Tiles)
                        if (tile.isInTempRange(temp))
                            wave[x, y].Add(tile);
                    if (wave[x, y].Count == 0)
                        Debug.LogError($"No valid tiles at ({x},{y}) for temp {temp}");
                }

            if (CollapseWFC())
            {
                Debug.Log($"WFC completado en {attempt + 1} intentos, {Time.realtimeSinceStartup - startTime:F3}s");
                RenderTiles();
                return grid;
            }

            Debug.LogWarning($"Attempt {attempt + 1} of fallback failed...");
        }
        Debug.LogError("Fallback failed. How unlucky!");
        totalFailure = true;
        return null;

    }


    private bool CollapseWFC()
    {
        var history = new Stack<CollapseSnapshot>();
        var triedPerCell = new Dictionary<Vector2Int, HashSet<Tile>>();
        int iterations = 0;
        int limit = width * height * 5;

        while (TryGetLowestEntropy(out Vector2Int current))
        {
            if (++iterations > limit)
            {
                history.Clear();
                triedPerCell.Clear();
                Debug.LogWarning("WFC hit iteration limit");
                return false;
            }

            if (!triedPerCell.TryGetValue(current, out HashSet<Tile> tried))
            {
                tried = new HashSet<Tile>();
                triedPerCell[current] = tried;
            }

            // Build available from current wave minus tried
            List<Tile> available = new List<Tile>();
            foreach (Tile t in wave[current.x, current.y])
                if (!tried.Contains(t)) available.Add(t);

            if (available.Count == 0)
            {
                if (history.Count == 0)
                {
                    history.Clear();
                    triedPerCell.Clear();
                    return false;
                }
                triedPerCell.Remove(current);
                var popped = history.Pop();
                RestoreSnapshot(popped);
                popped.changedCells.Clear();
                continue;
            }

            Tile chosen = chooseWeightedRandomTile(available);
            tried.Add(chosen);

            var snapshot = new CollapseSnapshot(current);
            // Save current cell state before collapsing
            snapshot.SaveCell(current, wave[current.x, current.y], grid[current.x, current.y]);

            grid[current.x, current.y] = chosen;
            wave[current.x, current.y] = new List<Tile> { chosen };

            if (Propagate(current, snapshot))
            {
                history.Push(snapshot);
            }
            else
            {
                RestoreSnapshot(snapshot);
                snapshot.changedCells.Clear();
            }
        }

        history.Clear();
        triedPerCell.Clear();
        return true;
    }

    private void PlaceTile(int x, int y, Tile type)
    {
        grid[x, y] = type;
    }

    private bool TryGetLowestEntropy(out Vector2Int coord)
    {
        int lowest = int.MaxValue;
        List<Vector2Int> candidates = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null) continue;

                int entropy = wave[x, y].Count;

                if (entropy == 0) continue; // propagation should catch this

                if (entropy == 1)
                {
                    coord = new Vector2Int(x, y); // force collapse immediately
                    return true;
                }

                if (entropy < lowest)
                {
                    lowest = entropy;
                    candidates.Clear();
                    candidates.Add(new Vector2Int(x, y));
                }
                else if (entropy == lowest)
                {
                    candidates.Add(new Vector2Int(x, y));
                }
            }
        }

        if (candidates.Count == 0)
        {
            coord = Vector2Int.zero;
            return false;
        }

        coord = candidates[Random.Range(0, candidates.Count)];
        return true;
    }


    private Tile chooseWeightedRandomTile(List<Tile> tiles)
    {
        float rollingWeight = 0;
        float[] maxEndWeight = new float[tiles.Count];
        if (tiles.Count == 1)
        {
            return tiles[0];
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            rollingWeight += tiles[i].weight;
            maxEndWeight[i] = rollingWeight;
        }
        if (rollingWeight <= 0)
        {
            Debug.LogWarning("Somehow, weight is 0 in WFCManager");
            return tiles[0];
        }
        float randomFloat = Random.Range(0, rollingWeight);

        for (int i = 0; i < tiles.Count; i++)
        {
            if (randomFloat < maxEndWeight[i])
            {
                return tiles[i];
            }
        }

        Debug.LogWarning("WARNING: WEIGHTED RANDOM DIDNT WORK");
        return tiles[0];
    }


    private bool Propagate(Vector2Int start, CollapseSnapshot snapshot)
    {
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Vector2Int p = queue.Dequeue();

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int n = p + directions[i];

                if (!inGrid(n)) continue;
                if (grid[n.x, n.y] != null) continue;

                // Build allowed set
                var allowed = new HashSet<Tile>();
                foreach (Tile option in wave[p.x, p.y])
                {
                    List<Tile> compat = i switch
                    {
                        0 => option.compatibleTilesUp,
                        1 => option.compatibleTilesDown,
                        2 => option.compatibleTilesRight,
                        3 => option.compatibleTilesLeft,
                        _ => null
                    };
                    if (compat != null)
                        foreach (Tile t in compat) allowed.Add(t);
                }

                bool changed = false;
                var currentWave = wave[n.x, n.y];

                for (int j = currentWave.Count - 1; j >= 0; j--)
                {
                    if (!allowed.Contains(currentWave[j]))
                    {
                        if (!snapshot.HasCell(n))
                            snapshot.SaveCell(n, currentWave, grid[n.x, n.y]);

                        currentWave.RemoveAt(j);
                        changed = true;
                    }
                }

                if (currentWave.Count == 0) return false;
                if (changed) queue.Enqueue(n);
            }
        }
        return true;
    }

    private void RestoreSnapshot(CollapseSnapshot snapshot)
    {
        foreach (var (pos, tiles, tile) in snapshot.changedCells)
        {
            wave[pos.x, pos.y] = tiles; // reuse original list, no copy needed
            grid[pos.x, pos.y] = tile;
        }
    }

    private bool inGrid(Vector2Int coords)
    {
        return coords.x > -1 && coords.x < width && coords.y > -1 && coords.y < height;
    }

    string GetTileName(int x, int y)
    {
        if (!inGrid(new Vector2Int(x, y))) return "OOB";
        return grid[x, y] ? grid[x, y].name : "Empty";
    }

    private void RenderTiles()
    {
        // Clear old visuals
        if (placedTiles != null)
        {
            for (int x = 0; x < placedTiles.GetLength(0); x++)
            {
                for (int y = 0; y < placedTiles.GetLength(1); y++)
                {
                    if (placedTiles[x, y] != null)
                    {
                        Destroy(placedTiles[x, y]);
                    }
                }
            }
        }

        placedTiles = new GameObject[width, height];

        // Render final grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = grid[x, y];

                if (tile == null)
                    continue;

                GameObject g = new GameObject($"Tile_{x}_{y}");
                g.transform.parent = transform;

                SpriteRenderer sr = g.AddComponent<SpriteRenderer>();

                Sprite[] sprites = tile.sprites;
                sr.sprite = sprites[Random.Range(0, sprites.Length)];

                g.transform.position = new Vector2(x, y);

                placedTiles[x, y] = g;
            }
        }
    }


    private class CollapseSnapshot
    {
        public Vector2Int pos;
        public List<(Vector2Int pos, List<Tile> wave, Tile grid)> changedCells = new();
        private HashSet<Vector2Int> savedPositions = new(); // O(1) lookup instead of LINQ

        public CollapseSnapshot(Vector2Int pos)
        {
            this.pos = pos;
        }

        public void SaveCell(Vector2Int p, List<Tile> wave, Tile grid)
        {
            changedCells.Add((p, wave, grid));
            savedPositions.Add(p);
        }

        public bool HasCell(Vector2Int p) => savedPositions.Contains(p);
    }
    public void BenchmarkWFC(int width, int height, int runs)
    {

        float totalTime = 0;
        float maxTime = 0;

        int totalAttempts = 0;
        int maxAttempts = 0;

        int normalSuccesses = 0;
        int fallbackSuccesses = 0;
        int failures = 0;

        for (int i = 0; i < runs; i++)
        {
            float start = Time.realtimeSinceStartup;
            noiseGen.randomOffset = true;
            Random.InitState(System.DateTime.Now.Millisecond + i * 1000);
            Tile[,] result = startWFC(width, height, true);

            float elapsed = Time.realtimeSinceStartup - start;

            totalTime += elapsed;

            if (elapsed > maxTime)
                maxTime = elapsed;

            totalAttempts += lastAttempts;

            if (lastAttempts > maxAttempts)
                maxAttempts = lastAttempts;

            if (result != null)
            {
                if (usedFallback)
                    fallbackSuccesses++;
                else
                    normalSuccesses++;
            }
            else
            {
                failures++;
            }
        }

        Debug.Log(
            "\n=== WFC BENCHMARK ===" +
            $"\nRuns: {runs}" +
            $"\nSize: {width} x {height}" +
            $"\nTiempo medio: {totalTime / runs:F4}s" +
            $"\nTiempo m·ximo: {maxTime:F4}s" +
            $"\nIntentos medios: {(float)totalAttempts / runs:F2}" +
            $"\nIntentos m·ximos: {maxAttempts}" +
            $"\n…xito normal: {(float)normalSuccesses / runs * 100f:F1}%" +
            $"\n…xito con fallback: {(float)fallbackSuccesses / runs * 100f:F1}%" +
            $"\nFallo total: {(float)failures / runs * 100f:F1}%"
        );
    }
}




