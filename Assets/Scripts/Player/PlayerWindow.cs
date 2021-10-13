using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWindow : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public TextMeshPro textMeshPro;
    public int player;

    private void Start()
    {
        //после составления сохранения о выборе иконки игрока и имени, сделать выгрузку данных игрока
    }

    private void Update()
    {
        if (GameManager.game.players[player] != null)
        {
            textMeshPro.text = GameManager.game.players[player].Money + "";
        }
    }
}
