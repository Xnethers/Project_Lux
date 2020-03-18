using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace UITween
{
    [System.Serializable]
    public class selectCharacter
    {
        public Sprite nomal;
        public Sprite choose;
        public int ID;
        public Button bt;
    }
    public class UIInsideRoomPanelContoller : UIManager
    {
        [HideInInspector]
        public static UIInsideRoomPanelContoller insideRoomPanelUI;
        [Header("Inside Room Panel")]
        public Image BackGround;
        public Sprite Level1;
        public Sprite Level2;

        [Header("Select Camp Button")]
        public RectTransform CampButton;
        public Button YellowCamp;
        public Button PurpleCamp;
        private Vector2 pos = new Vector2(0, 1080);

        [Header("Camp Group")]
        public RectTransform YellowCampGroup;
        public RectTransform PurpleCampGroup;
        public Button selectCampButton;
        private Vector2 originpos = new Vector2(0, -1080);

        [Header("My All Character")]
        public selectCharacter[] AllCharacterUI;
        public Image Borislav;
        public Image Enid;
        public Image Lena;
        public Image Adela;
        [Space(10)]
        public float duration;

        #region private 
        private Vector2 originScale;
        #endregion

        void Start()
        {
            lobbyMainPanel.myAllCharacters.SetActive(false);
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
            originScale = AllCharacterUI[0].bt.GetComponent<RectTransform>().localScale;
        }
        void Update()
        {
            // foreach (selectCharacter item in AllCharacterUI)
            // {
            //     if (Global.selectCharacterID == item.ID)
            //     {
            //         if (eventSystem.currentSelectedGameObject != item.bt.gameObject && eventSystem.alreadySelecting)
            //         { eventSystem.SetSelectedGameObject(item.bt.gameObject); }
            //         else if (!eventSystem.alreadySelecting)
            //         {
            //             item.bt.gameObject.GetComponent<Image>().sprite = item.choose;
            //             item.bt.gameObject.GetComponent<RectTransform>().localScale = originScale * 1.2f;
            //         }
            //     }
            //     else
            //     {

            //     }
            // }
        }



        EventSystem eventSystem;
        public void Click(RectTransform rect)
        {
            Tweener c = rect.DOScale(originScale * 1.2f, 0.1f);
        }

        public void UnSelect(RectTransform rect)
        { rect.DOScale(originScale, 0.1f); }

        public void ShowCampGroup(bool isYellow)
        {
            Sequence mySequence = DOTween.Sequence();
            Tweener s1 = CampButton.DOAnchorPos(pos, duration);
            Tweener s2;
            if (isYellow)
            { s2 = YellowCampGroup.DOAnchorPos(Vector2.zero, duration); }
            else
            { s2 = PurpleCampGroup.DOAnchorPos(Vector2.zero, duration); }
            Tweener s3 = YellowCamp.image.DOFade(0, duration / 2);
            Tweener s4 = PurpleCamp.image.DOFade(0, duration / 2);

            lobbyMainPanel.myAllCharacters.SetActive(true);
            mySequence.Append(s1).Join(s2).Join(s3).Join(s4);

            if (isYellow)
            { eventSystem.SetSelectedGameObject(Borislav.gameObject); }
            else
            { eventSystem.SetSelectedGameObject(Lena.gameObject); }
        }

        public void BackToSelectCamp(bool isYellow)
        {
            Sequence mySequence = DOTween.Sequence();
            Tweener s1 = CampButton.DOAnchorPos(Vector2.zero, duration);
            Tweener s2;
            if (isYellow)
            { s2 = YellowCampGroup.DOAnchorPos(originpos, duration); }
            else
            { s2 = PurpleCampGroup.DOAnchorPos(originpos, duration); }
            Tweener s3 = YellowCamp.image.DOFade(1, duration / 2);
            Tweener s4 = PurpleCamp.image.DOFade(1, duration / 2);

            lobbyMainPanel.myAllCharacters.SetActive(false);
            mySequence.Append(s1).Join(s2).Join(s3).Join(s4);
        }

        public void ChangeBackGround()
        {
            if (Global.Level == 1)
            { BackGround.sprite = Level1; }
            else if (Global.Level == 2)
            { BackGround.sprite = Level2; }
        }
    }
}