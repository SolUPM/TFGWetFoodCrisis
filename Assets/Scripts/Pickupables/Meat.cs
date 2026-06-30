using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Pickupable
{
    [Range(0f, 1f)]
    public float chanceHeal, chanceHPUP;

    public int healValue;

    private DefendObject defendObj;
    private HPBar defendHP;

    private MeatInventory inv;

    private bool pickedUp = false;

    public static event System.Action<Vector2> onHeal, onHPUp;



    private void Start()
    {
        
        defendObj = GameObject.FindGameObjectWithTag("Defend").GetComponent<DefendObject>();
        defendHP = GameObject.FindGameObjectWithTag("HPBar").GetComponent<HPBar>();
        inv = playerRef.GetComponent<MeatInventory>();
    }

    public override void OnPickup()
    {
        if (inv.AddMeat(this))
        {
            pickedUp = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public void Deposit()
    {
        StartCoroutine("goToCollect");
    }

    private void OnDeposit()
    {
        float randomChance = Random.Range(0f, 1f);
        if (randomChance < chanceHeal)
        {
            defendObj.changeHealth(healValue);
            onHeal?.Invoke(transform.position);
        }

        if (randomChance < chanceHPUP)
        {            
            defendObj.maxHP += defendHP.hpVisuals.Length - 1;
            //Heal for 0 to update the HP bar
            defendObj.changeHealth(0);
            onHPUp?.Invoke(transform.position);
        }

        base.OnPickup();
    }

    

    private IEnumerator goToCollect()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        transform.position = playerRef.transform.position;
        GetComponent<SpriteRenderer>().enabled = true;
        //The meat flies over to the defend object each frame to get deposited
        while(Vector3.Distance(transform.position, defendObj.transform.position) > 0.25f)
        {
            transform.position = Vector2.Lerp(transform.position, defendObj.transform.position, 3.5f * Time.deltaTime);
            yield return null;
        }
        OnDeposit();
    }
    public override void Dissapear()
    {
        if (!pickedUp)
            base.Dissapear();
    }
}
