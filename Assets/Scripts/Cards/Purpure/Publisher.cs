using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Publisher : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;
        int playerCount = GameManager.game.playerCount;

        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i] == parameter)
                for (int p = player - 1; true; p--)
                {
                    if (p < 0) p += playerCount;
                    if (p == player) break;

                    int cafe = players[p].CheckTypeBuildings(TypeBuildings.cafe);
                    int shop = players[p].CheckTypeBuildings(TypeBuildings.shop);

                    int moneyTake = Mathf.Min(players[p].money, receivedMoney * (cafe + shop));
                    players[p].money -= moneyTake;
                    players[player].money += moneyTake;
                }
        }

        return "";
    }
}
