using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteBar : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;
        int playerTurn = GameManager.game.playerTurn;

        bool townHall = (players[playerTurn].CheckNameCard("Town hall", (int)TypeCards.yellow) > 0);
        if (players[playerTurn].CheckTypeCard(TypeCards.yellow) > 2 + (townHall ? 1 : 0))//если у активного игрока 3 и более достопримечательности (Ратуш не учитывается)
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i] == parameter)
                    if (!repair)
                    {
                        int receivedmoney = players[playerTurn].money;
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
