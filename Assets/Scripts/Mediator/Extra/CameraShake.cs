using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	public float duration = .2f;
	public float magnitude = .3f;
	public IEnumerator Shake(float duration,float magnitude){
		Vector3 originalPos = transform.localPosition;
		float elapsed = 0.0f;
		while(elapsed<duration){
			float x=Random.Range(-1f,1f)*magnitude;
			float y=Random.Range(-1f,1f)*magnitude;
			
			// transform.localPosition = new Vector3(x,y,originalPos.z);
			transform.localPosition = originalPos+new Vector3(x,y,0);
			
			elapsed+=Time.deltaTime;
			yield return null;

		}
		transform.localPosition =originalPos;
	}
	/*// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
	
	// How long the object should shake for.
	public float shakeDuration = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{
		if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;
		}
	}*/
}
