using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditBureau : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        if (GameManager.game.Stage == Stages.GreenBuildings)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i] == parameter)
                    if (!repair)
                    {
                        return nameCard;
                    }
                    else
                    {
                        repair = false;
                    }
            }
        }
        else if (GameManager.game.Stage == Stages.GreenCardsProperties || GameManager.game.Stage == Stages.ChoosingOfDice)
        {
            int receivedmoney = Mathf.Min(players[player].money, receivedMoney);
            players[player].money -= receivedmoney;
        }

        return "";
    }
}
