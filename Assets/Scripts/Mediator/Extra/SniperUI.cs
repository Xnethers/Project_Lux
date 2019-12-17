using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SniperUI : PlayerUI
{
    public SniperController sc;
    public RectTransform aimUI;
    public Text magazineText;
    //private Vector2 hide;

    // Use this for initialization
    new void Start()
    {
        base.Start();
        aimUI.gameObject.SetActive(false);
        //hide = new Vector2(0, 300);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (sc.target != null)
        {
            aimUI.gameObject.SetActive(true);
            Vector2 viewPos = Camera.main.WorldToScreenPoint(sc.target.transform.position);
            aimUI.position = viewPos;
        }
        else
        {
            aimUI.gameObject.SetActive(false);
            //aimUI.localPosition = hide;
        }
        magazineText.text = sc.magazine.ToString();
    }

}
