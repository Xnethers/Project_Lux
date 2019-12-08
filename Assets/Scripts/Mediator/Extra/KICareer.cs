using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Enid
public class KICareer : KeyboardInput
{
    [Space(10)]
    [Header("===== Basic Output signals =====")]
    public bool attackML;
    public bool auxiliaryMR;
    public bool attackF;
    public bool attackQ;
    public bool forcingML;
    public bool forceReleaseML;//forcetest
    [Space(10)]
    [Header("===== Others Career Output signals =====")]
    public bool aimingMR;
    public bool unAimMR;
    public bool R;

    void Start()
    {
        buttonML = new MyButton(0.8f);
    }
    new void Update()
    {
        if (isAI)
            return;
        if (!photonView.IsMine)
            return;
        if(!inputActive)
            return;
        base.Update();

        if (inputEnabled == false)
        {
            attackML = false;
            forcingML = false;
            forceReleaseML = false;
            auxiliaryMR=false;
            aimingMR = false;
            unAimMR = false;
            attackF = false;
            attackQ = false;
            R = false;
        }
        //基本操作
        attackML = buttonML.OnReleased;
        forcingML = buttonML.IsPressing && !buttonML.IsDelaying;
        forceReleaseML = buttonML.OnReleased && !buttonML.IsDelaying;
        auxiliaryMR = buttonMR.OnPressed;
        
        attackF = buttonF.OnPressed;
        attackQ = buttonQ.OnPressed;
        //補彈
        R = buttonR.OnPressed;
        //狙擊瞄準
        aimingMR = buttonMR.IsPressing;
        unAimMR = buttonMR.OnReleased;
    }
}
