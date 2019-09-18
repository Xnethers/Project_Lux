using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class Sniper : SkillBasicData//, IPunObservable
    {
        #region Private Fields
        [SerializeField] private Rigidbody bullet;
        [SerializeField] private Transform shooter;
        [SerializeField] private float delaytime = 0.08f;

        [SerializeField] private int ThrowerPower = 50;
        [SerializeField] bool IsFiring;

        [Header("甩槍")]
        RaycastHit m_Hit;
        [SerializeField] private float s_dis;



        #endregion

        // Use this for initialization
        void Start()
        {
            animator = this.GetComponent<Animator>();
            camerawork = this.GetComponent<CameraWork>();
        }

        // Update is called once per frame
        void Update()
        {
            Rayaim();

            if (photonView.IsMine)
            {
                // trigger bullet active state
                if (bullet != null && !IsFiring && Input.GetButton("Fire1"))
                {
                    IsFiring = true;
                    GetComponent<PhotonView>().RPC("Normalattack", RpcTarget.All);
                }

                if (Input.GetButton("Fire2"))
                {
                    camerawork.Aim();
                }
                else
                {
                    if (camerawork.isaim())
                    { camerawork.unAim(); }
                }

                if (Input.GetKeyDown(KeyCode.F))
                { GetComponent<PhotonView>().RPC("Thirdattack", RpcTarget.All); }

                // When using trigger parameter
                if (Input.GetButton("Fire1"))
                { animator.SetBool("shoot", true); }
                if (Input.GetButtonUp("Fire1"))
                { animator.SetBool("shoot", false); }
            }
        }

        #region Skill Program

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>

        [PunRPC]
        public override void Normalattack()
        {
            Rigidbody Bullet = (Rigidbody)Instantiate(bullet, shooter.position, transform.rotation);
            Bullet.transform.LookAt(targetPoint);
            Bullet.velocity = Bullet.transform.forward * ThrowerPower;//transform.TransformDirection(new Vector3(0, 0, ThrowerPowerZ));
            Physics.IgnoreCollision(Bullet.GetComponent<Collider>(), transform.GetComponent<Collider>());
            StartCoroutine("Shoot");
        }

        [PunRPC]
        /*甩槍擊退近身敵人。槍托產生小型光爆，具有傷害的閃光效果（單一目標，持續時間短） */
        public override void Thirdattack()//3技
        {
            bool HitDetect = Physics.BoxCast(transform.position + new Vector3(0, 1, 0), Vector3.one / 2, transform.forward, out m_Hit, Quaternion.identity, 1, LayerMask.NameToLayer("Player"));
            if (HitDetect && m_Hit.collider.tag != this.transform.tag)
            {
                Debug.Log("1");
                m_Hit.collider.GetComponent<PhotonView>().gameObject.GetComponent<CharacterController>().Move(this.transform.forward * 2f * Time.deltaTime);
            }
            //&& m_Hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")
        }
        #endregion

        private bool istarget;
        private RaycastHit hitInfo;
        private Vector3 targetPoint;


        #region Subprogram

        void Rayaim()
        {
            //通过摄像机在屏幕中心点位置发射一条射线
            //Physics.Raycast(shooter.position, camerawork.aimdir, out hitInfo);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
            if (Physics.Raycast(ray, out hitInfo))//如果射线碰撞到物体
            {
                targetPoint = hitInfo.point;//记录碰撞的目标点
            }
            else//射线没有碰撞到目标点
            {
                //将目标点设置在摄像机自身前方1000米处
                targetPoint = Camera.main.transform.forward * 1000;
            }
        }

        IEnumerator Shoot()
        {
            yield return new WaitForSeconds(delaytime);
            IsFiring = false;
        }

        #endregion

        #region PUN Callbacks
        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
            }
        }
        #endregion

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            //Check if there has been a hit yet

            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0), transform.forward * m_Hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0) + transform.forward * m_Hit.distance, transform.localScale);

            //If there hasn't been a hit yet, draw the ray at the maximum distance
        }

    }
}
