using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public LarvaScript owner;
    public DefendObject defendObject;
    public float speed;

    private bool off = false;

    void Start()
    {
        //Point towards obj
        var dir = defendObject.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        StartCoroutine(Shoot());

        DefendObject.onDeath += turnOff;
    }

    public IEnumerator Shoot()
    {
        while (Vector2.Distance(transform.position, defendObject.transform.position) >= 0.5)
        {
            if (!off)
            {
                transform.position = Vector2.MoveTowards(transform.position, defendObject.transform.position, speed * Time.deltaTime);
                yield return null;
            }
            else
            {
                break;
            }
        }
        if (!off)
        {
            owner.receiveAttack();
            Destroy(gameObject);
        }
    }

    private void turnOff()
    {
        off = true;
    }
}
