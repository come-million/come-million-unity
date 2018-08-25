using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using ComeMillion;

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
    public Text TextTimeScale;
    public Text TextSkinBrightness;
    public Text TextMainBrightness;
    public Slider SkinBrightnessSlider;
    public StringBuilder theTextBuilder;
    public TimelineController theTimelineController;

    public int TabState = 0;
    private int NumTabStates = 2;

    public float SkinBrightnessSliderValue = 0.0f;
    public float MainBrightnessSliderValue = 0.0f;

    private float ReferenceSkinBrightness = 0.1f;

    void Start()
    {
        Application.targetFrameRate = 50;
        theTextBuilder = new StringBuilder("...", 20);
        SetTimeScale(1.0f);
        SetMainBrightnessSliderValue(1.0f);

        if (SkinBrightnessSlider != null)
        {
            ReferenceSkinBrightness = SkinBrightnessSlider.value;
            Shader.SetGlobalFloat("_ReferenceSkinBrightness", ReferenceSkinBrightness);
            SetSkinBrightnessSliderValue(ReferenceSkinBrightness);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TabState = (TabState + 1) % NumTabStates;
        }

        UpdateIngameText();
    }

    void UpdateIngameText()
    {
        // frame rate calculation
        m_SmoothDeltaTime = BuildUtils.SmoothDamp(m_SmoothDeltaTime, Time.unscaledDeltaTime, ref m_DeltaVelocity, 0.7f, Mathf.Infinity, Time.unscaledDeltaTime);
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
            //theTextBuilder.Append(string.Format("{0:hh\\:mm\\:ss\\:f}", time)); // this works well only for time-spans under 24 hours :) 
            int hours = (int)time.TotalHours;
            int minutes = time.Minutes;
            int seconds = time.Seconds;
            theTextBuilder.Append(hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2"));
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
            theTextBuilder.Append(theTimelineController.GetCurrentTimelineName());
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

    public void SetTimeScale(float t)
    {
        Time.timeScale = t;

        if (TextTimeScale != null && theTextBuilder != null)
        {
            theTextBuilder.Remove(0, theTextBuilder.Length);
            theTextBuilder.Append(Time.timeScale.ToString("F2"));
            TextTimeScale.text = theTextBuilder.ToString();
        }
    }

    public void SetSkinBrightnessSliderValue(float t)
    {
        SkinBrightnessSliderValue = t;
        Shader.SetGlobalFloat("_GlobalSkinBrightness", SkinBrightnessSliderValue);

        if (TextSkinBrightness != null && theTextBuilder != null)
        {
            theTextBuilder.Remove(0, theTextBuilder.Length);
            theTextBuilder.Append(SkinBrightnessSliderValue.ToString("F2"));
            TextSkinBrightness.text = theTextBuilder.ToString();
        }
    }

    public void SetMainBrightnessSliderValue(float t)
    { 
        MainBrightnessSliderValue = t;

        if (TextMainBrightness != null && theTextBuilder != null)
        {
            theTextBuilder.Remove(0, theTextBuilder.Length);
            theTextBuilder.Append(MainBrightnessSliderValue.ToString("F2"));
            TextMainBrightness.text = theTextBuilder.ToString();
        }
    }

}
