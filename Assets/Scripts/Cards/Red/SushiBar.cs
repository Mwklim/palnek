using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SushiBar : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;
        int playerTurn = GameManager.game.playerTurn;

        if (players[player].CheckNameCard("Port", (int)TypeCards.yellow) > 0)//если у владельца карты есть порт
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i] == parameter)
                    if (!repair)
                    {
                        int receivedmoney = receivedMoney + ((players[player].CheckNameCard("Shopping center", (int)TypeCards.yellow) > 0) ? 1 : 0);
                        int moneyTake = Mathf.Min(players[playerTurn].money, receivedmoney);

                        players[playerTurn].money -= moneyTake;
                        players[player].money += moneyTake;
                    }
                    else
                    {
                        repair = false;
                    }
            }


        return "";
    }
}
