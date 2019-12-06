using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
public class TowerHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isResult = false;
    [SerializeField] public float health = 100;
    [SerializeField] private float maxhealth = 100;
    private PhotonView PV;
    private FieldOfView fov;

    public float TowerMaxhealth
    {
        get { return maxhealth; }
    }

    public GameObject resultUI;
    public Text tempText;
    public OccupiedTest occupied;
    public float enemyHpValue = -5f;
    // Use this for initialization
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (resultUI != null)
        { resultUI.SetActive(false); }
        health = maxhealth;
        fov = GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !isResult)
        {
            PV.RPC("SetResult", RpcTarget.All);
            foreach (ActorManager am in FindObjectsOfType<ActorManager>())
            {
                am.ac.SetBool("result", true);
                am.ac.pi.inputEnabled = false;
                am.ac.pi.inputMouseEnabled = false;
                //am.ac.pi.enabled = false;
                am.ac.camcon.isCursorVisible = true;
                am.ac.anim.ResetTrigger("attack");

            }
            isResult = true;
        }
        else
        {
            if (fov.useTargets.Count > 0)
            {
                foreach (ActorManager am in fov.useTargets)
                {
                    if (!am.sm.isHPing)
                        am.sm.StartAddHp(enemyHpValue);
                }
            }
        }

    }

    void OnTriggerEnter(Collider col)
    {
        // if (!photonView.IsSceneView)
        // {
        //     Debug.Log("!photonView.IsSceneView");
        //     return; 
        // }

        // if (other.gameObject.layer == LayerMask.NameToLayer("Projectile") && other.tag != this.tag)
        // { 
        //     health -= 100f * Time.deltaTime;
        //     Destroy(other.gameObject);
        //     Debug.Log("Projectile");
        // }
        // if (other.gameObject.layer == LayerMask.NameToLayer("Weapon") && other.tag != this.tag)
        // { 
        //     health -= 150f * Time.deltaTime; 
        //     Debug.Log("Weapon");
        // }
    }
    public void TryDoDamage(DamageData damageData)
    {
        if (health < 0)
            return;
        if (occupied.tag == "Untagged")//occupied.occupiedState == OccupiedTest.OccupiedState.None
            return;
        if (occupied.tag != this.tag)
            AddHP(-1 * damageData.Damage);
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
    [PunRPC]
    private void SetResult()
    {
        resultUI.SetActive(true);
        if (this.tag == "Red")
        {//red = Yellow , blue = Purple
            if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
            {
                tempText.text = "Yellow Team Lose";
                SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.Lose);
            }
            else if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
            {
                tempText.text = "Purple Team Win";
                SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.Win);
            }
        }
        else if (this.tag == "Blue")
        {
            if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
            {
                tempText.text = "Yellow Team Win";
                SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.Win);
            }
            else if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
            {
                tempText.text = "Purple Team Lose";
                SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.Lose);
            }
        }
    }


}