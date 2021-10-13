using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConferenceCenter : Cards
{
    public override string Properties(int player, int parameter = 0)
    {
        if (GameManager.game.Stage == Stages.PurpleBuildings)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i] == parameter)
                    return nameCard;
            }
        }

        return "";
    }
}
