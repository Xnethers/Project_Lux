using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Text;
using UITween;
using DG.Tweening;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    #region Private Fields
    public Text textTag;
    public Text textRP;

    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private Text playerNameText;


    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;
    private Image fill;
    [SerializeField]
    private RectTransform CurveBlood;
    public Vector3 bloodLatentRot = new Vector3(0f, 35f, 0f);
    public Vector3 bloodLatentVec = new Vector3(0f, 0.44f, -1.4f);

    // [SerializeField] private PlayerManager target;
    //new
    public ICareerController careercon;
    [SerializeField] private StateManager sm;
    public GameObject ScreenCanvas;
    public Image[] skill;
    public Image forcingAim;
    public Image OccupiedRed;
    public Image OccupiedBlue;
    public OccupiedTest occupied;
    public Text ReLiveTime;
    public Image[] Buffs;
    private RectTransform[] BuffsRectTrans = new RectTransform[3];
    public float duration = 0.6f;
    public Vector2 buffOutPos = new Vector2(-275, 0);

    #endregion


    #region MonoBehaviour Callbacks

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public virtual void Start()
    {
        // target=GetComponent<PlayerManager>();
        // if (target != null && playerNameText != null)
        // {
        //     playerNameText.text = photonView.Owner.NickName;
        // }
        if (sm.am.ac.pi.isAI)
        {
            ScreenCanvas.SetActive(false);
            return;
        }
        if (sm != null && playerNameText != null)
        {
            playerNameText.text = photonView.Owner.NickName;
        }
        if (!photonView.IsMine)
        {
            ScreenCanvas.SetActive(false);
            return;
        }

        //敵對血條紅色，同隊綠色
        fill = playerHealthSlider.transform.DeepFind("Fill").GetComponent<Image>();
        if (PhotonNetwork.LocalPlayer.GetTeam() == photonView.Owner.GetTeam())
        { fill.color = Color.green; }
        else
        { fill.color = Color.red; }
        textTag.text = "";

        occupied = FindObjectOfType<OccupiedTest>();
        ReLiveTime.text = sm.deadTime.ToString();
        for (int i = 0; i < Buffs.Length; i++)
        {
            BuffsRectTrans[i] = Buffs[i].GetComponent<RectTransform>();
            BuffsRectTrans[i].anchoredPosition = buffOutPos;
            Buffs[i].DOFade(0, duration);
            // Buffs[i].enabled = false; 
        }
        // skill[1].enabled = true;//rush
    }

    public virtual void Update()
    {
        // Reflect the Player Health
        // if (target != null && playerHealthSlider != null)
        // {
        //     playerHealthSlider.value = target.Health;
        // }
        if (sm != null && playerHealthSlider != null)
        {
            playerHealthSlider.value = sm.HP / sm.HPMax;
        }
        textRP.text = "RP:" + sm.RP.ToString();
        // if (textTag != null)
        // {//red = Yellow , blue = Purple
        //     if (transform.tag == "Red")
        //     { textTag.text = "Team:Yellow"; }
        //     else if (transform.tag == "Blue")
        //     { textTag.text = "Team:Purple"; }
        //     else
        //     { textTag.text = "Team:AI"; }
        //     //textTag.text ="Team:" + transform.tag.ToString();
        // }
        if (CurveBlood != null)
        {
            if (sm.am.ac.pi.isLatent)
            {
                CurveBlood.localPosition = bloodLatentVec;
                CurveBlood.localRotation = Quaternion.Euler(bloodLatentRot);
            }
            else
            {
                CurveBlood.localPosition = Vector3.zero;
                CurveBlood.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        if (sm.am.ac.pi.isAI)
            return;
        if (!photonView.IsMine)
            return;
        if (skill.Length > 0)
        {
            // skill[0].enabled = !careercon.CheckCD(careercon.skillF);
            // skill[2].enabled = !careercon.CheckCD(careercon.skillAir);
            // skill[3].enabled = !careercon.CheckCD(careercon.skillForce);
            skill[0].fillAmount = (careercon.careerValue.SecondCD - careercon.skillF.atkTimer.elapsedTime) / careercon.careerValue.SecondCD;
            skill[2].fillAmount = (careercon.careerValue.AirCD - careercon.skillAir.atkTimer.elapsedTime) / careercon.careerValue.AirCD;
            skill[3].fillAmount = (careercon.careerValue.ForceCD - careercon.skillForce.atkTimer.elapsedTime) / careercon.careerValue.ForceCD;
            skill[4].fillAmount = (careercon.careerValue.FirstCD - careercon.skillMR.atkTimer.elapsedTime) / careercon.careerValue.FirstCD;
            skill[1].fillAmount = sm.RP / sm.RPMax;
        }
        forcingAim.enabled = sm.isForcingAim;
        forcingAim.fillAmount = careercon.forcingTimer.elapsedTime / careercon.careerValue.ForcingCD;
        if (occupied != null)
        {
            OccupiedRed.fillAmount = occupied.redValue / 100;
            OccupiedBlue.fillAmount = occupied.blueValue / 100;
        }
        else
        {
            OccupiedRed.fillAmount = 0;
            OccupiedBlue.fillAmount = 0;
        }
        if (sm.dieTimer.state == MyTimer.STATE.RUN)
        {
            ReLiveTime.gameObject.SetActive(true);
            ReLiveTime.text = Mathf.Ceil(sm.deadTime - sm.dieTimer.elapsedTime).ToString();
        }
        else
            ReLiveTime.gameObject.SetActive(false);
        if (Buffs.Length > 0)
        {
            SetBuffsUI(sm.ATKBuff, Buffs[0], BuffsRectTrans[0]);
            SetBuffsUI(sm.DEFBuff, Buffs[1], BuffsRectTrans[1]);
            SetBuffsUI(sm.HOTBuff, Buffs[2], BuffsRectTrans[2]);
        }
    }
    void SetBuffsUI(float buffValue, Image buffImg, RectTransform buffRect)
    {
        if (buffValue != 1)
        {
            // buffObj.enabled = true;
            buffImg.DOFade(1, duration);
            buffRect.DOAnchorPos(Vector2.zero, duration);
        }
        else
        {
            // buffObj.enabled = false;
            buffImg.DOFade(0, duration);
            buffRect.DOAnchorPos(buffOutPos, duration);
        }
    }
    #endregion
}

