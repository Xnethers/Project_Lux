using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{

    public class TowerKey : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields
        [SerializeField] bool CanPass; //可交易狀態
        [SerializeField] float range;
        [SerializeField] RaycastHit[] a;
        LayerMask mask;

        [SerializeField] GameObject target;
        [SerializeField] bool haveKey;
        #endregion

        #region Public Fields

        public Text i;

        //擁有鑰匙
        public bool HaveKey
        {
            get { return haveKey; }
            set { haveKey = value; }
        }

        #endregion

        // Use this for initialization
        void Start()
        {
            mask = LayerMask.GetMask("Player");
        }

        // Update is called once per frame
        void Update()
        {

            if (photonView.IsMine)
            {
                if (this.HaveKey && i)
                { i.text = "Have Key"; }
                else
                { i.text = "No Key"; }
                
                if (CanPass && Input.GetKeyDown(KeyCode.R))
                {
                    GetComponent<PhotonView>().RPC("Passkey", RpcTarget.All);
                }
            }
        }

        void LateUpdate()
        {
            a = Physics.SphereCastAll(transform.position, range, Vector3.forward, 0, mask);
            if (a != null && GetClosestMate(a) && HaveKey == true)
            { CanPass = true; target = GetClosestMate(a); Debug.Log(GetClosestMate(a).GetComponent<PhotonView>().Owner.NickName); }
            else
            { CanPass = false; target = null; }

        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(HaveKey);
            }
            else
            {
                // Network player, receive data
                this.HaveKey = (bool)stream.ReceiveNext();
            }
        }

        GameObject GetClosestMate(RaycastHit[] mates)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (RaycastHit potentialTarget in mates)
            {
                if (potentialTarget.collider != this.GetComponent<Collider>() && potentialTarget.collider.tag == this.tag)
                {
                    Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget.collider.gameObject;
                    }
                }
                else
                { }
            }

            return bestTarget;
        }

        [PunRPC]
        void Passkey()
        {
            HaveKey = false;
            target.GetComponent<PhotonView>().RPC("Receivekey", RpcTarget.All);
        }

        [PunRPC]
        void Receivekey()
        { HaveKey = true; }
    }
}