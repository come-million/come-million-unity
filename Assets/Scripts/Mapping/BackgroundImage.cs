using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BackgroundImage : MonoBehaviour
{
    public Texture texture;
    public Material mat;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mat == null)
        {
            Graphics.Blit(src, dest);

        }
        else
        {
            Graphics.Blit(texture, dest);
            Graphics.Blit(src, dest, mat);
        }
    }
}
