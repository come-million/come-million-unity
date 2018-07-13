using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToShader : MonoBehaviour {
	
	public string PropertyName;
	public bool inverse = false;
    private int propId;

    // Use this for initialization
    void Start () {
		propId = Shader.PropertyToID(PropertyName);
	}
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalMatrix(propId, inverse ? transform.worldToLocalMatrix : transform.localToWorldMatrix);
	}
}
