using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

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
        if(sm.am.ac.pi.isAI){
            ScreenCanvas.SetActive(false);
            return;
        }
        if (sm != null && playerNameText != null)
        {
            playerNameText.text = photonView.Owner.NickName;
        }
        if(!photonView.IsMine)
        {
            ScreenCanvas.SetActive(false);
           return;
        }
        foreach(Image image in skill)  
            image.enabled=false;
        skill[1].enabled=true;//rush
        occupied =FindObjectOfType<OccupiedTest>();
        ReLiveTime.text = sm.deadTime.ToString();
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
            playerHealthSlider.value = sm.HP/sm.HPMax;
        }
        textRP.text ="RP:" + sm.RP.ToString();
        if(textTag !=null)
            textTag.text ="Team:" + transform.tag.ToString();
        if(sm.am.ac.pi.isAI)
            return;
        if(!photonView.IsMine)
            return;
        skill[0].enabled= !careercon.CheckCD(careercon.skillF);
        skill[2].enabled= !careercon.CheckCD(careercon.skillAir);
        skill[3].enabled= !careercon.CheckCD(careercon.skillForce);
        skill[1].fillAmount = sm.RP/sm.RPMax;
        forcingAim.enabled = sm.isForcingAim;
        forcingAim.fillAmount=careercon.forcingTimer.elapsedTime/careercon.careerValue.ForcingCD;
        OccupiedRed.fillAmount = occupied.redValue/100;
        OccupiedBlue.fillAmount = occupied.blueValue/100;
        if(sm.dieTimer.state == MyTimer.STATE.RUN){
            ReLiveTime.gameObject.SetActive(true);
            ReLiveTime.text = Mathf.Ceil(sm.deadTime-sm.dieTimer.elapsedTime).ToString();
        }
        else
            ReLiveTime.gameObject.SetActive(false);
    }

    #endregion
}
