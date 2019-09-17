using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    bool initialize = false;

    GameObject Camera;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (initialize == false)
        {
            Camera = GameObject.FindGameObjectWithTag("MainCamera");

            initialize = true;
        }
        transform.LookAt(Camera.transform);
	}
}
