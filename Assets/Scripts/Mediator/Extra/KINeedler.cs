using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//JK
public class KINeedler : KeyboardInput
{
    [Space(10)]
    [Header("===== Career Output signals =====")]
    public bool attackML;
    public bool forcingML;
    public bool forceReleaseML;
    public bool attackMR;
    public bool attackF;
    public bool attackQ;

    void Start()
    {
        buttonML = new MyButton(.8f);
    }
    new void Update()
    {
        if (!photonView.IsMine)
            return;
        base.Update();
        if(inputEnabled == false){
            attackML=false;
            forcingML=false;
            forceReleaseML=false;
            attackMR=false;
            attackF=false;
            attackQ=false;
        }
        attackML = buttonML.OnReleased ;//&& buttonML.IsDelaying;
        forcingML = buttonML.IsPressing && !buttonML.IsDelaying;
        forceReleaseML = buttonML.OnReleased && !buttonML.IsDelaying;
        //&&  && buttonML.FinishDelay
        //|| (buttonML.OnReleased && buttonML.IsForce)
        //Debug.Log("attackML"+buttonML.delayTimer.elapsedTime);
        attackMR = buttonMR.OnPressed;
        attackF = buttonF.OnPressed;
        attackQ = buttonQ.OnPressed;
    }
}
