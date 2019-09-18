using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Photon.Pun.Demo.Asteroids{


    public class MenuController : MonoBehaviourPunCallbacks//, IPunObservable
    {
        
        public GameObject[] tempUseUI;
        public GameObject[] tempCamps;
        public GameObject[] myCharacter;
        public GameObject[] myCharacterUI;
        public bool[] isChooseCharacter = new bool[6];
        
        public void ChangeCharacter()
        {
            object tempVisible = PhotonNetwork.LocalPlayer.CustomProperties[PlayerInfo.PLAYER_Character];
            if(tempVisible == null)
                return;
            ObjVisiable(myCharacter,(int)tempVisible);
            ObjSclae(myCharacterUI,(int)tempVisible);
            PlayerInfo.PI.mySelectedCharacter = (int)tempVisible;
        }
        public void OnClickCamp(int whichCamp)
        {
            ObjVisiable(tempCamps, whichCamp);
            PlayerInfo.PI.mySelectedCamp = whichCamp;
        }

        void ObjVisiable(GameObject[] tempObjs ,int visible){
            foreach(GameObject obj in tempObjs){
                obj.SetActive(false);
            }
            tempObjs[visible].SetActive(true);
        }
        void ObjSclae(GameObject[] tempObjs ,int visible){
            foreach(GameObject obj in tempObjs){
                obj.GetComponent<RectTransform>().localScale = new Vector3(0.5f,0.5f,0.5f);
            }
            tempObjs[visible].GetComponent<RectTransform>().localScale = new Vector3(0.6f,0.6f,0.6f);
        }
        public void isChoosedCharacter(bool[] tempBools){
            for (int i = 0; i < tempUseUI.Length; i++)
            {
                if(tempBools[i])
                    tempUseUI[i].SetActive(true);
                else if(!tempBools[i])
                    tempUseUI[i].SetActive(false);
            }
        }
    }
}