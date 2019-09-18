using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoubleGunRushATK : MonoBehaviourPunCallbacks {
    [Header("===== DoubleGunRushATK Settings =====")]
	public ActorManager am;
	public GameObject RushZoneVFX;
	
	[Space(10)]
    [Header("===== Throw Settings =====")]
	public DoubleGunController dc;
	public float rotationSpeed = 3f;
	public bool isReturning = false;
    private float time=0.0f;
	public Transform target,curve_point;
    private Vector3 old_pos;

	void Update()
	{
		// // if(Input.GetKeyDown(KeyCode.M))
        // //     ReturnNeedle();
		// if(activated)
		// {
		// 	//transform.localEulerAngles+=transform.forward*rotationSpeed*Time.deltaTime;
		// 	//rb.velocity = rb.transform.forward * speed;

		// 	//transform.localEulerAngles= new Vector3(0, -90 +transform.eulerAngles.y, 0) * rotationSpeed;
		// }
		// if(isReturning){
        //     if(time<1.0f){
        //         rb.position = getBQCPoint(time,old_pos,curve_point.position,target.position);
        //         rb.rotation = Quaternion.Slerp(rb.transform.rotation,target.rotation,50*Time.deltaTime);
        //         time+=Time.deltaTime;
        //     }else{
        //         ResetNeedle();
        //     }
        // }
	}
	// public override void AdditionalAttack(Collider col){
	// 	Debug.Log("overrideAdditionalAttack");
	// 	// rb.velocity=Vector3.zero;
	// 	if(beeHitVFX !=null  && !isVFX){
	// 		GameObject vfx = Instantiate(beeHitVFX,targetPoint,transform.rotation) as GameObject;
	// 		vfx.transform.SetParent(col.transform);
	// 		isVFX=true;
	// 	}
	// 	this.col = col;
	// 	for(int i = 0;i<12;i++){
	// 		Invoke("Invoke_TryDoDamage",Random.Range(2.5f,GetComponent<ParticleSystem>().main.duration-0.5f));
	// 	}
	// }
	// public void Invoke_TryDoDamage()
    // {
	// 	col.SendMessageUpwards("TryDoDamage",am.ac.careercon.careerValue.RushDamage);
    //     Debug.Log("RushAddAttack" + am.sm.ATK);
    // }
	// public void OnTriggerEnter(Collider other)
	// {
	// 	transform.GetChild(0).gameObject.SetActive(true);
		
			
	// }

}
