using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearWhenLose : MonoBehaviour
{
    [SerializeField]
    public GameObject toAppear;

    private void Start()
    {
        DefendObject.onDeath += appear;
    }

    private void appear()
    {
        StartCoroutine(appearWithDelay(2.5f));
    }

    private IEnumerator appearWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        toAppear.SetActive(true);
        
    }
}
        

