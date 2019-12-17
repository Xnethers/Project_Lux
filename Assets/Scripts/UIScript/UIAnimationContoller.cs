using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace UITween
{
    public class UIAnimationContoller : MonoBehaviour
    {
        public int level = 0;

        private bool mouseStay;
        public RectTransform ThisButton;
        public Image ButtonImage;
        public Sprite SelectSprite;
        private Sprite originSprite;
        public Color color;
        private Color originColor;
        public float duration;
        private Vector3 originScale;
        public Vector3 endScale;

        // Use this for initialization
        void Start()
        {
            originScale = ThisButton.localScale;
            originColor = ButtonImage.color;
            originSprite = ButtonImage.sprite;
        }

        void Update()
        {
            if (Global.Level != level)
            {ButtonImage.sprite = originSprite; }
            else
            {
                ButtonImage.color = color;
                ButtonImage.sprite = SelectSprite;
            }
        }

        public void Enter()
        { ButtonImage.DOColor(color, duration); }
        public void Exit()
        {
            ButtonImage.DOColor(originColor, duration);
        }

        public void Click()
        {
            if (Global.Level != level)
            { ButtonImage.sprite = SelectSprite; }
        }

        public void ColorChange()
        {
            ButtonImage.DOColor(color, duration);
        }
        public void ColorChangeBack(int level)
        {
            if (Global.Level != level)
            { ButtonImage.DOColor(originColor, duration); }
        }
        public void SelectScale()
        { ThisButton.localScale = endScale; }
        public void ImageChange(int level)
        {
            if (Global.Level == level)
            { ButtonImage.sprite = SelectSprite; }
            else
            { ButtonImage.sprite = originSprite; }
        }
    }
}
