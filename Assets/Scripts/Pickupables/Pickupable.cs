using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickupable : MonoBehaviour
{
    [SerializeField]
    private int scoreValue = 5;
    public int value = 2;

    [HideInInspector]
    public GameObject runParticles;
    [HideInInspector]
    public GameObject playerRef;

    public System.Action<GameObject> onPickup;

    void Start()
    {
        //runParticles = Resources.Load<GameObject>("PoofParticles");
        //SM = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent <ScoreManager>();
    }

    public virtual void OnPickup() 
    {
        ScoreManager.AddScore(scoreValue, transform.position);
        onPickup.Invoke(gameObject);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {          
            OnPickup();
        }
    }

    public virtual void Dissapear()
    {
        Instantiate(runParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
 
}
