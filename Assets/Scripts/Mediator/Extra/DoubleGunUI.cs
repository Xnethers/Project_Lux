using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubleGunUI : PlayerUI
{
    public DoubleGunController dc;
    public RectTransform aimUI;
    public Text magazineText;
    //private Vector2 hide;

    // Use this for initialization
    new void Start()
    {
        base.Start();
        // aimUI.gameObject.SetActive(false);
        //hide = new Vector2(0, 300);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        magazineText.text = dc.magazine.ToString();
    }

}
