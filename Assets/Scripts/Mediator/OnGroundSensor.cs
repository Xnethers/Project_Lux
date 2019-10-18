using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour {
    public bool isStuck;
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
        radius = chacon.radius - 0.05f;//+ 0.05
        StartCoroutine(Stuck());
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if(isStuck){//如果卡住,切換成在地上
            SendMessageUpwards("IsGround");
        }
        else
        {//如果沒卡住,以正常collider偵測去判定
            point1 = transform.position + transform.up * (radius - offset);
            point2 = transform.position + transform.up * (chacon.height - offset) - transform.up * radius;
            // outputCols = Physics.OverlapBox(transform.position+transform.up*offset,new Vector3(chacon.radius*2,chacon.radius,chacon.radius*2),Quaternion.identity,LayerMask.GetMask("Ground","Wall","Sensor"));
            outputCols = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground","Wall","Sensor"));//踩到別的玩家頭上會FALL卡住
            count = outputCols.Length;
            if (outputCols.Length != 0)
            {
                foreach (var col in outputCols)
                {
                    if(col == transform.GetComponent<Collider>())//如果是自己
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
        }
        SendMessageUpwards("SetHeight",GetGroundDistance());
        // if(GetGroundDistance() > 3)
        //     SendMessageUpwards("IsHighFall");
        // else
        //     SendMessageUpwards("IsNotHighFall");
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
    IEnumerator Stuck(){//判定是否卡住了
        while(true){
            float originY= transform.position.y;
            yield return new WaitForSeconds(0.05f);
            if(transform.position.y == originY){
                isStuck=true;
            }
            else{
                isStuck=false;
            }
            //Debug.Log(isStuck);
        }
    }
    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
    //      //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
    //     if (m_Started)
    //         Gizmos.DrawWireCube(transform.position+transform.up*offset,new Vector3(chacon.radius*2,chacon.radius,chacon.radius*2));
    // }
}
