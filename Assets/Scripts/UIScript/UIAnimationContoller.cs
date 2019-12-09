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
        public RectTransform ThisButton;
        public Image ButtonImage;
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
        }

        public void ButtonScaleEnter(RectTransform rect, Vector3 endScale, float duration)
        { ThisButton.DOScale(endScale, duration); }
        public void ButtonScaleExit()
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            if (entry.eventID == EventTriggerType.Select)
            { ThisButton.DOScale(originScale, duration); }
        }

        public void ColorChange()
        {
            ButtonImage.DOColor(color, duration);
        }
        public void ColorChangeBack()
        {
            ButtonImage.DOColor(originColor, duration);
        }
        public void SelectScale()
        { ThisButton.localScale = endScale; }
    }
}
