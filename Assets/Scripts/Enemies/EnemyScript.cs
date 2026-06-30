using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : WPathTester
{
    //Setup
    public MovingUnit type;
    public GameObject destination;

    //Objective
    public Vector2Int goTo;
    protected DefendObject obj;

    //Changing stats
    public int hp;
    public bool attacking = false;
    protected float attackCounter;
    protected float range = 1;


    //Flash
    protected Material flashMaterial, originalMaterial;
    protected SpriteRenderer sr;

    protected GameObject deathParticles, runParticles;

    public System.Action<GameObject> onDeath;
    //End game
    private bool freeze = false;
    


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
        flashMaterial = Resources.Load<Material>("WhiteFlash");
        deathParticles = Resources.Load<GameObject>("DeathParticles");
        runParticles = Resources.Load<GameObject>("PoofParticles");
        attackCounter = -type.firstHitCooldown;

        DefendObject.onDeath += turnOff;

       
    }


    void Update()
    {
        if (!freeze)
        {
            if (destination == null) return;

            if (obj == null) obj = destination.GetComponent<DefendObject>();

            if (Mathf.Abs(Vector2.Distance(destination.transform.position, transform.position)) <= range)
            {
                attacking = true;
                attackFrame();
            }
            else
            {
                attacking = false;
                MoveFrame();
            }
        }
    }
    protected virtual void attackFrame()
    {
        attackCounter += Time.deltaTime;
        if (attackCounter >= type.attackSpeed)
        {
            attack();
            attackCounter = 0;
        }
    }

    protected virtual void attack()
    {
        if (obj.dead)
        {
            ableToMove = false;
            freeze = true;
        }
        obj.changeHealth(-type.damage);
    }



    public void getHit(Vector3 fromPos)
    {
        StartCoroutine(FlashWhite());
        hp -= 1;
        if(hp <= 0) Die();

        attackCounter = Mathf.Min(0f, attackCounter - 0.25f);

        if (type.pushable)
        {
            Vector2 differencePos = transform.position - fromPos;

            Vector2 pushDir = Mathf.Abs(differencePos.x) > Mathf.Abs(differencePos.y)
                                ? new Vector2(Mathf.Sign(differencePos.x), 0)
                                : new Vector2(0, Mathf.Sign(differencePos.y));

            if (!type.flies)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, pushDir, 1f, LayerMask.GetMask("Blocker"));
                if (hit.collider != null)
                    return; // blocked, no knockback
            }

            ableToMove = false;

            StartCoroutine(KnockbackCoroutine(pushDir));
        }
    }


    private IEnumerator KnockbackCoroutine(Vector2 dir)
    {
        float duration = 0.1f;
        float force = 25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.75f, LayerMask.GetMask("Blocker"));
            if (hit.collider == null)
                transform.Translate(dir * force * Time.deltaTime);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        StartPathfinding(goTo, snapStart: false);
        ableToMove = true;
    }

    private void Die()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        ScoreManager.AddScore(type.scoreValue, transform.position);
        onDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }

    public IEnumerator RunAway()
    {
        ableToMove = false;
        Vector2 awayDir = ((Vector2)transform.position - (Vector2)destination.transform.position).normalized;
        float elapsed = 0f;
        while (elapsed < 0.6f) //running away for 1 second
        {
            transform.localScale -= new Vector3(Time.deltaTime/3, Time.deltaTime/2);
            transform.Translate(awayDir * moveSpeed/2 * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Instantiate(runParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator FlashWhite()
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(0.1f);
        sr.material = originalMaterial;
    }

    private void turnOff()
    {
        ableToMove = false;
        freeze = true;
    }
}
