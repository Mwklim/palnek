using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i] == parameter)
                if (!repair)
                {
                    players[player].money += receivedMoney;
                }
                else
                {
                    repair = false;
                }
        }

        return "";
    }
}
