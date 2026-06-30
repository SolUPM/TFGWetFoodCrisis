using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDebugDrawer : MonoBehaviour
{
    public WFCManager wfc;

    private void OnDrawGizmos()
    {
        if (wfc == null || wfc.noiseGen.temperature == null) return;

        int width = wfc.width;
        int height = wfc.height;
        var map = wfc.noiseGen.temperature;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float t = map[x, y];
                Gizmos.color = Color.Lerp(Color.blue, Color.red, t);
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
            }
        }
    }
}
