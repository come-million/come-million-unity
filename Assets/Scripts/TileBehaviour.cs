using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour {

    // Use this for initialization
    // setting up tile position, angle, connectivity
    void Start () {
       foreach (Transform child in transform) {
            child.position += Vector3.up * (child.position.y + child.position.x / 2.0f); // example
        } 		
    }

    // Update is called once per frame
    // color and animations go here
    void Update () {
        foreach (Transform child in transform) {
            child.GetComponent<Renderer>().material.color = new Color32(1, 234, 32, 128); // example
        }
    }
}
