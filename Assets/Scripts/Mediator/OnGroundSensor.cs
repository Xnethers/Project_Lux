using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour {

    //public CapsuleCollider capcol;
    public CharacterController chacon;
    public float offset = 0.1f;

    private Vector3 point1;
    private Vector3 point2;
    private float radius;
    public Collider[] outputCols;
    public int count = 0;

	// Use this for initialization
	void Awake () {
        chacon = GetComponentInParent<CharacterController>();
        radius = chacon.radius + 0.05f;//- 0.05
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        point1 = transform.position + transform.up * (radius - offset);
        point2 = transform.position + transform.up * (chacon.height - offset) - transform.up * radius;

        outputCols = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground","Wall","Sensor"));//踩到別的玩家頭上會FALL卡住
        count = outputCols.Length;
        if (outputCols.Length != 0)
        {
            
            foreach (var col in outputCols)
            {
                if(col == transform.GetComponent<Collider>())
                    count--;
                // print("collision : "+col.name );
            }
            if(count!=0)
                SendMessageUpwards("IsGround");//Send to ac
            else
                SendMessageUpwards("IsNotGround");
        }
        else {
            SendMessageUpwards("IsNotGround");
        }
        if(GetGroundDistance()>3)
            SendMessageUpwards("IsHighFall");
        else
            SendMessageUpwards("IsNotHighFall");
        // if(GetGroundDistance()<offset)//0.15
        //     SendMessageUpwards("IsGround");//Send to ac
        // else 
        //     SendMessageUpwards("IsNotGround");
    }
    public float GetGroundDistance(){//取得玩家與下方地面的距離
		Vector3 targetPoint;
		RaycastHit hit;
		if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.down), out hit))
		{
			targetPoint = hit.point;
		}
		else
		{
			targetPoint = Vector3.zero;
		}
		Debug.DrawRay(transform.position,transform.TransformDirection(Vector3.down) * 100,Color.green);
		return Vector3.Distance(transform.position,targetPoint);
		
		
	}

}
