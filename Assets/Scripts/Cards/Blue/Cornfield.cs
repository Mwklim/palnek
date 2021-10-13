using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cornfield : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        Player[] players = GameManager.game.players;

        bool townHall = (players[player].CheckNameCard("Town hall", (int)TypeCards.yellow) > 0);
        if (players[player].CheckTypeCard(TypeCards.yellow) < 2 + (townHall ? 1 : 0))//если не более 1 достопримечательности (Ратуш не учитывается)
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
