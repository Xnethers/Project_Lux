using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*鍵盤輸入 */


public class KeyboardInput : IUserInput
{


    [Header("===== Key settings =====")]
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyRun = "left shift";
    public string keyJump = "space";
    public string keyML = "mouse 0";
    public string keyMR = "mouse 1";
    public string keyF = "f";
    public string keyQ = "q";
    public string keyR = "r";
    public string keyE = "e";
    public string keyESC = "escape";
    public string keyTAB = "tab";
    public MyButton buttonUp = new MyButton();
    public MyButton buttonDown = new MyButton();
    public MyButton buttonLeft = new MyButton();
    public MyButton buttonRight = new MyButton();
    public MyButton buttonRun = new MyButton();
    public MyButton buttonJump = new MyButton();
    public MyButton buttonML = new MyButton();
    public MyButton buttonMR = new MyButton();
    public MyButton buttonF = new MyButton();
    public MyButton buttonQ = new MyButton();
    public MyButton buttonR = new MyButton();
    public MyButton buttonE = new MyButton();
    public MyButton buttonESC = new MyButton();
    public MyButton buttonTAB = new MyButton();

    [Header("===== Mouse settings =====")]
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    public void Update()
    {
        if (isAI)
            return;
        buttonUp.Tick(Input.GetKey(keyUp));
        buttonDown.Tick(Input.GetKey(keyDown));
        buttonRight.Tick(Input.GetKey(keyRight));
        buttonLeft.Tick(Input.GetKey(keyLeft));

        buttonRun.Tick(Input.GetKey(keyRun));
        buttonJump.Tick(Input.GetKey(keyJump));
        buttonML.Tick(Input.GetKey(keyML));
        buttonMR.Tick(Input.GetKey(keyMR));
        buttonF.Tick(Input.GetKey(keyF));
        buttonQ.Tick(Input.GetKey(keyQ));
        buttonR.Tick(Input.GetKey(keyR));
        buttonE.Tick(Input.GetKey(keyE));
        //menu
        // buttonESC.Tick(Input.GetKey(keyESC));
        // buttonTAB.Tick(Input.GetKey(keyTAB));

        Jup = (Input.GetAxis("Mouse Y")) * 3.0f * mouseSensitivityY;
        Jright = (Input.GetAxis("Mouse X")) * 2.5f * mouseSensitivityX;

        targetDup = (buttonUp.IsPressing ? 1.0f : 0) - (buttonDown.IsPressing ? 1.0f : 0);
        targetDright = (buttonRight.IsPressing ? 1.0f : 0) - (buttonLeft.IsPressing ? 1.0f : 0);
         
        isHorizontal = buttonLeft.IsPressing || buttonRight.IsPressing ;
        isVertical = buttonDown.IsPressing || buttonUp.IsPressing;
        isMove = isHorizontal || isVertical;

        if (inputEnabled == false)
        {
            targetDup = 0;
            targetDright = 0;
            run = false;
            jump = false;
            // menu = false;
        }
        if (inputMouseEnabled == false)
        {
            Jup = 0;
            Jright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref VelocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref VelocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt(Dup2 * Dup2 + Dright2 * Dright2);
        if(!isLatent)
            Dvec = Dright2 * transform.right + Dup2 * transform.forward;
        else{
            //潛光移動軸向為up,right
            Dvec = Dright2 * transform.right + Dup2 * transform.up;
        }
            

        run = buttonRun.IsPressing;

        jump = buttonJump.OnPressed;

        latent = buttonE.OnPressed;
        // menu = buttonTAB.OnPressed || buttonESC.OnPressed;
    }

}

