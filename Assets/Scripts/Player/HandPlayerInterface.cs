using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandPlayerInterface : MonoBehaviour
{
    public SpriteRenderer background;
    HandPlayer handPlayer;
    GameObject playerIcons;
    Player player;

    float screenWidth = 13.5f;
    float maxCards = 5f;
    float distanceY = 1.2f;
    

    void Start()
    {
        handPlayer = GetComponent<HandPlayer>();
        player = GetComponent<Player>();

        screenWidth = 13.5f * (Screen.width / 1080f) / (Screen.height / 1920f);
        maxCards = Mathf.Max(Mathf.Round(5f * (Screen.width / 1080f) / (Screen.height / 1920f)), 5f);

        SetSizeBackground();

        InstantiatePlayerIcon();
        
    }


    void Update()
    {
        SetSizeBackground();
    }

    void SetSizeBackground()
    {
        int lvlCardsAll = 0;
        for (int i = 0; i < handPlayer.CardsTransforms.Count - 1; i++)
            lvlCardsAll += handPlayer.CardsTransforms[i].Count;

        lvlCardsAll = Mathf.CeilToInt(lvlCardsAll / maxCards);

        background.size = new Vector2(screenWidth, animParameter(background.size.y, (lvlCardsAll + 2.5f) * distanceY, 3f));
        background.gameObject.transform.localPosition = new Vector3(0, animParameter(background.gameObject.transform.localPosition.y, (6f + 0.5f * (12f - (lvlCardsAll + 2.5f) * distanceY)), 1.5f), 0);
    }

    void InstantiatePlayerIcon()
    {
        playerIcons = Instantiate(GameManager.game.playerIcon);
        playerIcons.transform.parent = gameObject.transform;


        float x = (screenWidth / (maxCards - 1f)) * ((Mathf.CeilToInt(maxCards / 5f) - 0.2f)/2f - (maxCards - 1) / 2f);
        playerIcons.transform.localPosition = new Vector3(x, 10.7f, 0);

        if (player != null)
            playerIcons.GetComponent<PlayerWindow>().player = player.playerNumber;
    }

    float animParameter(float nowPar, float setPar, float speed = 3f)
    {
        if (nowPar > setPar)
        {
            nowPar -= Mathf.Max(speed, 3 * Mathf.Abs(nowPar - setPar)) * Time.deltaTime;
            if (nowPar < setPar)
                nowPar = setPar;
        }

        if (nowPar < setPar)
        {
            nowPar += Mathf.Max(speed, 3 * Mathf.Abs(nowPar - setPar)) * Time.deltaTime;
            if (nowPar > setPar)
                nowPar = setPar;
        }

        return nowPar;
    }
}


// 24 - 0
// 12 - 6
//  8 - 6 + 0.5*(12-8) = 8
//  6 - 
