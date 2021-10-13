using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<TypeCard> typeCard;
    public TypeCard marketYellow = new TypeCard();//отдельный маркет достопримечательностей для каждого игрока
    public int playerNumber;
    private int _money = 3;
    private int[] _moneyTypeCard = new int[] { 0, 0, 0, 0, 0 };

    private int[] _moneyTest = new int[] { 0, 0, 0 };
    private int[,] _moneyTestTypeCard = new int[,] {
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 }};

    public int Money
    {
        get => _money;
    }

    public int money
    {
        get
        {
            if (GameManager.game.Stage == Stages.ChoosingOfDice)
            {
                return _moneyTest[GameManager.game.numberDiceTest];
            }
            else
                return _money;
        }

        set
        {
            if (GameManager.game.Stage == Stages.ChoosingOfDice)
            {
                int _delta = value - _moneyTest[GameManager.game.numberDiceTest];
                _moneyTest[GameManager.game.numberDiceTest] = value;

                _moneyTestTypeCard[GameManager.game.numberDiceTest, GameManager.game.numberTypeCardTest] += _delta;
            }
            else
            {
                int _delta = value - _money;
                _money = value;
                switch (GameManager.game.Stage)
                {
                    case Stages.RedBuildings:
                        _moneyTypeCard[0] += _delta;
                        break;

                    case Stages.BlueBuildings:
                        _moneyTypeCard[1] += _delta;
                        break;
                    case Stages.BlueCardsProperties:
                        _moneyTypeCard[1] += _delta;
                        break;

                    case Stages.GreenBuildings:
                        _moneyTypeCard[2] += _delta;
                        break;
                    case Stages.GreenCardsProperties:
                        _moneyTypeCard[2] += _delta;
                        break;

                    case Stages.PurpleBuildings:
                        _moneyTypeCard[3] += _delta;
                        break;
                    case Stages.PurpleCardsProperties:
                        _moneyTypeCard[3] += _delta;
                        break;

                    case Stages.YellowBuildings:
                        _moneyTypeCard[4] += _delta;
                        break;
                }
            }
        }
    }

    public int[] moneyTypeCard
    {
        get => _moneyTypeCard;
    }

    public void ResetMoneyTypeCard()
    {
        _moneyTypeCard = new int[] { 0, 0, 0, 0, 0 };
    }

    public void ResetMoneyTest()
    {
        for (int i = 0; i < _moneyTest.Length; i++)
        {
            _moneyTest[i] = _money;
        }

        _moneyTestTypeCard = new int[,]{
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }};
    }


    private void Awake()
    {
        typeCard = new List<TypeCard>();
        for (int i = 0; i < 5; i++)
        {
            TypeCard cards = new TypeCard();
            typeCard.Add(cards);
        }
    }

    public int CheckNameCard(string name, int typecard = -1)
    {
        int number = 0;
        for (int t = (typecard == -1 ? 0 : typecard); t < typeCard.Count; t++)
        {
            for (int c = 0; c < typeCard[t].cards.Count; c++)
            {
                if (typeCard[t].cards[c].nameCard == name)
                {
                    if (!typeCard[t].cards[c].repair) //Нужно ли учитывать здания на ремонте?
                        number++;
                }
            }

            if (typecard != -1) break;
        }
        return number;
    }

    public int CheckTypeBuildings(TypeBuildings name, int typecard = -1) //сколько магазинов, кафе и так далее
    {
        int number = 0;
        for (int t = (typecard == -1 ? 0 : typecard); t < typeCard.Count; t++)
        {
            for (int c = 0; c < typeCard[t].cards.Count; c++)
            {
                if (typeCard[t].cards[c].typeBuilding == name)
                {
                    //if (!typeCard[t].cards[c].repair) //Нужно ли учитывать здания на ремонте?
                    number++;
                }
            }

            if (typecard != -1) break;
        }
        return number;
    }

    public int CheckTypeCard(TypeCards typecard) //сколько достопримечательностей
    {
        return typeCard[(int)typecard].cards.Count;
    }

}

public class TypeCard
{
    public List<Cards> cards = new List<Cards>();
}
