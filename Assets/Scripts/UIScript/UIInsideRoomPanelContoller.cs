using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace UITween
{
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

        public Image Borislav;
        public Image Enid;
        public Image Lena;
        public Image Adela;

        private Image click_b;
        private Image click_l;
        private Image click_e;
        private Image click_a;

        public Vector2 origin;

        public float duration;
        public UnityEvent onEnable;
        public UnityEvent onDisable;


        void Start()
        {
            click_b = Borislav.transform.FindChild("Click").GetComponent<Image>();
            click_e = Enid.transform.FindChild("Click").GetComponent<Image>();
            click_l = Lena.transform.FindChild("Click").GetComponent<Image>();
            click_a = Adela.transform.FindChild("Click").GetComponent<Image>();
            origin = click_b.rectTransform.sizeDelta;

            click_b.color = Color.clear;
            click_e.color = Color.clear;
            click_l.color = Color.clear;
            click_a.color = Color.clear;

            lobbyMainPanel.myAllCharacters.SetActive(false);
        }
        void Update()
        { }
        Tweener c;
        Vector2 b;
        public void Click(RectTransform rect)
        {
            b = rect.localScale ;
            c = rect.DOScale(b*1.2f, 0.1f);
        }

        public void UnSelect(RectTransform rect)
        {rect.DOScale(b, 0.1f); }
        public void Select()
        {
            if (PlayerInfo.PI.mySelectedCharacter == 0)
            {
                Vector2 b = click_b.rectTransform.sizeDelta * 5f;
                click_b.color = Color.white;
                Sequence mySequence = DOTween.Sequence();
                Tweener s1 = click_b.rectTransform.DOScale(0.5f, duration);
                Tweener s2 = click_b.DOFade(1, duration);
                Tweener s3 = click_b.DOFade(0, duration / 2);
                //Tweener showtext = loginText.DOText(logintextstring, StretchDuration).Pause();
                mySequence.Append(s1).Join(s2).Append(s3);

            }
            else if (PlayerInfo.PI.mySelectedCharacter == 1)
            { }
            else if (PlayerInfo.PI.mySelectedCharacter == 2)
            { }
            else if (PlayerInfo.PI.mySelectedCharacter == 3)
            { }
        }
        void OnEnable()
        {
            onEnable.Invoke();
        }

        void OnDisable()
        {
            onDisable.Invoke();
        }

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