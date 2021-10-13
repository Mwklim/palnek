using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstantiateIcon : MonoBehaviour
{

    public GameObject[] PlayerIcons;

    GameManager game;

    float screenWidth = 10f;
    int maxPlayers = 5;
    void Start()
    {
        game = GameManager.game;
        maxPlayers = game.playerCount;
        PlayerIcons = new GameObject[maxPlayers];


        screenWidth = 11f * (Screen.width / 1080f) / (Screen.height / 1920f);       
       

        for (int i = 0; i < PlayerIcons.Length; i++)
        {
            PlayerIcons[i] = Instantiate(game.playerIcon);
            PlayerIcons[i].transform.parent = gameObject.transform;

            float x = (screenWidth / (maxPlayers - 1f)) * (i - (PlayerIcons.Length - 1) / 2f);
            PlayerIcons[i].transform.localPosition = new Vector3(x, 0, 0);
            PlayerIcons[i].GetComponent<PlayerWindow>().player = i;
        }
    }


}
