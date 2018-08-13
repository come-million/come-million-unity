using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartbeat : MonoBehaviour
{
    public Vector3 ScaleMin = Vector3.zero;
    public Vector3 ScaleMax = Vector3.one;
    public float TimeScale = 1.0f;
    public float Lerp = 0.5f;
    public float RadiusSpeedValue = 1.0f;

    private float m_RandomStartTimeOffset = 0.0f;

    public ParticleSystem[] ParticleSystems;
    public float CurrentRadius = 0.0f;



    void Start ()
    {
        m_RandomStartTimeOffset = Random.Range(0.0f, 100000.0f);

        //ParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void UpdateParticleSystems(float currentFloatScale)
    {
        CurrentRadius = Mathf.Lerp(ScaleMin.magnitude, ScaleMax.magnitude, currentFloatScale);

        if (ParticleSystems != null && ParticleSystems.Length > 0)
        {
            for (int i = 0; i < ParticleSystems.Length; i++)
            {
                ParticleSystem.ShapeModule shape = ParticleSystems[i].shape;
                shape.radius = CurrentRadius;

                ParticleSystem.MainModule main = ParticleSystems[i].main;
                
                if (i <= 2)
                    main.startSpeed = RadiusSpeedValue * (float)(i + 1) * CurrentRadius;
                else
                    main.startSpeed = RadiusSpeedValue * 1.5f * CurrentRadius;
            }
        }
    }

    void Update ()
    {
        float currentFloatScale = 1.0f;
        float currentTime = m_RandomStartTimeOffset + Time.time;

        currentFloatScale = Mathf.Abs(Mathf.Cos(Mathf.Sin(3.0f * currentTime * TimeScale) + 3.0f * currentTime * TimeScale));

        //transform.localScale = Vector3.Lerp(transform.localScale, ScaleMin + currentFloatScale * (ScaleMax - ScaleMin), Lerp);

        UpdateParticleSystems(currentFloatScale);
    }
}
