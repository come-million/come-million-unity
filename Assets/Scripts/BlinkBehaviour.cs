using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkBehaviour : MonoBehaviour {

	public Color32 onColor = Color.white;	 
 	public Color32 offColor = Color.black;
	
	public static float freq = 3f; // blink every N_TILES / freq seconds
	static int totalCount = 0;
	int myIndex = 0;

	Renderer renderer; // if any
	
	// Use this for initialization
	// setting up tile position, angle, connectivity
	void Start () {
		
		foreach (Transform child in transform) {
			totalCount++;
			// print(totalCount);
			BlinkBehaviour bb = child.gameObject.AddComponent<BlinkBehaviour>();
			bb.myIndex = totalCount;
		}
		renderer = GetComponent<Renderer>();
	}

	// FixedUpdate is called once per frame
	// color and animations go here
	void FixedUpdate () {
		if (renderer) {
			// simple counter, tunrs on if time modulus #tri is my id:
			int t = (int)(Time.time * freq);
			renderer.material.color = (t % totalCount == myIndex) ? onColor : offColor;
		}
	}
    
}
