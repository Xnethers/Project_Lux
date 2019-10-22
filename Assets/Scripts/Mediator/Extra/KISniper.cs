using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Enid
public class KISniper : KeyboardInput
{
    [Space(10)]
    [Header("===== Career Output signals =====")]
    public bool attackML;
    public bool aimMR;
    public bool forcingML;
    public bool forceReleaseML;//forcetest
    public bool aimingMR;
    public bool unAimMR;
    public bool attackF;

    public bool attackQ;
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
        base.Update();

        if (inputEnabled == false)
        {
            attackML = false;
            forcingML = false;
            forceReleaseML = false;
            aimMR=false;
            aimingMR = false;
            unAimMR = false;
            attackF = false;
            attackQ = false;
            R = false;
        }

        attackML = buttonML.OnReleased;
        forcingML = buttonML.IsPressing && !buttonML.IsDelaying;
        forceReleaseML = buttonML.OnReleased && !buttonML.IsDelaying;
        aimMR = buttonMR.OnPressed;
        aimingMR = buttonMR.IsPressing;
        unAimMR = buttonMR.OnReleased;
        attackF = buttonF.OnPressed;
        attackQ = buttonQ.OnPressed;
        R = buttonR.OnPressed;
    }
}
