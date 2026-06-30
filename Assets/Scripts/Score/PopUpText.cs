using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpText : MonoBehaviour
{
    [SerializeField]
    private GameObject TextObject;

    void Start()
    {
        ScoreManager.OnScoreAdded += scorePopUp;
        Meat.onHeal += healPopUp;
        Meat.onHPUp += hpupPopUp;
        Orange.onPickedUp += speedPopUp;
        Onion.onPickedUp += rangePopUp;
        Apple.onPickedUp += atkUpPopUp;
    }

    private void textFloat(GameObject t)
    {
        t.GetComponent<Rigidbody2D>().velocity = Vector2.up * 3;
        Destroy(t, 0.5f);
    }

    private void textFall(GameObject t)
    { 
        //Move sprite down so it is centered
        t.transform.position = new Vector2(t.transform.position.x, t.transform.position.y - 0.5f);

        t.GetComponent<Rigidbody2D>().velocity = Vector2.down * 3;
        Destroy(t, 0.5f);
    }

    public void scorePopUp(int amount, Vector2 pos)
    {
        GameObject g = Instantiate(TextObject, pos, Quaternion.identity);
        g.GetComponent<TMP_Text>().text = "+" + amount;
        textFloat(g);
    }
    

    public void healPopUp(Vector2 pos)
    {
        GameObject g = Instantiate(TextObject, pos, Quaternion.identity);
        TMP_Text text = g.GetComponent<TMP_Text>();
        text.text = "Heal!";
        text.color = Color.green;
        textFall(g);
    }

    public void hpupPopUp(Vector2 pos)
    {
        GameObject g = Instantiate(TextObject, pos, Quaternion.identity);
        TMP_Text text = g.GetComponent<TMP_Text>();
        text.text = "HP UP!";
        text.color = Color.cyan;
        textFall(g);
    }

    public void atkUpPopUp(Vector2 pos)
    {
        GameObject g = Instantiate(TextObject, pos, Quaternion.identity);
        TMP_Text text = g.GetComponent<TMP_Text>();
        text.text = "ATK CD Down!";
        text.color = Color.red;
        textFall(g);
    }

    public void rangePopUp(Vector2 pos)
    {
        GameObject g = Instantiate(TextObject, pos, Quaternion.identity);
        TMP_Text text = g.GetComponent<TMP_Text>();
        text.text = "Range UP!";
        text.color = Color.gray;
        textFall(g);
    }

    public void speedPopUp(Vector2 pos)
    {
        GameObject g = Instantiate(TextObject, pos, Quaternion.identity);
        TMP_Text text = g.GetComponent<TMP_Text>();
        text.text = "Speed UP!";
        text.color = Color.yellow;
        textFall(g);
    }
}
