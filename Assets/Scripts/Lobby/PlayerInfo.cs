using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public const string PLAYER_Character = "PlayerCharacter";
    public const string PLAYER_READY = "IsPlayerReady";
    public static PlayerInfo PI;
    public int mySelectedCharacter;
    public int mySelectedCamp;
    private void OnEnable()
    {
        if (PlayerInfo.PI == null)
        {
            PlayerInfo.PI = this;
        }
        else
        {
            if (PlayerInfo.PI != this)
            {
                Destroy(PlayerInfo.PI.gameObject);
                PlayerInfo.PI = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);

    }
}
