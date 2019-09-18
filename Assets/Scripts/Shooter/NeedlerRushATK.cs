using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedlerRushATK : Projectile {
	[Space(10)]
    [Header("===== NeedlerRushATK Settings =====")]
	public GameObject beeHitVFX;
	
	[Space(10)]
    [Header("===== Throw Settings =====")]
	public NeedlerController nc;
	public bool activated;
	public float rotationSpeed = 100f;
	public bool isReturning = false;
    private float time=0.0f;
	public Transform target,curve_point;
    private Vector3 old_pos;
	/* public override void Initialize(ActorManager am,float speed,Vector3 targetPoint){
		base.Initialize(am,speed,targetPoint);
		nc = am.ac.GetComponent<NeedlerController>();
		this.target=nc.target;
		this.curve_point = nc.curve_point;
		Invoke("ReturnNeedle",nc.pullTime);
	}
	void Update()
	{
		// if(Input.GetKeyDown(KeyCode.M))
        //     ReturnNeedle();
		if(activated)
		{
			//transform.localEulerAngles+=transform.forward*rotationSpeed*Time.deltaTime;
			//rb.velocity = rb.transform.forward * speed;

			//transform.localEulerAngles= new Vector3(0, -90 +transform.eulerAngles.y, 0) * rotationSpeed;
		}
		if(isReturning){
            if(time<1.0f){
                rb.position = getBQCPoint(time,old_pos,curve_point.position,target.position);
                rb.rotation = Quaternion.Slerp(rb.transform.rotation,target.rotation,50*Time.deltaTime);
                time+=Time.deltaTime;
            }else{
                ResetNeedle();
            }
        }
	}*/
	public override void AdditionalAttack(Collider col){
		Debug.Log("overrideAdditionalAttack");
		// rb.velocity=Vector3.zero;
		if(beeHitVFX !=null  && !isVFX){
			GameObject vfx = Instantiate(beeHitVFX,targetPoint,transform.rotation) as GameObject;
			vfx.transform.SetParent(col.transform);
			isVFX=true;
		}
		this.col = col;
		float t=0.5f;
		//float d=beeHitVFX.GetComponent<ParticleSystem>().main.duration/12;
		for(int i = 0;i<12;i++){
			Invoke("Invoke_TryDoDamage",t);
			t+=0.2f;
			//Invoke("Invoke_TryDoDamage",Random.Range(2.5f,GetComponent<ParticleSystem>().main.duration-0.5f));//錯
			//Invoke("Invoke_TryDoDamage",Random.Range(0.5f,beeHitVFX.GetComponent<ParticleSystem>().main.duration-0.5f));
		}
	}
	public void Invoke_TryDoDamage()
    {
		col.SendMessageUpwards("TryDoDamage",am.ac.careercon.careerValue.RushAddDamage);
        //Debug.Log("RushAddAttack" + am.sm.ATK);
    }
	/*public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		transform.GetChild(0).gameObject.SetActive(true);
		if(!isReturning)
			Invoke("ReturnNeedle",.2f);
		// activated = false;
		// rb.isKinematic=true;
		// isReturning=true;
	}
	//大型針
	public void ReturnNeedle(){
		if(isReturning)
			return;
        activated=true;
        time=0.0f;
		old_pos = rb.position;
        isReturning=true;
        rb.velocity=Vector3.zero;
        rb.isKinematic=true;
    }
    void ResetNeedle(){
        activated=false;
        isReturning=false;
        //rb.transform.parent=am.wm.whR.transform;
        rb.transform.localPosition=target.transform.localPosition;
        rb.transform.localRotation=target.transform.localRotation;
		rb.gameObject.SetActive(false);
		//顯示武器
		// nc.ArmourNoPull();
    }
	//移動軌跡
    Vector3 getBQCPoint(float t,Vector3 p0,Vector3 p1,Vector3 p2){
        float u =1-t;
        float tt = t*t;
        float uu= u*u;
        Vector3 p =(uu *p0)+(2*u*t*p1)+(tt*p2);
        return p;
    }*/
}
