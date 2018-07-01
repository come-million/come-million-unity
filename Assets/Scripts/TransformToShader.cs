using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToShader : MonoBehaviour {
	
	public string PropertyName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalMatrix(PropertyName, transform.localToWorldMatrix);
	}
}
