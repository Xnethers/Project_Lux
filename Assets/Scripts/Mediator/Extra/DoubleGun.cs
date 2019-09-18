using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class DoubleGun : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Private Fields
    [Tooltip("The AxeSource GameObject to control")]
    [SerializeField] private Rigidbody bullet;
    [SerializeField] private Transform shooter;
    [SerializeField] private float delaytime = 0.08f;

    private int ThrowerPowerZ = 10;

    [SerializeField] bool IsFiring;
    Animator animator;
    bool isleft;

    #endregion

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (photonView.IsMine)
        {
            // trigger bullet active state
            if (bullet != null && !IsFiring && Input.GetButton("Fire1") && isleft)
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

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(delaytime);
        GetComponent<PhotonView>().RPC("MissileActive", RpcTarget.All);
        IsFiring = false;
    }

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
