using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingBoat : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        if (players[player].CheckNameCard("Port", (int)TypeCards.yellow) > 0)//если у владельца карты есть порт
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
