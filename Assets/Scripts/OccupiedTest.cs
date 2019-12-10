using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;

public class OccupiedTest : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView PV;
    [Range(0, 100)] public float redValue;
    [Range(0, 100)] public float blueValue;
    public Text textRed;
    public Text textBlue;
    public bool isBlueIn;
    public bool isRedIn;

    public GameObject redVFX;
    public GameObject blueVFX;


    [SerializeField] private bool isInvincible;
    [SerializeField] float invincibleTime = 10;
    private Coroutine coroutine;
    [SerializeField] Material[] i;
    public enum OccupiedState
    {
        None,
        Red,
        Blue
    }
    public OccupiedState occupiedState;
    void Start()
    {
        PV = GetComponent<PhotonView>();

    }
    void Update()
    {
        int redpercent = (int)redValue;
        int bluepercent = (int)blueValue;

        textRed.text = "Red :" + redpercent.ToString();
        textBlue.text = "Blue :" + bluepercent.ToString();

        if (redpercent >= 100)
        { 
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.Occupation);
            GetComponent<PhotonView>().RPC("changtag", RpcTarget.AllViaServer, "Red"); 
        }
        else if (bluepercent >= 100)
        { 
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.Occupation);
            GetComponent<PhotonView>().RPC("changtag", RpcTarget.AllViaServer, "Blue"); 
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<PhotonView>().RPC("SetPercent", RpcTarget.AllViaServer);
        }


    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            return;
        if (other.tag == "Red")
        { isRedIn = true; }
        if (other.tag == "Blue")
        { isBlueIn = true; }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            return;
        if (photonView.IsSceneView && !isInvincible && !GameManager.Instance.isResult)
        {
            if (isRedIn && isBlueIn)
            { }
            else if (isRedIn && !isBlueIn)
            {
                
                GetComponent<PhotonView>().RPC("redadd", RpcTarget.AllViaServer);
            }
            else if (!isRedIn && isBlueIn)
            {
                GetComponent<PhotonView>().RPC("blueadd", RpcTarget.AllViaServer);
            }
            else
            { }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            return;
        if (other.tag == "Red")
        { isRedIn = false; }
        if (other.tag == "Blue")
        { isBlueIn = false; }
    }


    [PunRPC]
    void redadd()
    { redValue += Time.deltaTime; }

    [PunRPC]
    void blueadd()
    { blueValue += Time.deltaTime; }

    [PunRPC]
    void changtag(string tag)
    {
        if (this.tag == "Untagged")
        {
            if (tag == "Red")
            {
                occupiedState = OccupiedTest.OccupiedState.Red;
                Instantiate(redVFX, transform);
            }
            if (tag == "Blue")
            {
                occupiedState = OccupiedTest.OccupiedState.Blue;
                Instantiate(blueVFX, transform);
            }
        }
        this.tag = tag;
        this.redValue = 0;
        this.blueValue = 0;
        StartCoroutine("InvincibleTime");

        //var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath , "Materials"));

        //GetComponent<Renderer>().material = myLoadedAssetBundle.LoadAsset<Material>(tag);
    }

    /* 無法佔領時間 */
    IEnumerator InvincibleTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        occupiedState = OccupiedTest.OccupiedState.None;
        this.tag = "Untagged";
        isInvincible = false;
        yield return null;
    }

    [PunRPC]
    void SetPercent()//作弊鍵
    {
        redValue = 97;
        blueValue = 97;
    }



    #region PUN Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(redValue);
            stream.SendNext(blueValue);
        }
        else
        {
            this.redValue = (float)stream.ReceiveNext();
            this.blueValue = (float)stream.ReceiveNext();
        }
    }
    #endregion
}


