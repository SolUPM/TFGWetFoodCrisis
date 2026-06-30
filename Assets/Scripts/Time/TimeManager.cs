using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [Header("Timing")]
    public float timeDay = 5f;
    public float timeNight = 4f;
    public float transitionDuration = 1.5f;

    [Header("References")]
    [SerializeField] private Image timeImage;
    public Animator filterAnimator;
    public Sprite dayIcon, nightIcon;

    public bool isNight { get; private set; }

    private enum State { Day, Transitioning, Night }
    private State state = State.Day;
    private float timer;

    private void Start()
    {
        DefendObject.onDeath += turnOff;
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case State.Day:
                if (timer >= timeDay)
                    BeginTransition(toNight: true);
                break;

            case State.Night:
                if (timer >= timeNight)
                    BeginTransition(toNight: false);
                break;

            case State.Transitioning:
                if (timer >= transitionDuration)
                    CompleteTransition();
                break;
        }
    }

    private void BeginTransition(bool toNight)
    {
        state = State.Transitioning;
        timer = 0;
        isNight = toNight; //flip
        filterAnimator.SetTrigger(toNight ? "ToNight" : "ToDay");
    }

    private void CompleteTransition()
    {
        timer = 0;
        state = isNight ? State.Night : State.Day;
        timeImage.sprite = isNight ? nightIcon : dayIcon; //just update visuals
    }

    private void turnOff()
    {
        this.enabled = false;
    }
}
