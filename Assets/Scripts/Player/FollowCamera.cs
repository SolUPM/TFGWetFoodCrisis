using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform follow;
    public float width, height;
    public float followSpeed = 3f;

    private Camera cam;

    private bool lost = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        DefendObject.onDeath += panToDeath;
    }

    private void FixedUpdate()
    {

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        Vector3 target = new Vector3(
            Mathf.Clamp(follow.position.x, halfW, (width - 0.5f) - halfW),
            Mathf.Clamp(follow.position.y, halfH, (height - 0.5f) - halfH),
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(transform.position, target, followSpeed * Time.fixedDeltaTime);

        if(lost && Vector2.Distance(transform.position, target) < 0.5f)
        {
            follow.gameObject.GetComponent<DefendObject>().explode = true;
        }
    }

    private void panToDeath()
    {
        follow = GameObject.FindGameObjectWithTag("Defend").transform;

        Debug.Log("Ahora follow es " + follow.gameObject.name);
        lost = true;
    }
}
