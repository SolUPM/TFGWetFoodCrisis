using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeatInventory : MonoBehaviour
{
    public Stack<Meat> inventory = new Stack<Meat>();
    [Header("The sprites for the Meat Container on-screen, in order, including 0")]
    public Sprite[] invSprites;

    public Image invUI;

    public float cooldownBetweenDeposits = 0.75f;
    private float count = 0;

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Deposit")){
            count += Time.deltaTime;
            if(count >= cooldownBetweenDeposits && inventory.Count > 0)
            {
                count = 0;
                inventory.Pop()?.Deposit();
                
            }
            invUI.sprite = invSprites[inventory.Count];
        }

       
    }

    public bool AddMeat(Meat meat) {
        
        if(inventory.Count >= invSprites.Length - 1){
            return false;
        }
        else
        {
            inventory.Push(meat);
            invUI.sprite = invSprites[inventory.Count];
            return true;
        }
    }
}
