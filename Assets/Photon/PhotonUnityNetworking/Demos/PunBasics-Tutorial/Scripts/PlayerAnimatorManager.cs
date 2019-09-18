// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerAnimatorManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player Animator Component controls.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame//Photon.Pun.Demo.PunBasics
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        public bool TPS;
        #region Private Fields

        [SerializeField]
        private float directionDampTime = 0.25f;
        public Animator animator;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {

            // Prevent control is connected to Photon and represent the localPlayer
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            // failSafe is missing Animator component on GameObject
            if (!animator)
            {
                return;
            }

            // deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // only allow jumping if we are running.
            /* if (stateInfo.IsName("Base Layer.Run"))
            {
                // When using trigger parameter
                
            } */
            if (Input.GetKeyDown(KeyCode.Space)) animator.SetTrigger("Jump");
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            animator.SetBool("TPS",TPS);
            animator.SetFloat("Speed", h * h + v * v);
            if(TPS){
                animator.SetFloat("h",h);
                animator.SetFloat("v",v);
            }
            
            /* 

            // deal with movement
           

            // prevent negative Speed.
            if (v < 0)
            {
                v = 0;
            }

            // set the Animator Parameters
            
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
            */
        }
        
        #endregion
        public void Ani_Trigger(string tempAni){
            animator.SetTrigger(tempAni);
        }

    }
}