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

        public float duration;
        public UnityEvent onEnable;
        public UnityEvent onDisable;


        void Start()
        {
            lobbyMainPanel.myAllCharacters.SetActive(false);
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
    }
}