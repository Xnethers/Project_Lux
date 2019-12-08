using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/*處理實體輸入端狀態 */
public abstract class IUserInput : MonoBehaviourPunCallbacks {
    public bool isAI;
    public bool isLatent = false;
    [Header("===== Output signals =====")]
    public float Dup;
    public float Dright;
    public float Dmag;
    public Vector3 Dvec;

    public float Jup;
    public float Jright;

    //pressing signal
    public bool run;
    public bool isMove;
    public bool isHorizontal;
    public bool isVertical;

    //trigger once signal
    public bool jump;
    public bool roll;
    public bool latent;
    // public bool menu;
    //double trigger

    [Header("===== Others =====")]
    // public bool inputInitialize = false;
    public bool inputActive = true;
    public bool inputEnabled = true;
    public bool inputMouseEnabled = true;
    protected float targetDup;
    protected float targetDright;
    protected float VelocityDup;
    protected float VelocityDright;

    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }
    protected void UpdateDmagDvec(float Dup2, float Dright2) {
        Dmag = Mathf.Sqrt(Dup2 * Dup2 + Dright2 * Dright2);
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;
    }
    public void InputInitialize(){
        Dup = 0;
        Dright = 0;
        Dmag = 0;
        Dvec = Vector3.zero;
        Jup = 0;
        Jright = 0;
    }
}
