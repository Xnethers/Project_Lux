using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UITween;
using DG.Tweening;

namespace UITween
{
    public class UILoginPanelContoller : UIManager
    {
        [Header("Title Panel")]
        public Text startText;

        [Header("Login Panel")]
        public RectTransform LoginPanel;
        public Vector2 StretchSize;
        public float StretchDuration;
        public Text loginText;
        private string logintextstring;
        public InputField playerNameInput;
        public Button loginButton;
        public Button cancelButton;
        public UnityEvent onEnable;
        public UnityEvent onLoginPanelActive;
        public UnityEvent onDisable;

        // Use this for initialization
        void Start()
        {
            logintextstring = loginText.text;
            loginText.text = "";
        }
        void Update()
        {

        }

        void OnEnable()
        { onEnable.Invoke(); }

        void OnDisable()
        { onDisable.Invoke(); }

        public void StartText()//標題畫面文字閃爍
        {
            TextAlphaFade(startText, 1);
        }

        public void OpenLoginPanel()//登入介面展開
        {
            // PanelHorizontalStretch(LoginPanel, StretchSize, 1);
            LoginPanel.gameObject.SetActive(true);
            Sequence mySequence = DOTween.Sequence();
            Tweener scale1 = LoginPanel.DOSizeDelta(new Vector2(StretchSize.x, 1), StretchDuration).Pause();
            Tweener scale2 = LoginPanel.DOSizeDelta(StretchSize, StretchDuration).Pause();
            Tweener showtext = loginText.DOText(logintextstring, StretchDuration).Pause();
            mySequence.Append(scale1.Play()).Append(scale2.Play()).Append(showtext.Play());
        }

        public void RevokeLoginPanel()//取消登入
        {
            Sequence mySequence = DOTween.Sequence();
            Tweener scale1 = LoginPanel.DOSizeDelta(new Vector2(LoginPanel.sizeDelta.x, 1), StretchDuration).Pause();
            Tweener scale2 = LoginPanel.DOSizeDelta(Vector2.one, StretchDuration).Pause().OnComplete(() => { lobbyMainPanel.LoginPanel.SetActive(false); });
            Tweener cleartext = loginText.DOText("", StretchDuration);
            mySequence.Append(scale1.Play()).Append(scale2.Play()).Append(cleartext);
            // Tweener showtext = loginText.DOText(logintextstring, 1).Pause();
            // PanelHorizontalStretch(LoginPanel, new Vector2(1, 1), 1);
        }
    }
}
