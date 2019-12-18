using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Fungus;

public class InGameMenu : MonoBehaviour
{
    public ActorManager PlayerAm;
    public GameObject EscPanel;
    public GameObject ResultPanel;
    public RectTransform PlayersListPanel;
    protected CanvasGroup PlayersListCanvasGroup;
	public float duration = 0.8f;
	private Vector2 size;
    public bool isMenu;
    public bool isTab;

    [Header("Win/Lose Image")]
	public Image WinorLoseImg;
	public Sprite WinSprite;
	public Sprite LoseSprite;
    private Vector2 ExpandSize;
    public virtual void Start()
    {
        size = PlayersListPanel.sizeDelta;
		SettlementPanelDisable();
        ExpandSize = WinorLoseImg.GetComponent<RectTransform>().sizeDelta;
        WinorLoseImg.rectTransform.sizeDelta = Vector2.one;
		EscPanel.SetActive(false);
        ResultPanel.SetActive(false); 
        PlayersListCanvasGroup = PlayersListPanel.GetComponentInChildren<CanvasGroup>();
    }
    public virtual void Update()
    {
        if (isMenu)
        {
            if (!GameManager.Instance.isResult)
                EscPanel.SetActive(true);
            else
            {
                ResultPanel.SetActive(true);
                if (Input.anyKeyDown && PlayersListPanel.sizeDelta == Vector2.one)
                {
                    Sequence mySequence = DOTween.Sequence();
                    Tweener scale1 = PlayersListPanel.DOSizeDelta(new Vector2(size.x, 1), duration);
                    Tweener scale2 = PlayersListPanel.DOSizeDelta(size, duration);
                    mySequence.Append(scale1).Append(scale2);
                }
                if (Input.anyKeyDown &&  PlayersListPanel.sizeDelta == size){//mySequence.IsComplete()
                    // SoundManager.Instance.FadeOutBGM();
                    SoundManager.Instance.PlaySceneBGM(SoundManager.Instance.BGM);
                    if(Global.Level == -1){
                        if(!FindObjectOfType<Flowchart>().GetBooleanVariable("isTalking")){
                            GameManager.Instance.LeaveRoom();
                        }
                    }
                    else 
                        GameManager.Instance.LeaveRoom();
                }
            }
            PlayerDisable();
        }
        else
        {
            PlayerEnable();
        }
        if(isTab){
            return;
        }
        if (!GameManager.Instance.isResult)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isMenu = !isMenu;
                // if (PlayerAm != null)
                //     PlayerAm.sm.RPC_Lock();
            }
        }
    }
    public void PlayerDisable(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (PlayerAm != null)
        {
            // PlayerAm.ac.pi.enabled=false;
            // PlayerAm.ac.camcon.enabled = false;
            PlayerAm.ac.pi.inputActive = false;
            PlayerAm.ac.pi.InputInitialize();
        }
    }
    public void PlayerEnable(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EscPanel.SetActive(false);
        ResultPanel.SetActive(false);
        if (PlayerAm != null)
        {
            PlayerAm.ac.pi.inputActive = true;
            // PlayerAm.ac.pi.enabled=true;
            // PlayerAm.ac.camcon.enabled = true;
        }
    }
    public void SettlementPanelDisable(){
		PlayersListPanel.sizeDelta=Vector2.one;
    }
    public void SettlementPanelEnable(){
		PlayersListPanel.sizeDelta=size;
    }
    public void ShowWinOrLose(bool iswinteam)
    {
        Sequence mySequence = DOTween.Sequence();

        if (iswinteam)
        {
            WinorLoseImg.sprite = WinSprite;
            Tweener scale1 = WinorLoseImg.rectTransform.DOSizeDelta(ExpandSize * 2, duration * 0.5F);
            Tweener scale2 = WinorLoseImg.rectTransform.DOSizeDelta(ExpandSize, duration);
            mySequence.Append(scale1).Append(scale2);
        }
        else
        {
            WinorLoseImg.sprite = LoseSprite;
            Tweener scale1 = WinorLoseImg.rectTransform.DOSizeDelta(ExpandSize * 2, duration * 0.5F);
            Tweener scale2 = WinorLoseImg.rectTransform.DOSizeDelta(ExpandSize, duration);
            mySequence.Append(scale1).Append(scale2);
        }
    }

    public void CloseEsc()
    { isMenu = false; }
}
