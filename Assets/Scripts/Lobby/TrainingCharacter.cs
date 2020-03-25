using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TrainingCharacter : InGameMenu
{
    public GameObject TrainingPanel;
    public GameObject PlayerUI;
    public override void Start()
    {
        base.Start();
        TrainingPanel.SetActive(false);
        originScale = AllCharacterUI[0].bt.GetComponent<RectTransform>().localScale;
    }
    public override void Update()
    {
        base.Update();

        if (isMenu)
            return;
        if (PlayerUI == null)
            PlayerUI = PlayerAm.GetComponent<PlayerUI>().ScreenCanvas;
        if (!GameManager.Instance.isResult)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {//Input.GetKeyDown(KeyCode.Tab) || 
                isTab = !isTab;
            }
            if (isTab)
            {
                PlayerDisable();
                PlayerUI.gameObject.SetActive(false);
                TrainingPanel.SetActive(true);
            }
            else
            {
                PlayerEnable();
                PlayerUI.gameObject.SetActive(true);
                TrainingPanel.SetActive(false);
            }
        }

        foreach (selectCharacter item in AllCharacterUI)
        {
            if (Global.selectCharacterID == item.ID)
            {
                item.bt.gameObject.GetComponent<Image>().sprite = item.choose;
                Click(item.bt.gameObject.GetComponent<RectTransform>());
            }
            else
            {
                item.bt.gameObject.GetComponent<Image>().sprite = item.nomal;
                UnSelect(item.bt.gameObject.GetComponent<RectTransform>());
            }
        }
    }
    public void OnClickCharacter(int whichCharacter)
    {
        Global.selectCharacterID = PlayerInfo.PI.mySelectedCharacter = whichCharacter;
    }
    public void OnClickChoose()
    {
        GameManager.Instance.DestroyCharacter(PlayerAm.gameObject);
        GameManager.Instance.CreateCharacter();
        isTab = !isTab;
    }

    [Space(10)]
    public selectCharacter[] AllCharacterUI;

    #region private 
    private Vector2 originScale;
    #endregion

    // Update is called once per frame
    public void Click(RectTransform rect)
    { Tweener c = rect.DOScale(originScale * 1.2f, 0.1f); }

    public void UnSelect(RectTransform rect)
    { rect.DOScale(originScale, 0.1f); }
}
