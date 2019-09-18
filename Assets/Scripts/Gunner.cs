using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

namespace Com.MyCompany.MyGame
{
    public class Gunner : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields
        [Tooltip("The AxeSource GameObject to control")]
        [SerializeField] private Rigidbody bullet;
        [SerializeField] private Transform shooter;
        [SerializeField] private float delaytime = 0.08f;

        private int ThrowerPowerZ = 10;

        [SerializeField] bool IsFiring;
        Animator animator;

        #endregion

        // Use this for initialization
        void Start()
        {
            animator = this.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

            if (photonView.IsMine)
            {
                // trigger bullet active state
                if (bullet != null && !IsFiring && Input.GetButton("Fire1"))
                {
                    IsFiring = true;
                    StartCoroutine("Shoot");
                }

                // When using trigger parameter
                if (Input.GetButton("Fire1"))
                { animator.SetBool("shoot", true); }
                if (Input.GetButtonUp("Fire1"))
                { animator.SetBool("shoot", false); }
            }
        }

        #region Custom

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>

        [PunRPC]
        private void MissileActive()
        {
            Rigidbody Bullet = (Rigidbody)Instantiate(bullet, shooter.position, transform.rotation);
            if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
                Bullet.tag = "Red";
            else if(PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
                Bullet.tag = "Blue";
            Bullet.velocity = shooter.forward * ThrowerPowerZ;//transform.TransformDirection(new Vector3(0, 0, ThrowerPowerZ));
            Physics.IgnoreCollision(Bullet.GetComponent<Collider>(), transform.GetComponent<Collider>());
        }

        IEnumerator Shoot()
        {
            yield return new WaitForSeconds(delaytime);
            GetComponent<PhotonView>().RPC("MissileActive", RpcTarget.All);
            IsFiring = false;
        }
        #endregion

        #region PUN Callbacks
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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

    }
}
