using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer {
    public enum STATE
    {
        IDLE,
        RUN,
        //Pause,
        FINISHED
    }
    public STATE state;

    private float duration = 1.0f;

    public float elapsedTime = 0;

    private bool pause = false;

    //public delegate void OnTimerSignature(string value);
    //public OnTimerSignature OnTimer;

    public void Tick() {
        if (state == STATE.IDLE){
        }
        else if (state == STATE.RUN){
            //if (pause == false) {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > duration){
                    state = STATE.FINISHED;
                //}
            } 
        }
        //else if (state == STATE.Pause){
        //}
        else if (state == STATE.FINISHED){
            //elapsedTime -= duration;
            //OnTimer.Invoke("hello!!");
        }
        else {
            Debug.Log("MyTimer error");
        }
    }
    public void Go(float _duration) {
        duration = _duration;
        elapsedTime = 0;
        //pause = false;
        state = STATE.RUN;
    }
    /*public void StopTimer() {
        duration = 0;
        elapsedTime = 0;
        pause = true;
        state = STATE.FINISHED;
    }
    public void PauseTimer() {
        pause = true;
        state = STATE.Pause;
    }
    public void ResumeTimer() {
        pause = false;
        state = STATE.RUN;
    }*/
}
