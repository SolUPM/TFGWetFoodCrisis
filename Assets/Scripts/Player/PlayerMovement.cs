using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    public float moveSpeed = 0f, attackRange = 0f, attackCooldown = 0f;
    [HideInInspector]
    public float baseSpeed = 8, baseRange = 1.8f, baseAttack = 1.5f;
    [HideInInspector]
    public int speedUpgrades = 0, rangeUpgrades = 0, attackUpgrades = 0;

    public LayerMask enemyLayers;


    public GameObject slash;

    private float counter = 3;

    private Rigidbody2D rb;
    private Vector2 movement;

    private Animator anim;

    private static readonly int IsMoving = Animator.StringToHash("Moving");
    private static readonly int HorizontalLook = Animator.StringToHash("HorizontalLook");
    private static readonly int VerticalLook = Animator.StringToHash("VerticalLook");

    

    public enum LookDir
    {
        UP, DOWN, RIGHT, LEFT
    };
    
    public LookDir looking;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        moveSpeed = baseSpeed;
        attackCooldown = baseAttack;
        attackRange = baseRange;

        DefendObject.onDeath += turnOff;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && counter >= attackCooldown) 
        {
            Hit(attackRange);
            counter = 0;
        }
        else {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }

        UpdateAnimator();

        counter += Time.deltaTime;
    }

    private void UpdateLookDir()
    {
        if (movement.x > 0) looking = LookDir.RIGHT;
        else if (movement.x < 0) looking = LookDir.LEFT;
        else if (movement.y > 0) looking = LookDir.UP;
        else looking = LookDir.DOWN;
    }

    private void UpdateAnimator()
    {
        bool isMoving = movement.magnitude > 0;
        anim.SetBool(IsMoving, isMoving);

        if (!isMoving) return;

        anim.SetFloat(HorizontalLook, movement.x);
        anim.SetFloat(VerticalLook, movement.y);
        UpdateLookDir();
    }

    void FixedUpdate()
    {

        rb.velocity = movement.normalized * moveSpeed;
    }

    void Hit(float size)
    {
        anim.SetTrigger("Hit");

        Vector3 centerOfHit = transform.position;
        int rotation = 0;
        switch (looking)
        {
            case LookDir.UP:
                centerOfHit = centerOfHit + (Vector3.up * size / 2);
                rotation = 90;
                break;
            case LookDir.DOWN:
                centerOfHit = centerOfHit + (Vector3.down * size / 2);
                rotation = -90;
                break;
            case LookDir.LEFT:
                centerOfHit = centerOfHit + (Vector3.left * size / 2);
                rotation = 180;
                break;
            case LookDir.RIGHT:
                centerOfHit = centerOfHit + (Vector3.right * size / 2);
                break;
        }

        GameObject slashInst = Instantiate(slash, centerOfHit, Quaternion.Euler(new Vector3(0, 0, rotation)), transform);
        float sizeScale = attackRange / baseRange;
        slashInst.transform.localScale = new Vector3(sizeScale, sizeScale, 1);

        Destroy(slashInst, 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(centerOfHit, new Vector2(size, size), 0, enemyLayers);

        foreach(Collider2D hit in hits)
        {
            hit.gameObject.GetComponent<EnemyScript>().getHit(transform.position);
        }
    }

    private void turnOff()
    {
        rb.velocity = Vector2.zero;
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
            Vector3 centerOfHit = transform.position;
            switch (looking)
            {
                case LookDir.UP:
                    centerOfHit = centerOfHit + (Vector3.up * attackRange / 2);
                    break;
                case LookDir.DOWN:
                    centerOfHit = centerOfHit + (Vector3.down * attackRange / 2);
                    break;
                case LookDir.LEFT:
                    centerOfHit = centerOfHit + (Vector3.left * attackRange / 2);
                    break;
                case LookDir.RIGHT:
                    centerOfHit = centerOfHit + (Vector3.right * attackRange / 2);
                    break;
            }

            Debug.Log("im drawing a  cube at " + centerOfHit);
            Gizmos.DrawCube(centerOfHit, new Vector3(attackRange, attackRange));

        
    }
}
