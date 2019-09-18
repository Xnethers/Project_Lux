using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
namespace Com.MyCompany.MyGame
{
    public class Sworder : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields
        [SerializeField] Transform shooter;
        [SerializeField] private float ComboTime = 0.3f;

        private bool isComboTime = false;
        private float ComboTime_Time = 0;

        [SerializeField] private int Combo = 0;

        private int ComboLenth = 3;
        private Animator animator;

        [SerializeField] private List<GameObject> atczone;

        #endregion


        // Use this for initialization
        void Start()
        {
            animator = this.GetComponent<Animator>();
        }


        // Update is called once per frame
        void Update()
        {

            //技能完畢後,可以在接續下段攻擊的時間
            if (isComboTime)
            {
                ComboTime_Time += Time.deltaTime;
                if (ComboTime_Time > ComboTime)
                {
                    //Debug.Log("超出Combo時間,重製攻擊順序");
                    ResetCombo();
                    isComboTime = false;
                }
            }


            if (photonView.IsMine)
            {
                switch (this.Combo)
                {
                    case 1:
                        {
                            animator.SetInteger("Chop", Combo);
                            isComboTime = true;

                        }
                        break;
                    case 2:
                        {
                            animator.SetInteger("Chop", Combo);
                            isComboTime = true;
                        }
                        break;
                    case 3:
                        {
                            animator.SetInteger("Chop", Combo);
                            isComboTime = true;
                        }
                        break;
                }
                //show_damage(_d,Player_target._target.position);
                if (Input.GetButtonDown("Fire1"))
                {
                    GetComponent<PhotonView>().RPC("AddCombo", RpcTarget.All);
                }
            }

        }

        #region Custom
        [PunRPC]
        void AddCombo()
        {
            if (Combo < ComboLenth)
            {
                Combo++;
                GameObject a = (GameObject)Instantiate(atczone[Combo - 1], shooter.position, transform.rotation);
                if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
                    a.tag = "Red";
                else if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
                    a.tag = "Blue";
                Physics.IgnoreCollision(a.GetComponent<Collider>(), transform.GetComponent<Collider>());
            }
            else if (Combo == ComboLenth)
            {
                Combo = 1;
                GameObject a = (GameObject)Instantiate(atczone[Combo - 1], shooter.position, transform.rotation);
                if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
                    a.tag = "Red";
                else if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
                    a.tag = "Blue";
                Physics.IgnoreCollision(a.GetComponent<Collider>(), transform.GetComponent<Collider>());
            }
            isComboTime = false;
            ComboTime_Time = 0;
        }

        //重製攻擊段數
        [PunRPC]
        void ResetCombo()
        {
            Combo = 0;
            animator.SetInteger("Chop", Combo);
        }

        [PunRPC]
        private void CreatAttackZone()
        {
            GameObject a = (GameObject)Instantiate(atczone[Combo - 1], shooter.position, transform.rotation);
            if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
                a.tag = "Red";
            else if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
                a.tag = "Blue";
            Physics.IgnoreCollision(a.GetComponent<Collider>(), transform.GetComponent<Collider>());
        }
        #endregion

        #region PUN Callbacks

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(Combo);
            }
            else
            {
                // Network player, receive data
                this.Combo = (int)stream.ReceiveNext(); Debug.Log(stream.ReceiveNext());
            }
        }
        #endregion
    }
}