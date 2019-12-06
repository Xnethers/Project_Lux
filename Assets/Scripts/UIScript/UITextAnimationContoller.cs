using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace UITween
{
    public class UITextAnimationContoller : MonoBehaviour
    {
        public Text ThisText;
        public float duration;
        public Ease EaseType;
        [TextArea] public string Msg;
        [Space(10)]
        public UnityEvent onEnable;

        public UnityEvent onDisable;

        Tweener AlphaFade;
        Tweener ShowEachWord;

        void Start()
        {
            // AlphaFade = 
            // ShowEachWord = ThisText.DOText(Msg, duration);
        }
        void OnEnable()
        {
            onEnable.Invoke();
        }

        void OnDisable()
        {
            onDisable.Invoke();
        }

        public void TextAlphaFade()
        {
            ThisText.DOFade(0, duration).SetEase(this.EaseType).SetLoops(-1, LoopType.Yoyo);
        }

        public void TextShowEachWord()
        {
            ThisText.DOText(Msg, duration);
        }

        public void TextClear()
        { ThisText.text = ""; }
    }
}