using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InGameMenu : MonoBehaviour
{
    public ActorManager PlayerAm;
    public GameObject MenuPanel;
    public GameObject ResultPanel;
    public RectTransform PlayersListPanel;
	public float duration = 0.8f;
	private Vector2 size;
    public bool isMenu;
    void Start()
    {
		size = PlayersListPanel.sizeDelta;
		PlayersListPanel.sizeDelta=Vector2.one;
		MenuPanel.SetActive(false);
        ResultPanel.SetActive(false);
    }
    public virtual void Update()
    {
        if (!GameManager.Instance.isResult)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {//Input.GetKeyDown(KeyCode.Tab) || 
                isMenu = !isMenu;
            }
        }
        if (isMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (!GameManager.Instance.isResult)
                MenuPanel.SetActive(true);
            else
            {
                ResultPanel.SetActive(true);
                if (Input.anyKeyDown && PlayersListPanel.sizeDelta == Vector2.one)
                {
                    Sequence mySequence = DOTween.Sequence();
                    Tweener scale1 = PlayersListPanel.DOSizeDelta(new Vector2(size.x, 1), duration);
                    Tweener scale2 = PlayersListPanel.DOSizeDelta(size, duration);
                    mySequence.Append(scale1).Append(scale2);
                }
            }
            if (PlayerAm != null)
            {
                // PlayerAm.ac.pi.enabled=false;
                // PlayerAm.ac.camcon.enabled = false;
                PlayerAm.ac.pi.inputActive = false;
                PlayerAm.ac.pi.InputInitialize();
            }

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MenuPanel.SetActive(false);
            ResultPanel.SetActive(false);
            if (PlayerAm != null)
            {
                PlayerAm.ac.pi.inputActive = true;
                // PlayerAm.ac.pi.enabled=true;
                // PlayerAm.ac.camcon.enabled = true;
            }
        }

    }
}
