using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace UITween
{
    public class UISelectionPanelContoller : MonoBehaviour
    {
        public Button CreateRoomButton;
        public Button JoinRandomRoomButton;
        public Button RoomListButton;
        public Button NoviceTeachingButton;
        private RectTransform CBrect;
        private RectTransform JBrect;
        private RectTransform RBrect;
        private RectTransform NTrect;
        private Vector2 originPos;
        public float MoveDistance = 400;
        public float duration;


        public UnityEvent onEnable;

        public UnityEvent onDisable;

        // Use this for initialization
        void Start()
        {
            CBrect = CreateRoomButton.GetComponent<RectTransform>();
            JBrect = JoinRandomRoomButton.GetComponent<RectTransform>();
            RBrect = RoomListButton.GetComponent<RectTransform>();
            NTrect = NoviceTeachingButton.GetComponent<RectTransform>();
            originPos = CBrect.anchoredPosition;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            onEnable.Invoke();
        }

        void OnDisable()
        {
            onDisable.Invoke();
        }

        public void ButtonClickedToMove()
        {
            if (CBrect.anchoredPosition == originPos)
            {
                CBrect.DOAnchorPos(CBrect.anchoredPosition - new Vector2(MoveDistance, 0), duration);
                JBrect.DOAnchorPos(JBrect.anchoredPosition - new Vector2(MoveDistance, 0), duration);
                RBrect.DOAnchorPos(RBrect.anchoredPosition - new Vector2(MoveDistance, 0), duration);
                NTrect.DOAnchorPos(NTrect.anchoredPosition - new Vector2(MoveDistance, 0), duration);
            }
        }

		public void OpenPanel()
		{
			
		}
    }
}
