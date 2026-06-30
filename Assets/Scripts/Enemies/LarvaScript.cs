using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LarvaScript : EnemyScript
{
    public GameObject projectileObj;
    public float projectileSpeed;

    private Animator animator;


    private void Awake()
    {
        range = 8;
        animator = GetComponent<Animator>();
    }

    public override void MoveFrame()
    {
        animator.SetBool("Moving", true);
        base.MoveFrame();
    }

    protected override void attackFrame()
    {
        animator.SetBool("Moving", false);
        base.attackFrame();
    }
    protected override void attack()
    {
        animator.SetTrigger("Shoot");
        GameObject g = Instantiate(projectileObj, transform.position, Quaternion.identity);
        EnemyProjectile p = g.GetComponent<EnemyProjectile>();
        p.defendObject = obj;
        p.speed = projectileSpeed;
        p.owner = this;
    }

    public void receiveAttack()
    {
        base.attack();
    }
}
