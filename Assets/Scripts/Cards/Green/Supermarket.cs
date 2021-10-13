using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supermarket : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i] == parameter)
                if (!repair)
                {
                    int receivedmoney = receivedMoney + ((players[player].CheckNameCard("Shopping center", (int)TypeCards.yellow) > 0) ? 1 : 0);
                    players[player].money += receivedmoney;
                }
                else
                {
                    repair = false;
                }
        }

        return "";
    }
}
