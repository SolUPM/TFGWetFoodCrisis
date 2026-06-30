using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefendObject : MonoBehaviour
{

    public int maxHP = 12;
    public int hp = 12;
    [SerializeField]
    private HPBar hpBar;

    public static event System.Action onDeath;
    public bool explode, dead = false;
    [SerializeField]
    private GameObject deathParticles;
    [SerializeField]
    private Sprite dogSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*//DEBUG
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            changeHealth(-1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            changeHealth(-4);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            changeHealth(+1);
        }*/

    }

    public void changeHealth(int amount)
    {
        if (!dead)
        {
            int hpBefore = hp;
            hp = Mathf.Clamp(hp + amount, 0, maxHP);
            if (hp == 0)
            {
                StartCoroutine(Die());
            }
            
            hpBar.showHP(hp, amount, maxHP);

        }
    }


    public IEnumerator Die()
    {
        dead = true;
        onDeath?.Invoke();

        yield return new WaitUntil(() => explode);
        yield return new WaitForSeconds(1f);

        Instantiate(deathParticles, transform.position, Quaternion.identity);
        GetComponent<SpriteRenderer>().sprite = dogSprite;
        yield return new WaitForSeconds(1.5f);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
