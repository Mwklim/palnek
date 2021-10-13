using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trawler : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        if (players[player].CheckNameCard("Port", (int)TypeCards.yellow) > 0)//если у владельца карты есть порт
            if (GameManager.game.Stage == Stages.BlueBuildings)
            {
                for (int i = 0; i < dice.Length; i++)
                {
                    if (dice[i] == parameter)
                        return nameCard;
                }
            }
            else if (GameManager.game.Stage == Stages.BlueCardsProperties)
            {
                if (!repair)
                {
                    players[player].money += parameter;
                }
                else
                {
                    repair = false;
                }
            }

        return "";
    }
}
