using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace UITween
{
    public class UISelectionPanelContoller : UIManager
    {
        [Header("Selection Panel Button")]
        public Button CreateRoomButton;
        public Button JoinRandomRoomButton;
        public Button RoomListButton;
        public Button NoviceTeachingButton;
        private RectTransform CBrect;
        private RectTransform JBrect;
        private RectTransform RBrect;
        private RectTransform NTrect;
        private Vector2 CBrectoriginPos;
        private Vector2 JBrectoriginPos;
        private Vector2 RBrectoriginPos;
        private Vector2 NTrectoriginPos;
        public float MoveDistance = 400;
        public float duration;

        [Space(10)]
        [Header("Right Panel")]
        public RectTransform CreatRoomPanel;
        public RectTransform RoomListPanel;
        public RectTransform NoviceTeachingPanel;
        public RectTransform InsideRoomPanel;
        public Vector2 StretchSize = new Vector2(1182, 900);
        public float StretchDuration;
        [Space(10)]
        private bool ButtonisLeft = false;
        public UnityEvent onEnable;
        public UnityEvent onDisable;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {
            CBrect = CreateRoomButton.GetComponent<RectTransform>();
            JBrect = JoinRandomRoomButton.GetComponent<RectTransform>();
            RBrect = RoomListButton.GetComponent<RectTransform>();
            NTrect = NoviceTeachingButton.GetComponent<RectTransform>();

            CBrectoriginPos = CBrect.anchoredPosition;
            JBrectoriginPos = JBrect.anchoredPosition;
            RBrectoriginPos = RBrect.anchoredPosition;
            NTrectoriginPos = NTrect.anchoredPosition;
            CreatRoomPanel.sizeDelta = RoomListPanel.sizeDelta = NoviceTeachingPanel.sizeDelta = Vector2.one;
        }

        // Update is called once per frame
        void Update()
        {
            if (CBrect.anchoredPosition == CBrectoriginPos)
            { ButtonisLeft = false; }
            else if (CBrect.anchoredPosition == (CBrect.anchoredPosition - new Vector2(MoveDistance, 0)))
            { ButtonisLeft = true; }
        }

        void OnEnable()
        {
            onEnable.Invoke();
        }

        void OnDisable()
        {
            onDisable.Invoke();
            CBrect.anchoredPosition = CBrectoriginPos;
            JBrect.anchoredPosition = JBrectoriginPos;
            RBrect.anchoredPosition = RBrectoriginPos;
            NTrect.anchoredPosition = NTrectoriginPos;
            ResetPanelSize(); 
        }

        public void ButtonClickedToMove()
        {
            Sequence mySequence = DOTween.Sequence();
            if (!ButtonisLeft)
            {
                Tweener s1 = CBrect.DOAnchorPos(CBrectoriginPos - new Vector2(MoveDistance, 0), duration);
                Tweener s2 = JBrect.DOAnchorPos(JBrectoriginPos - new Vector2(MoveDistance, 0), duration);
                Tweener s3 = RBrect.DOAnchorPos(RBrectoriginPos - new Vector2(MoveDistance, 0), duration);
                Tweener s4 = NTrect.DOAnchorPos(NTrectoriginPos - new Vector2(MoveDistance, 0), duration);
                mySequence.Insert(0f, s1).Insert(0.1f, s2).Insert(0.2f, s3).Insert(0.3f, s4);
                ButtonisLeft = true;
            }
        }

        public void ButtonMoveBack()
        {
            Sequence mySequence = DOTween.Sequence();
            if (ButtonisLeft)
            {
                Tweener s1 = CBrect.DOAnchorPos(CBrectoriginPos, duration);
                Tweener s2 = JBrect.DOAnchorPos(JBrectoriginPos, duration);
                Tweener s3 = RBrect.DOAnchorPos(RBrectoriginPos, duration);
                Tweener s4 = NTrect.DOAnchorPos(NTrectoriginPos, duration);
                mySequence.Insert(0f, s4).Insert(0.1f, s3).Insert(0.2f, s2).Insert(0.3f, s1);
                ButtonisLeft = false;
            }
        }

        public void ResetPanelSize()
        {
            CreatRoomPanel.sizeDelta = Vector2.one;
            RoomListPanel.sizeDelta = Vector2.one;
            NoviceTeachingPanel.sizeDelta = Vector2.one;
        }

        public void OpenPanel(RectTransform panel)
        {
            if (panel.sizeDelta != StretchSize)
            {
                ResetPanelSize();
                Sequence mySequence = DOTween.Sequence();
                Tweener scale1 = panel.DOSizeDelta(new Vector2(3, StretchSize.y), StretchDuration).Pause();
                Tweener scale2 = panel.DOSizeDelta(StretchSize, StretchDuration).Pause();
                mySequence.Append(scale1.Play()).Append(scale2.Play());
            }
        }

        public void ClosePanel(RectTransform panel)
        {
            if (panel.sizeDelta != Vector2.one)
            {
                Sequence mySequence = DOTween.Sequence();
                Tweener scale1 = panel.DOSizeDelta(new Vector2(3, StretchSize.y), StretchDuration).Pause();
                Tweener scale2 = panel.DOSizeDelta(Vector2.one, StretchDuration).Pause();
                mySequence.Append(scale1.Play()).Append(scale2.Play());
            }
        }
    }
}
