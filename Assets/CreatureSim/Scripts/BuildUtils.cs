using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class BuildUtils : MonoBehaviour
{
    public float m_SmoothDeltaTime = 0.0f;
    public float FPS = 0.0f;

    private float m_DeltaVelocity = 0.0f;

    public Text FPSText;

    public StringBuilder FPSBuilder;

    void Start ()
    {
        if (FPSText == null)
            FPSText = transform.GetComponent<Text>();

        FPSBuilder = new StringBuilder("...", 5);
    }
	
	void Update ()
    {
        // frame rate calculation
        m_SmoothDeltaTime = BuildUtils.SmoothDamp(m_SmoothDeltaTime, Time.deltaTime, ref m_DeltaVelocity, 0.7f, Mathf.Infinity, Time.deltaTime);
        FPS = m_SmoothDeltaTime > 0.0f ? (1.0f / m_SmoothDeltaTime) : 0.0f;

        if (FPSText != null && FPSBuilder != null)
        {
            FPSBuilder.Remove(0, FPSBuilder.Length);
            FPSBuilder.Append(Mathf.RoundToInt(FPS));
            FPSText.text = FPSBuilder.ToString();
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
