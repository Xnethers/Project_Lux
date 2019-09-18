using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterObjRotate : MonoBehaviour {
    private float tempEulerX;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0)) {
            transform.Rotate(-Vector3.up, (Input.GetAxis("Mouse X")) * 100f * Time.deltaTime);
        }
	}
}
