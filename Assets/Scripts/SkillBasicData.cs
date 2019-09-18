using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{

    public abstract class SkillBasicData : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields
        public Animator animator;
        public float distance;

        public GameObject PopupDamage;//傷害顯示

        [Header("冷卻時間")]
        public float CD;
        public float NowCD;
        [Header("消耗MP")]
        public int Cost;

        [Header("傷害數值")]
        [HideInInspector] public float Damage;

        [Header("控制接口")]
        public bool CanUseSkill = true;//可以使用此技能
        public bool isUse = false;//確認有使用技能

        #endregion

        #region Private Fields
        protected GameObject player;
        protected CameraWork camerawork;

        [SerializeField] protected bool isReadahead;

        #endregion


        void Awake()
        {
            //NowCD = 0;
        }
        void Start()
        {
            player = GameObject.FindObjectOfType<PhotonView>().gameObject;
            animator = player.GetComponent<Animator>();
            camerawork = player.GetComponent<CameraWork>();
        }
        void Update()
        { }



        public virtual void UseSkill()
        { isUse = true; }

        public virtual void Normalattack()//普攻
        { }

        public virtual void Secondattack()//2技
        { }

        public virtual void Thirdattack()//3技
        { }

        public virtual void Rushattack()//大招
        { }

        public virtual void Airattack()//空技
        { }

        public virtual void Forceattack()//蓄力
        { }

        public void CostMP()
        { }

        void StartReadAhead()
        {
            isReadahead = true;
        }

        void SkillEnd()
        {
            isReadahead = false;
        }

        //計算傷害
        public int CalculateDamege()
        {
            float CountDamage = 0;//最終傷害
                                  //最終傷害 = 角色攻擊數值 * 技能倍率 * 亂數平衡值(%)
            CountDamage = (float)Damage * Random.Range(1, 101) / 100;
            //Debug.Log(CountDamage);
            //最低傷害1
            if (CountDamage <= 1)
            { CountDamage = 1; }
            //爆擊觸發 亂數0~99 < 角色爆擊值
            if (Random.Range(0, 100) < Damage)
            {
                //爆擊 =  最終傷害 * 角色爆擊傷害%
                // CountDamage *= _ability.mCritDamage / 100;
            }


            return (int)CountDamage;
        }

        public void showDamage(int damage, Vector3 _position)
        {

        }

        public void StartCD()
        { CanUseSkill = false; isUse = false; }
        public float GetCDFlaot()
        { return NowCD / CD; }

        public void CDing()
        {
            if (!CanUseSkill)
            {
                isUse = false;
                NowCD += Time.deltaTime;
                if (NowCD >= CD)
                {
                    CanUseSkill = true;
                    NowCD = 0;
                }
            }
        }

        public abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
    }
}

