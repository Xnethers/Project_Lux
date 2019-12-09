using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using Photon.Pun.Demo.Asteroids;

//所有UI組件的功能集合
namespace UITween
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager uiManager;
        public LobbyMainPanel lobbyMainPanel;
        public Ease EaseType;

        void Start()
        {

        }

        #region Panel Tween Method

        public void PanelHorizontalStretch(RectTransform rect, Vector2 size, float duration)
        {
            Tweener scale1 = rect.DOSizeDelta(new Vector2(size.x, 1), duration).Pause();
            Tweener scale2 = rect.DOSizeDelta(size, duration).Pause();
            Sequence mySequence = DOTween.Sequence();

            mySequence.Append(scale1.Play()).Append(scale2.Play()); ;
        }

        public void PanelVerticalStretch(RectTransform rect, Vector2 size, float duration)
        {
            Tweener scale1 = rect.DOSizeDelta(new Vector2(1, size.y), duration).Pause();
            Tweener scale2 = rect.DOSizeDelta(size, duration).Pause();
            Sequence mySequence = DOTween.Sequence();

            mySequence.Append(scale1.Play()).Append(scale2.Play()); ;
        }

        // public void TextShowEachWord()
        // { Text.DOText(Msg, duration); }

        // public void TextClear()
        // { Text.text = ""; }

        #endregion

        #region Button Tween Method
        public void ButtonScaleEnter(RectTransform rect, Vector3 endScale, float duration)
        { rect.DOScale(endScale, duration); }
        public void ButtonScaleExit(RectTransform rect, Vector3 endScale, float duration)
        {
            rect.DOScale(endScale, duration);
        }

        public void ButtonColorChange(Button bt, Color color, float duration)
        {
            bt.image.DOColor(color, duration);
        }
        public void ButtonColorChangeBack(Button bt, Color color, float duration)
        {
            bt.image.DOColor(color, duration);
        }
        public void ButtonSelectScale(RectTransform rect, Vector3 endScale)
        { rect.localScale = endScale; }
        #endregion

        #region Text Tween Method
        public void TextAlphaFade(Text thistext, float duration)
        {
            thistext.DOFade(0, duration).SetEase(this.EaseType).SetLoops(-1, LoopType.Yoyo);
        }

        public void TextShowEachWord(Text thistext, string msg, float duration)
        {
            thistext.DOText(msg, duration);
        }

        public void TextClear(Text thistext)
        {
            thistext.text = "";
        }

        #endregion
    }
}