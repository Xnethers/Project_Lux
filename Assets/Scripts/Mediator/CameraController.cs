using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class CameraController : MonoBehaviourPunCallbacks {
    [Space(10)]
    [Header("===== Base Settings =====")]
    public ActorController ac;
    public IUserInput pi;
    public Image lockDot;
    
    public float HorizontalSpeed=100f;
    public float VerticalSpeed = 80f;
    public float minVerAngle = -30f;
    public float maxVerAngle = 30f;
    // public float camDampXZValue=0.1f;
    // public float camDampYValue=0.01f;
    [Space(10)]
    [Header("===== Offest Settings =====")]
    public float offsetXDistanceConstant=20f;
    public float offsetZDistance =2.5f;
    public float offsetZAimDis =0.5f;
    public float offsetYDistance =1.6f;
    public float offsetYAimDis =1.3f;
    public float offsetXDistance =0.6f;
    public float offestDampValue = 0.2f;
    public float offestYDampValue =0.05f;

    public bool isCursorVisible;

    private GameObject playerHandle;
    public GameObject cameraHandle;
    public float tempEulerX;
    private GameObject model;
    private GameObject mainCamera;
    private ChangeIgnoreRayLayer mainCamCollider;
    // private float camDampXVelocity;
    // private float camDampYVelocity;
    // private float camDampZVelocity;
    private Vector3 cameraDampVelocity;
    private float offestXDampVelocity;
    private float offestZDampVelocity;
    private float offestX;
    private float offestZ;
    Vector3 playerHead;
    public bool rayHit;
    // public float radius =0.1f;
    private float tempHitDistance = -2.0f;
    //no
    public LayerMask layerMask;
    //Die
    public bool isDieChange = false;
    public bool isDieInvoke = false;
    public bool isHorizontalView = false; 
    [Space(10)]
    [Header("===== Aim Settings =====")]
    public bool doAim;
    [SerializeField]private float offestY;
    private float offestYDampVelocity;
    [Space(10)]
    [Header("===== Latent Settings =====")]
    public float latentHalfAngle=100f;
    public float angle;
    public float offsetYLatentDis =1.35f;
	// Use this for initialization
	void Start () {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;
        offestX=offsetXDistance;
        offestY = offsetYDistance;
        cameraHandle.transform.localPosition = new Vector3(offsetXDistance,cameraHandle.transform.localPosition.y,cameraHandle.transform.localPosition.z);
        ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        pi = ac.pi;

        mainCamera = Camera.main.gameObject;
        mainCamCollider = mainCamera.GetComponentInChildren<ChangeIgnoreRayLayer>();
        //lockDot.enabled = false;
        isCursorVisible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible=false;
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(ac.pi.isAI)
            return;
        if(!ac.am.photonView.IsMine)
            return;
        // CursorControl();
        if(isHorizontalView){
            pi.Jup = 0f;
            tempEulerX =Mathf.Lerp(tempEulerX,0,.1f);
        }
        //攝影機旋轉
        playerHandle.transform.Rotate(Vector3.up, pi.Jright * HorizontalSpeed * Time.fixedDeltaTime);//直接旋轉玩家水平角度
        /*if(ac.pi.isLatent){
            // if(ac.am.im.overlapEcastms[0].latentType == LatentType.Vertical)
            // Vector3 relative = playerHandle.transform.InverseTransformPoint (ac.am.im.overlapEcastms[0].transform.position);
            // float angle = Mathf.Atan2 (relative.x, relative.z) * Mathf.Rad2Deg;
            
            // float angle=Mathf.Acos( Vector3.Dot(playerHandle.transform.forward,-ac.am.im.overlapEcastms[0].transform.forward))*Mathf.Rad2Deg;
            float angle = Vector3.Angle(playerHandle.transform.forward,-ac.am.im.overlapEcastms[0].transform.forward);
            Debug.Log("角度"+angle+",轉向"+pi.Jright);
            
            if(Mathf.Abs(angle)<latentHalfAngle || (angle>=latentHalfAngle && pi.Jright>0) || (angle<=-latentHalfAngle && pi.Jright<0))
                playerHandle.transform.Rotate(Vector3.up, pi.Jright * HorizontalSpeed * Time.fixedDeltaTime);//直接旋轉玩家水平角度
        }
        else{
            playerHandle.transform.Rotate(Vector3.up, pi.Jright * HorizontalSpeed * Time.fixedDeltaTime);//直接旋轉玩家水平角度
        }*/
        if(!ac.am.sm.isDie){//isDieChange
            tempEulerX -= pi.Jup * VerticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, minVerAngle, maxVerAngle);
        }
        else{
            isHorizontalView=false;
            tempEulerX =Mathf.Lerp(tempEulerX,90,.1f);
            tempEulerX = Mathf.Clamp(tempEulerX, minVerAngle, 90);
        }
        cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);
        mainCamera.transform.localEulerAngles = new Vector3(cameraHandle.transform.localEulerAngles.x,  playerHandle.transform.localEulerAngles.y, 0);
        
        //攝影機位置
        //相機與目標點距離
        CameraRay();
        if(doAim)
        {
            offestZ = Mathf.SmoothDamp(transform.localPosition.z, -offsetZAimDis, ref offestZDampVelocity, offestDampValue);
        }
        else{
            if (!rayHit) {
                offestZ = Mathf.SmoothDamp(transform.localPosition.z, -offsetZDistance, ref offestZDampVelocity, offestDampValue);
            }
            else {
                offestZ = Mathf.SmoothDamp(transform.localPosition.z, -tempHitDistance, ref offestZDampVelocity, offestDampValue);
            }
        }
        if(ac.pi.isLatent){
            angle = Vector3.Angle(playerHandle.transform.forward,-ac.am.im.overlapEcastms[0].transform.forward);
            // Debug.Log("角度"+angle);
            if(angle>latentHalfAngle){
                offsetZDistance = Mathf.Lerp(offsetZDistance,-0.5f,.3f);
            }
            else
                offsetZDistance = Mathf.Lerp(offsetZDistance,2.5f,.3f);
        }
        else{
            offsetZDistance=2.5f;
        }
        offestZ = Mathf.Clamp(offestZ, -offsetZDistance, 1);//0

        transform.localPosition = new Vector3(0, 0, offestZ);
        
        if(ac.am.sm.isDie){
            if(!isDieChange){
                if(ac.am.sm.isGround)
                    Invoke("DieSmoothCam",1f);//2
                else
                    Invoke("DieSmoothCam",2f);
                isDieChange=true;
            }
            if(isDieInvoke)
                mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, transform.position+transform.up/2, ref cameraDampVelocity, offestDampValue*2);
            else
                mainCamera.transform.position = transform.position;
            //mainCamera.transform.position = new Vector3(transform.position.x,Mathf.SmoothDamp(mainCamera.transform.position.y,transform.position.y,ref camDampYVelocity, offestDampValue*2),transform.position.z + 0.5f);
        }
        else{
            if(isDieChange){
                ReLiveCam();
                isDieChange=false;
            }
            mainCamera.transform.position = transform.position;
        }
              
        // mainCamera.transform.position = new Vector3(Mathf.SmoothDamp(mainCamera.transform.position.x,transform.position.x,ref camDampXVelocity, camDampXZValue),
        //     Mathf.SmoothDamp(mainCamera.transform.position.y,transform.position.y,ref camDampYVelocity, camDampYValue),Mathf.SmoothDamp(mainCamera.transform.position.z,transform.position.z,ref camDampZVelocity, camDampXZValue));
        //mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, transform.position, ref cameraDampVelocity, offestDampValue);
        //mainCamera.transform.LookAt(cameraHandle.transform);
        
        //改變看向目標點的位置
        SetOffsetX();
        SetOffestY();
    }
    public void DoAim(){
        doAim=true;
        // if(!doAim){
        //     Camera.main.fieldOfView -= 45;
        //     HorizontalSpeed /= 5f;
        //     VerticalSpeed /= 5f;
        //     doAim=true;
        // }
    }
    public void DoUnAim(){
        doAim=false;
        // if(doAim){
        //     Camera.main.fieldOfView += 45;
        //     HorizontalSpeed *= 5f;
        //     VerticalSpeed *= 5f;
        //     doAim=false;
        // }
    }
    private void SetOffsetX(){//改變看向目標點的位置
        // Collider[] hitColliders = Physics.OverlapBox(playerHandle.transform.position +transform.up, new Vector3(1,.1f,1),Quaternion.identity, layerMask);
        // if(hitColliders.Length>0){
        //     //camDampYValue=Mathf.Lerp(camDampYValue,0.001f,.7f);
        //     offestX = Mathf.Lerp(offestX,0.001f,offestDampValue);
        // }
            
        // else {
        //     //camDampYValue=camDampXZValue;
        //     offestX = Mathf.Lerp(offestX,offsetXDistance,offestDampValue);
        // }
        if(!ac.am.sm.isDie){
            if(!doAim)
                offestX=(offsetZDistance/offsetXDistance)*tempHitDistance/offsetXDistanceConstant;//Mathf.Abs(offestZ)/17
            else
                offestX=offsetXDistance;
        }
        // Debug.Log(offestX);
        offestX = Mathf.Clamp(offestX,0,offsetXDistance);
        cameraHandle.transform.localPosition = new Vector3(Mathf.SmoothDamp(cameraHandle.transform.localPosition.x, offestX, ref offestXDampVelocity, offestDampValue), 
        Mathf.SmoothDamp(cameraHandle.transform.localPosition.y,offestY, ref offestYDampVelocity, offestYDampValue), cameraHandle.transform.localPosition.z);//cameraHandle.transform.localPosition.y
    }
    public void SetOffestY(){
        if(ac.pi.isLatent){
            offestY = Mathf.Lerp(offestY,offsetYLatentDis,.1f);
        }
        else{
            if(!doAim){
                offestY = Mathf.Lerp(offestY,offsetYDistance,.1f);
            }
            else{
                offestY = Mathf.Lerp(offestY,offsetYAimDis,.1f);
            }
        }
    }
    private void CameraRay()//AutoDetection
    {
        //LayerMask mask = 1<<LayerMask.NameToLayer("Default")|1<<LayerMask.NameToLayer("Wall")|1<<LayerMask.NameToLayer("Ground");
        // Debug.Log("mask"+mask);
        // Debug.Log("layermask"+layerMask);
        if(!ac.pi.isLatent)
            playerHead = ac.transform.position+new Vector3(0,ac.chacon.height,0);
        else{
            if(ac.latentType == LatentType.Horizontal)
                playerHead = ac.transform.position;
            else
                playerHead = ac.transform.position+new Vector3(0,ac.am.bm.OriginColValue[2]/2,0);
        }
            
        Vector3 dir = transform.position - playerHead;
        RaycastHit hit;
        if (Physics.Raycast(playerHead, dir * 100, out hit,100, layerMask))//cameraHandle.transform.position, mainCamera.transform.TransformDirection(-Vector3.forward)
        {
            tempHitDistance = hit.distance / 2;
            rayHit = true;
        }
        else
        {
            rayHit = false;
        }

        if (tempHitDistance > offsetZDistance && rayHit == true)
        {
            rayHit = false;
        }
        Debug.DrawRay(playerHead, dir * 100,Color.green);
        
    }
    public void DieSmoothCam(){
        //transform.localPosition = new Vector3(0, 0,  offsetZDistance);
        offsetZDistance*=2;
        offestDampValue*=5;
        isDieInvoke = true;
    }
    public void ReLiveCam(){
        offsetZDistance/=2;
        offestDampValue/=5;
        isDieInvoke =false;
    }
    public void CursorControl(){
        // if(Global.Level==0){
        //     return;
        // }
        if(isCursorVisible){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible=true;
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible=false;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(tempEulerX);
        }
        else
        {
            // Network player, receive data
            this.tempEulerX = (float)stream.ReceiveNext();
        }
    }
}
