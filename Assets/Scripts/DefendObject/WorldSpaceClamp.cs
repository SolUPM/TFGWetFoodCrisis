using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WorldSpaceClamp : MonoBehaviour
{
    private Camera cam;
    private Vector3 target;
    public float barHalfWidth;
    public float paddingX = 0.5f;
    public float paddingY = 1.5f;

    void Start()
    {
        cam = Camera.main;
        target = transform.parent.position - new Vector3(0,1.5f,0);
    }

    void LateUpdate()
    {
        if (cam == null) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 camPos = cam.transform.position;

        float clampedX = Mathf.Clamp(target.x, camPos.x - halfW + barHalfWidth + paddingX, camPos.x + halfW - barHalfWidth - paddingX);
        float clampedY = Mathf.Clamp(target.y, camPos.y - halfH + paddingY, camPos.y + halfH - paddingY);

        transform.position = new Vector3(clampedX, clampedY, target.z);
    }
}
