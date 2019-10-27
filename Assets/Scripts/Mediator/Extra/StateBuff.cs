using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using EZCameraShake;

public class StateBuff : MonoBehaviourPunCallbacks
{
    private StateManager sm;

    [Header("DeBuff")]
    public Buff[] Buffs;
    public bool isBlind;
    public bool isRepel;
    public bool isMark;
    public bool isShake;
    public bool isSpeedup;
    public GameObject thunderflash;
    public GameObject Markeffect;
    public OutlineObject[] OutlineScript;

    MyTimer Timer_mark = new MyTimer();

    public float MarkTime = 5;

    public List<Player> OtherTeam = new List<Player>();
    [Header("=== Camera Shake Setting ===")]
    public CameraShake cameraShake;
    // public CameraShaker playerCamShaker;
    // public float magnitude = 4f;
    // public float roughness = 5f;
    // public float fadeIn = 0.1f;
    // public float fadeOut = 1.5f;


    // Use this for initialization
    void Start()
    {
        sm = GetComponent<StateManager>();
        if (OutlineScript != null)
        {
            foreach (var item in OutlineScript)
            { item.enabled = false; }
        }
        //StartCoroutine(Mark());
        PunTeams.Team team = PhotonNetwork.LocalPlayer.GetOtherTeam();
        OtherTeam = PunTeams.PlayersPerTeam[team];

        if (this.tag == "Untagged")
        {
            PunTeams.Team redteam = PunTeams.Team.red;
            PunTeams.Team buleteam = PunTeams.Team.blue;
            OtherTeam = PunTeams.PlayersPerTeam[redteam];
            OtherTeam.AddRange(PunTeams.PlayersPerTeam[buleteam]);
        }
        // playerCamShaker = sm.am.ac.camcon.GetComponentInParent<CameraShaker>();
        // if (photonView.IsMine && !sm.am.ac.pi.isAI)
        // {
        //     playerCamShaker = sm.am.ac.camcon.transform.parent.gameObject.AddComponent<CameraShaker>();
        // }
        cameraShake = sm.am.ac.camcon.GetComponentInParent<CameraShake>();
    }

    private new void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Timer_mark.Tick();
        if (sm.isDie)
        {
            SetAllDeBuff(new DamageBuff(false, false, false, false));
        }
        if (isRepel)
        {
            //photonView.RPC("RPC_SetTrigger", RpcTarget.All, "hit");
            if (!sm.isDie)
                sm.am.ac.RPC_SetTrigger("hit");
            isRepel = false;
        }
        if (!photonView.IsMine)
        { return; }

        if (isBlind)
        {
            GameObject flash = (GameObject)Instantiate(thunderflash, Camera.main.transform.GetChild(0));
            flash.transform.position = flash.transform.parent.position;
            isBlind = false;
        }
        if (isMark)
        {
            foreach (var item in OtherTeam)
            { photonView.RPC("openOutline", item); }
            Instantiate(Markeffect, transform);
            Invoke("Invoke_closeOutline", MarkTime);
            //StartCoroutine(Mark());
            isMark = false;
        }
        if (isShake)
        {
            // playerCamShaker.ShakeOnce(magnitude,roughness,fadeIn,fadeOut);
            // Debug.Log(gameObject.name+":Y");
            StartCoroutine(cameraShake.Shake(cameraShake.duration, cameraShake.magnitude));
            isShake = false;
        }
        if (isSpeedup)
        {
            sm.am.ac.SetSpeedup(2.0f);
            Invoke("notSpeedup", 3.0f);
        }
        else
        {
            sm.am.ac.SetSpeedup(1.0f);
        }
    }

    void notSpeedup() { isSpeedup = false; }


    public void SetAllDeBuff(DamageBuff buff)
    {
        this.isBlind = buff.isBlind;
        this.isRepel = buff.isRepel;
        this.isMark = buff.isMark;
        this.isShake = buff.isShake;
    }
    void SetDeBuff()
    {

    }

    #region PUN Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(isBlind);
            stream.SendNext(isRepel);
            stream.SendNext(isMark);
        }
        else
        {
            // Network player, receive data
            this.isBlind = (bool)stream.ReceiveNext();
            this.isRepel = (bool)stream.ReceiveNext();
            this.isMark = (bool)stream.ReceiveNext();
        }
    }
    #endregion

    IEnumerator Mark()
    {
        while (true)
        {
            if (Timer_mark.state == MyTimer.STATE.RUN)
            {
                yield return null;
            }
            else if (Timer_mark.state == MyTimer.STATE.FINISHED)
            {
                foreach (var item in OtherTeam)
                { photonView.RPC("closeOutline", item); }
                yield return null;
            }
            else
            {
                yield return null;
            }
        }
    }

    [PunRPC]
    void openOutline()
    {
        Timer_mark.Go(MarkTime);
        foreach (var item in OutlineScript)
        { item.enabled = true; }
    }

    [PunRPC]
    void closeOutline()
    {
        foreach (var item in OutlineScript)
        { item.enabled = false; }
        Timer_mark.state = MyTimer.STATE.IDLE;
    }

    void Invoke_closeOutline()
    {
        foreach (var item in OtherTeam)
        { photonView.RPC("closeOutline", item); }
    }

    [PunRPC]
    void PS_creatMarkeffect()
    { Instantiate(Markeffect, transform); }

    public void SeteBuff(Buff[] message)
    {
        foreach (var item in message)
        {
            //     if (item.isopen)
            //     { Buffs.GetHashCode();}
        }
    }
}

[System.Serializable]
public class Buff
{
    public string BuffName;
    public bool isopen;
    public Buff(string BuffName, bool isopen)
    {
        this.BuffName = BuffName;
        this.isopen = isopen;
    }

    public void GetBuffName()
    { }


}