using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullScript : EnemyScript
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    protected override void attackFrame()
    {
        animator.SetTrigger("BlowUp");
        attackCounter += Time.deltaTime;
        if (attackCounter >= type.attackSpeed)
        {
            obj.changeHealth(-type.damage);
            attackCounter = 0;
            Instantiate(deathParticles, transform.position, Quaternion.identity);
            onDeath?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }
}
