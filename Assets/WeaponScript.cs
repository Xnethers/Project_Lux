using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour {

	public bool activated;
	public float rotationSpeed;
	private float speed=50f;
	private Rigidbody rb;
	void Start()
	{
		rb=GetComponent<Rigidbody>();
	}
	void Update () {
		if(activated)
		{
			transform.localEulerAngles+=transform.forward*rotationSpeed*Time.deltaTime;
			//rb.velocity = rb.transform.forward * speed;
			transform.localEulerAngles= new Vector3(0, -90 +transform.eulerAngles.y, 0);
		}
		
	}
	private void OnTriggerEnter(Collider other)
	{
		//activated = false;
		GetComponent<Rigidbody>().isKinematic=true;
	}
}
