using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public static event System.Action<int> OnScoreChanged;
    public static event System.Action<int, Vector2> OnScoreAdded;

    private int score;

    void Awake()
    {
        Instance = this;
    }

    public static void AddScore(int amount)
    {
        Instance.score += amount;
        OnScoreChanged?.Invoke(Instance.score);
    }
    public static void AddScore(int amount, Vector2 pos)
    {
        Instance.score += amount;
        OnScoreChanged?.Invoke(Instance.score);
        OnScoreAdded?.Invoke(amount, pos);
    }
}
