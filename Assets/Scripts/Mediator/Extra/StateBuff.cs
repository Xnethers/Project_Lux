using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;


public class StateBuff : MonoBehaviourPunCallbacks
{
    private StateManager sm;

    [Header("DeBuff")]
    public bool isBlind;
    public bool isRepel;
    public bool isMark;
    public GameObject thunderflash;
    public GameObject Markeffect;
    public OutlineObject[] OutlineScript;

    MyTimer Timer_mark = new MyTimer();

    public float MarkTime = 5;

    public List<Player> OtherTeam = new List<Player>();



    // Use this for initialization
    void Start()
    {
        sm = GetComponent<StateManager>();
        if (OutlineScript != null)
        {
            foreach (var item in OutlineScript)
            { item.enabled = false; }
        }
        StartCoroutine(Mark());
        PunTeams.Team team = PhotonNetwork.LocalPlayer.GetOtherTeam();
        OtherTeam = PunTeams.PlayersPerTeam[team];

        if (this.tag == "Untagged")
        {
            PunTeams.Team redteam = PunTeams.Team.red;
            PunTeams.Team buleteam = PunTeams.Team.blue;
            OtherTeam = PunTeams.PlayersPerTeam[redteam];
            OtherTeam.AddRange(PunTeams.PlayersPerTeam[buleteam]);
        }

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
            SetAllDeBuff(new DamageBuff(false, false, false));
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
            StartCoroutine(Mark());
            isMark = false;
        }

    }
    public void SetAllDeBuff(DamageBuff buff)
    {

        this.isBlind = buff.isBlind;
        this.isRepel = buff.isRepel;
        this.isMark = buff.isMark;
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

    [PunRPC]
    void PS_creatMarkeffect()
    {
        Instantiate(Markeffect, transform);

    }
}
