using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Xml.Linq;
using System;
public abstract class ICareerController : MonoBehaviourPunCallbacks{
	protected ActorController ac;
	public LayerMask ignoreLayerMask;
	public CareerValue careerValue;
	public GameObject[] projectile;
	public GameObject[] comboParticle;
    protected Transform muzzle;
	protected Transform muzzleR;
	public bool rayhitAirWall;
	protected delegate void MyDelegate();
	//CD
	public MySkillTimer skillF = new MySkillTimer();
    public MySkillTimer skillQ = new MySkillTimer();
    public MySkillTimer skillAir = new MySkillTimer();
    public MySkillTimer skillForce = new MySkillTimer();
    public MyTimer forcingTimer = new MyTimer();
	public virtual void NormalAttack()//普攻
	{ }

	public virtual void FirstAttack()//1技
	{ }

	public virtual void SecondAttack()//2技
	{ }

	public virtual void RushAttack()//大招
	{ }

	public virtual void AirAttack()//空技
	{ }

	public virtual void ForceAttack()//蓄力
	{ }
	protected void UseSkill(int attackSkill,float ATK,string triggerName = "attack",bool comboAttack = false){//動作+無CD技能
		ac.am.sm.ATK = ATK;
        photonView.RPC("RPC_SetTrigger", RpcTarget.All, triggerName);
        ac.anim.SetInteger("attackSkill", attackSkill);
		ac.canAttack = comboAttack;
    }

    // protected void UseSkill(int attackSkill,float ATK, MySkillTimer timer, float duration){//動作+有CD的技能
    //     //Check CD time.
    //     if (timer.FinishedCD)
    //     {
	// 		ac.am.sm.ATK = ATK;
    //         photonView.RPC("RPC_SetTrigger", RpcTarget.All, "attack");
    //         ac.anim.SetInteger("attackSkill", attackSkill);
            
    //         timer.duration = duration;
    //         timer.isSkilling = true;
    //         ac.canAttack = false;
    //     }
    // }
	// protected void UseSkill(int attackSkill,float ATK, MySkillTimer timer, float duration,MyDelegate doFuntion){//無動作+CD的技能+委派
    //     //Check CD time.
    //     if (timer.FinishedCD)
    //     {
	// 		ac.am.sm.ATK = ATK;
	// 		ac.anim.SetInteger("attackSkill", attackSkill);
    //         timer.duration = duration;
    //         timer.isSkilling = true;
	// 		doFuntion();
    //     }
    // }
	protected void StartCD(MySkillTimer timer, float CDTime){
        //Check CD time.
        if (timer.FinishedCD)
        {
            timer.duration = CDTime;
            timer.isSkilling = true;
        }
    }
	public bool CheckCD(MySkillTimer timer){
        //Check CD time.
        if (timer.FinishedCD)
        {
			return true;
        }
		else
		{
			return false;
		}
    }
	
	public Vector3 RayAim(){//取得準心目標點
		Vector3 targetPoint;
		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 2f)),Camera.main.transform.TransformDirection(Vector3.forward), out hit, 100,~ignoreLayerMask))//如果射線碰撞到物體
		{
			targetPoint = hit.point;//記錄碰撞的目標點
			Debug.Log(hit.collider.name);
			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("AirWall"))
			{rayhitAirWall = true;}
			else
			{
				rayhitAirWall =false;
			}
		}
		else//射線没有碰撞到目标点
		{
			//將目標點設置在攝影機自身前方1000米处
			targetPoint = Camera.main.transform.forward * 1000 ;
			rayhitAirWall =false;
		}
		Debug.DrawRay(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 2f)),Camera.main.transform.TransformDirection(Vector3.forward) * 100,Color.red);
		return targetPoint;
	}
	
	#region RPC
    [PunRPC]
    public void RPC_Projectile(Vector3 originVec3,Vector3 targetPoint,float throwerPower){//生成具體攻擊(特效)
        GameObject bullet = Instantiate(projectile[ac.anim.GetInteger("attackSkill")], originVec3, transform.rotation) as GameObject;
        bullet.transform.LookAt(targetPoint);
        foreach (Projectile projectile in bullet.GetComponentsInChildren<Projectile>())
        {
            projectile.Initialize(ac.am,throwerPower,targetPoint);
        }
    }
	[PunRPC]
    public void RPC_NearProjectile(Vector3 originVec3,int combo){//生成近戰combo具體攻擊(特效)
        GameObject bullet = Instantiate(comboParticle[combo], originVec3, transform.rotation) as GameObject;
        foreach (Projectile projectile in bullet.GetComponentsInChildren<Projectile>())
        {
            projectile.Initialize(ac.am,0,Vector3.zero);
        }
    }
	
	#endregion
}
 public class MySkillTimer{//技能CD計時器
	public bool  FinishedCD= true;//canAttackSkill
	public bool isSkilling = false;
	public float duration;
	public MyTimer atkTimer = new MyTimer();
	public void Tick() {
		atkTimer.Tick();
		if(isSkilling){
			StartTimer(atkTimer,duration);	
			isSkilling = false;
		}
		if (atkTimer.state == MyTimer.STATE.RUN)
			FinishedCD=false;
		else
			FinishedCD=true;
	}
	private void StartTimer(MyTimer timer, float duration) {
        timer.Go(duration);
    }
}
