using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public float scale = 0.065f;
    public Vector2 offset;

    public bool randomOffset = false;

    public float[,] temperature { get; private set; }

    public void Generate(int width, int height)
    {
        if (randomOffset)
        {
            offset = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
        }
        temperature = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nx = (x + offset.x) * scale;
                float ny = (y + offset.y) * scale;
                temperature[x, y] = Mathf.Clamp01(Mathf.PerlinNoise(nx, ny));
            }
        }

    }

}
