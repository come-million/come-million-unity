using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateParticleRenderer : MonoBehaviour {

    public Renderer rend;
    public float baseAlpha = 0.35f;
    public float alphaRange = 0.2f;

	void Start ()
    {
        rend = GetComponent<Renderer>();
    }

    void Update ()
    {
        ////Set the main Color of the Material to green
        //rend.material.shader = Shader.Find("_TintColor");
        Color currentColor = rend.material.GetColor("_TintColor");

        currentColor.a = baseAlpha + alphaRange * Mathf.Sin(Mathf.PI * Time.timeSinceLevelLoad);
        rend.material.SetColor("_TintColor", currentColor);
        // GetComponent<ParticleSystemRenderer>().material.color = Color.Lerp(Color.red, Color.blue, 0.5f);
    }
}
