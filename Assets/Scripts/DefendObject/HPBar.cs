using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [Header("All HP sprites, in order, with a 0 HP sprite too.")]
    [SerializeField]
    public Sprite[] hpVisuals;

    [Header("Space parameters")]
    [SerializeField]
    private float spriteSize = 1, spaceBetweenSprites = 0.125f;

    private List<GameObject> createdSprites = new List<GameObject>();



    public void showHP(int hp, int difference, int maxHP)
    {
        RemoveSprites();
        int spriteContains = hpVisuals.Length - 1;

        if (maxHP % spriteContains != 0)
        {
            Debug.LogWarning("MaxHP is not divisible by the amount of sprites");
        }

        int numSprites = Mathf.CeilToInt(maxHP / spriteContains);


        
        float widthOfBar = (numSprites * (spriteSize + spaceBetweenSprites)) - spaceBetweenSprites;
        transform.parent.GetComponent<WorldSpaceClamp>().barHalfWidth = widthOfBar / 2f;

        int hpToShow = hp;



        Vector3 spritePos = new Vector3(0.5f, 0, 0);
        
        CreateSprites(hpToShow, spriteContains, spritePos, numSprites);

        transform.localPosition = new Vector3(-(widthOfBar / 2), 0, 0);

    }

    private void RemoveSprites()
    {
        for(int i = createdSprites.Count-1; i >= 0; i--)
        {
            GameObject g = createdSprites[i];
            PooledDelete(g);
        }
    }


    private void CreateSprites(int hpToShow, int spriteContains, Vector3 spritePos, int numSprites)
    {
        int spritesCreated = 0;
        //Create filled or half-filled sprites
        do
        {
            
            if (hpToShow >= spriteContains)
            {
                PooledInstantiate(spritePos, spritesCreated, spriteContains);
                hpToShow -= spriteContains;
            }
            else
            {
                PooledInstantiate(spritePos, spritesCreated, hpToShow);
                hpToShow = 0;
            }

            spritePos = spritePos + Vector3.right * (spriteSize + spaceBetweenSprites);
            numSprites--;
            spritesCreated++;

        } while (hpToShow > 0); 
        
        //Create empty sprites
        while (numSprites > 0)
        {
            PooledInstantiate(spritePos, spritesCreated, 0);
            spritePos = spritePos + Vector3.right * (spriteSize + spaceBetweenSprites);
            numSprites--;
            spritesCreated++;
        }
    }

    private void PooledInstantiate(Vector3 pos, int spriteNum, int hpVisual)
    {
        GameObject g;
        SpriteRenderer spriteR;

        if (createdSprites.Count <= spriteNum || createdSprites.Count == 0)
        {
            g = new GameObject("HPSprite " + spriteNum);
            spriteR = g.AddComponent<SpriteRenderer>();
            spriteR.sortingOrder = 5;
            g.transform.parent = transform;
            createdSprites.Add(g);
        }
        else
        {
            g = createdSprites[spriteNum];
            spriteR = g.GetComponent<SpriteRenderer>();
        }

        spriteR.sprite = hpVisuals[hpVisual];
        g.transform.localPosition = pos;
        spriteR.enabled = true;
    }

    private void PooledDelete(GameObject g)
    {
        if (createdSprites.Contains(g))
        {
            g.GetComponent<SpriteRenderer>().enabled = false;
            g.transform.localPosition = Vector3.zero;
        }
    }

}
