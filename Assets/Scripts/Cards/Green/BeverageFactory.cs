using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeverageFactory : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i] == parameter)
                if (!repair)
                {
                    int cafe = 0;
                    for(int p=0;p< players.Length; p++)
                    {
                        cafe += players[p].CheckTypeBuildings(TypeBuildings.cafe, (int)TypeCards.red);
                    }
                    players[player].money += receivedMoney * cafe;
                }
                else
                {
                    repair = false;
                }
        }

        return "";
    }
}
