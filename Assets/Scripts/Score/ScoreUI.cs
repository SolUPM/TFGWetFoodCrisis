using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    void Start()
    {
        ScoreManager.OnScoreChanged += UpdateText;
    }

    

    public void UpdateText(int score)
    {
        text.text = score + ""; 
    }
}
