using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour {

    const int TRI_IN_TILE = 50;
    const int TRI_IN_ROW = 5;

    public GameObject tri_prefab;

    public GameObject[] triangles = new GameObject[TRI_IN_TILE];

	// Use this for initialization
	// setting up tile position, angle, connectivity
	void Start () {
        /*Quaternion triLookUp = Quaternion.Euler(new Vector3(90, 0, 0));
        Quaternion triLookDown = Quaternion.Euler(new Vector3(270, 0, 0));
        for (int i=0; i< TRI_IN_TILE; i++)
        {
            int rowInt = (i / TRI_IN_ROW);
            float row = (float)rowInt;
            bool isUp = i % (2 * TRI_IN_ROW) < TRI_IN_ROW;
            float col = (float)i % TRI_IN_ROW;
            if(isUp)
            {
                col = (TRI_IN_ROW - 1 - col) + 0.5f;
                row = row - 0.5f;
            }
            if(rowInt % 4 == 2)
            {
                col = col - 0.5f;
            }
            if (rowInt % 4 == 3)
            {
                col = col + 0.5f;
            }
            Quaternion triQuaternion = isUp ? triLookUp : triLookDown;
            GameObject prefab = (GameObject)Resources.Load("Triangle");
            triangles[i] = (GameObject) Instantiate(prefab, new Vector3(col / 9.0f, row / 17.0f, isUp ? 0.0f : 0.05f), triQuaternion);
            triangles[i].transform.parent = this.transform;
            triangles[i].name = "Tri" + i;
        }*/
	}

	// Update is called once per frame
	// color and animations go here
	void Update () {
		foreach (Transform child in transform) {
			child.GetComponent<Renderer>().material.color = new Color32(1, 234, 32, 128); // example
		}
	}
}
