using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionToShader : MonoBehaviour
{

    public string property;
    public bool inverse = false;
    // public bool GPU = true;
    // public bool RT = true;

    int propId;
    int orthoId;

    Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        propId = Shader.PropertyToID(property);
        orthoId = Shader.PropertyToID("_IsOrtho");
    }

    // Update is called once per frame
    void Update()
    {
        var p = inverse ? cam.projectionMatrix.inverse : cam.projectionMatrix;
        // if (GPU)
            // p = GL.GetGPUProjectionMatrix(p, RT);
        Shader.SetGlobalInt(orthoId, cam.orthographic ? 1 : 0);
        Shader.SetGlobalMatrix(propId, p);
    }
}
