using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adela

    public class KIDoubleGun : KeyboardInput
    {
        [Space(10)]
        [Header("===== Career Output signals =====")]
        public bool attackML;
        public bool attackMR;
        public bool forcingML;
        public bool forceReleaseML;//forcetest
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
                attackF = false;
                attackQ = false;
                R = false;
            }

            attackML = buttonML.OnReleased;
            forcingML = buttonML.IsPressing && !buttonML.IsDelaying;
            forceReleaseML = buttonML.OnReleased && !buttonML.IsDelaying;
            attackF = buttonF.OnPressed;
            attackQ = buttonQ.OnPressed;
            R = buttonR.OnPressed;
        }
    }

