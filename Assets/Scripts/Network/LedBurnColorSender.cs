using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sends color buffer every frame
// traverse on all children and send a buffer to LedBurnClient
// the purpose is to attach this object to a tile object
// EW

public class LedBurnColorSender : MonoBehaviour {

    public LBClientSender ledBurnClient;
    
    void Start () {
        ledBurnClient = GetComponent<LBClientSender>();
    }

	void Update () {
        Color32[] colors = new Color32[transform.childCount];
        int i = 0;
        foreach (Transform child in transform) {
            colors[i] = child.GetComponent<Renderer>().material.color;
            i++;
		}

        ledBurnClient.SetData(0, 0, colors);
	}
}
