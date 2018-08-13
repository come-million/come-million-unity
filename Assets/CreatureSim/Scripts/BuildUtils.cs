using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class BuildUtils : MonoBehaviour
{
    public float m_SmoothDeltaTime = 0.0f;
    public float FPS = 0.0f;

    private float m_DeltaVelocity = 0.0f;

    public Text TextFPS;
    public Text TextTimeTotal;
    public Text TextTimePlay;
    public Text TextTimePause;
    public Text TextTimelineCurrent;

    public StringBuilder theTextBuilder;
    public TimelineController theTimelineController;

    void Start ()
    {
        theTextBuilder = new StringBuilder("...", 20);
    }
	
	void Update ()
    {
        UpdateIngameText();

    }

    void UpdateIngameText()
    {
        // frame rate calculation
        m_SmoothDeltaTime = BuildUtils.SmoothDamp(m_SmoothDeltaTime, Time.deltaTime, ref m_DeltaVelocity, 0.7f, Mathf.Infinity, Time.deltaTime);
        FPS = m_SmoothDeltaTime > 0.0f ? (1.0f / m_SmoothDeltaTime) : 0.0f;

        if (TextFPS != null && theTextBuilder != null)
        {
            theTextBuilder.Remove(0, theTextBuilder.Length);
            theTextBuilder.Append(Mathf.RoundToInt(FPS));
            TextFPS.text = theTextBuilder.ToString();
        }


        if (TextTimeTotal != null && theTextBuilder != null && theTimelineController != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(theTimelineController.TimerTotal);
            theTextBuilder.Remove(0, theTextBuilder.Length);

            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                time.Hours,
                time.Minutes,
                time.Seconds);
            //time.TotalHours.ToString("F0") + ":" + time.TotalMinutes.ToString("F0") + ":" + time.Seconds.ToString("F0")
            theTextBuilder.Append(answer);
            TextTimeTotal.text = theTextBuilder.ToString();
        }

        if (TextTimePlay != null && theTextBuilder != null && theTimelineController != null)
        {
            theTextBuilder.Remove(0, theTextBuilder.Length);
            theTextBuilder.Append(theTimelineController.TimerPlaystatePlay.ToString("F2"));
            TextTimePlay.text = theTextBuilder.ToString();
        }

        if (TextTimePause != null && theTextBuilder != null && theTimelineController != null)
        {
            theTextBuilder.Remove(0, theTextBuilder.Length);
            theTextBuilder.Append(theTimelineController.TimerPlaystatePause.ToString("F2"));
            TextTimePause.text = theTextBuilder.ToString();
        }

        if (TextTimelineCurrent != null && theTextBuilder != null && theTimelineController != null)
        {
            theTextBuilder.Remove(0, theTextBuilder.Length);
            if (theTimelineController.CurrentTimelinePlaying < 0)
                theTextBuilder.Append("Skin");
            else
                theTextBuilder.Append(theTimelineController.CurrentTimelinePlaying.ToString("F2"));
            TextTimelineCurrent.text = theTextBuilder.ToString();
        }
    }


    // SmoothDamp float
    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    {
        // enforce minimum smooth time
        smoothTime = Mathf.Max(0.0001f, smoothTime);

        // calculate new target based on maximum allowable speed
        float maxDist = maxSpeed * smoothTime;
        float newTargetToCurrent = Mathf.Clamp(current - target, -maxDist, maxDist);
        float newTarget = current - newTargetToCurrent;

        // update velocity
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        float offset = (currentVelocity + num * newTargetToCurrent) * deltaTime;
        currentVelocity = (currentVelocity - num * offset) * d;

        float result = newTarget + (newTargetToCurrent + offset) * d;

        // check for overshoot
        if ((target - current > 0f) == (result > target))
        {
            // snap to target
            result = target;
            currentVelocity = (target - current) / deltaTime;
        }

        return result;
    }
}
