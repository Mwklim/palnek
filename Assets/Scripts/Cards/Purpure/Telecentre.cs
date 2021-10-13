using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telecentre : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        if (GameManager.game.Stage == Stages.PurpleBuildings)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i] == parameter)
                    return nameCard;
            }
        }
        else if (GameManager.game.Stage == Stages.PurpleCardsProperties)
        {          
                int moneyTake = Mathf.Min(players[parameter].money, receivedMoney);
                players[parameter].money -= moneyTake;
                players[player].money += moneyTake;
        }

        return "";
    }
}
