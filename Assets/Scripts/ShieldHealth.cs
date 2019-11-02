using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
public class ShieldHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] public float health = 100;
    [SerializeField] private float maxhealth = 100;
    private PhotonView PV;
    private FieldOfView fov;
    private Collider col;
    private ActorManager am;

    public float Maxhealth
    {
        get { return maxhealth; }
    }
    //public float LifeTime = 5f;
    // Use this for initialization
    void Start()
    {
        PV = GetComponent<PhotonView>();
        col = GetComponent<Collider>();
        am = GetComponentInParent<ActorManager>();
        col.gameObject.SetActive(false);
        health = maxhealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (health <= 0)
            {
                // Show Break VFX
                gameObject.SetActive(false);
            }
            else
            {

            }
        }
        else
        {
            col.gameObject.SetActive(false);
            return;
        }
    }

    void OnTriggerEnter(Collider col)
    {

    }
    public void TryDoDamage(float damage)
    {
        if (health < 0)
        { return; }
        if (this.gameObject.activeSelf)
        { AddHP(-1 * damage); }
        // if (occupied.tag == "Untagged")//occupied.occupiedState == OccupiedTest.OccupiedState.None
        //     return;
        // if (occupied.tag != this.tag)
    }
    public void AddHP(float value)
    {
        health += value;
        health = Mathf.Clamp(health, 0, maxhealth);

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(health);
        }
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
        }
    }
}