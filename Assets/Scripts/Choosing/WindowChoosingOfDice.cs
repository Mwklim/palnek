using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowChoosingOfDice : MonoBehaviour
{
    GameManager game;
    int player;

    void Start()
    {
        game = GameManager.game;
    }

 
    void Update()
    {
        //в стадии ChoosingOfDice загрузить из game лист listChoosingOfDice и выгрузить варианты на экран
    }
}
