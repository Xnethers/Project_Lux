using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTest : MonoBehaviour {
	public float bonceVelocity = 5.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other)
	{
		ActorManager am = other.GetComponent<ActorManager>();
		if(am != null)
		{
			//am.ac.pi.inputEnabled = true;
			am.ac.BounceTrigger(bonceVelocity);
			// am.ac._velocity=Vector3.zero;
			// am.ac._velocity.y += Mathf.Sqrt(bonceVelocity * -0.5f * Physics.gravity.y);
		}
	}
}
