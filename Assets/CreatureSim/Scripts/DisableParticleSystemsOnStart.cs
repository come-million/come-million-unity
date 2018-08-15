using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableParticleSystemsOnStart : MonoBehaviour
{

    public bool DisableOnStart = true;

    public ParticleSystem[] ParticleSystems;

    void Start()
    {
        if (ParticleSystems == null || ParticleSystems.Length == 0)
            ParticleSystems = GetComponentsInChildren<ParticleSystem>(true);

        if (ParticleSystems != null)
        {
            for (int i = 0; i < ParticleSystems.Length; i++)
            {
                ParticleSystem.EmissionModule emission = ParticleSystems[i].emission;
                emission.enabled = false;
            }
        }
    }
}
