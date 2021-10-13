using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandPlayer : MonoBehaviour
{
    public List<GameObject> typeCardsTransform;
    Player player;
    GameManager game;
    SortingGroup sortingGroup;

    public List<List<CardsTransform>> CardsTransforms = new List<List<CardsTransform>>();
    List<CardsTransform> NameCards = new List<CardsTransform>();

    float screenWidth = 11f;
    float maxCards = 5f;
    float distanceY = 1.2f;

    private void Awake()
    {
        player = GetComponent<Player>();
        for (int i = 0; i < 5; i++)
        {
            List<CardsTransform> cardsTransform = new List<CardsTransform>();
            CardsTransforms.Add(cardsTransform);
        }
    }
    void Start()
    {
        game = GameManager.game;
        sortingGroup = GetComponent<SortingGroup>();

        screenWidth = 11f * (Screen.width / 1080f) / (Screen.height / 1920f);
        maxCards = Mathf.Max(Mathf.Round(5f * screenWidth / 11f), 5f);

        for (int i = 0; i < 5; i++)
            SetSorting(i);
        SetUniteCards();
        SetPosition();
        SetPositionTransform();

        SkipAnimationSetPositionTransform();
    }

    void Update()
    {
        for (int i = 0; i < 5; i++)
            SetSorting(i);
        SetUniteCards();
        SetPosition();
        SetPositionTransform();

        sortingGroup.sortingOrder = 10 + (game.playerTurn == player.playerNumber ? 10 : 0);
    }

    void SetSorting(int typeCard)//сортировка карточек
    {
        List<CardsTransform> nameCards = new List<CardsTransform>();

        for (int c = 0; c < player.typeCard[typeCard].cards.Count; c++)
        {
            bool add = false;
            for (int n = 0; n < nameCards.Count; n++)
            {
                if (player.typeCard[typeCard].cards[c].nameCard == nameCards[n].nameCards)
                {
                    add = true;
                    break;
                }
            }

            if (!add)
            {
                CardsTransform cardsTransform = new CardsTransform();
                cardsTransform.nameCards = player.typeCard[typeCard].cards[c].nameCard;
                if (typeCard == (int)TypeCards.yellow)
                {
                    cardsTransform.dice = player.typeCard[typeCard].cards[c].price;
                }
                else
                    cardsTransform.dice = player.typeCard[typeCard].cards[c].dice[0] + (player.typeCard[typeCard].cards[c].dice.Length - 1) / 10f;
                cardsTransform.typeCard = typeCard;
                nameCards.Add(cardsTransform);
            }
        }

        if (typeCard == (int)TypeCards.yellow)
        {
            for (int c = 0; c < player.marketYellow.cards.Count; c++)
            {
                bool add = false;
                for (int n = 0; n < nameCards.Count; n++)
                {
                    if (player.marketYellow.cards[c].nameCard == nameCards[n].nameCards)
                    {
                        add = true;
                        break;
                    }
                }

                if (!add)
                {
                    CardsTransform cardsTransform = new CardsTransform();
                    cardsTransform.nameCards = player.marketYellow.cards[c].nameCard;
                    cardsTransform.dice = player.marketYellow.cards[c].price;
                    cardsTransform.typeCard = typeCard;
                    nameCards.Add(cardsTransform);
                }
            }
        }

        for (int i = 0; i < nameCards.Count - 1; i++)
        {
            for (int n = i + 1; n < nameCards.Count; n++)
            {
                if (nameCards[i].dice > nameCards[n].dice)
                {
                    CardsTransform cardsTransform = nameCards[i];
                    nameCards[i] = nameCards[n];
                    nameCards[n] = cardsTransform;
                }
            }
        }
        CardsTransforms[typeCard] = nameCards;
    }

    void SetUniteCards()//объединение типов карточек и распределение по рядам
    {
        List<CardsTransform> nameCards = new List<CardsTransform>();

        for (int c = 0; c < CardsTransforms[(int)TypeCards.purple].Count; c++)
            nameCards.Add(CardsTransforms[(int)TypeCards.purple][c]);

        for (int c = 0; c < CardsTransforms[(int)TypeCards.red].Count; c++)
            nameCards.Add(CardsTransforms[(int)TypeCards.red][c]);

        for (int c = 0; c < CardsTransforms[(int)TypeCards.blue].Count; c++)
            nameCards.Add(CardsTransforms[(int)TypeCards.blue][c]);

        for (int c = 0; c < CardsTransforms[(int)TypeCards.green].Count; c++)
            nameCards.Add(CardsTransforms[(int)TypeCards.green][c]);

        PositionCalculation(nameCards);
        NameCards = nameCards;

        PositionCalculation(CardsTransforms[(int)TypeCards.yellow], true);
    }

    private void PositionCalculation(List<CardsTransform> nameCards, bool setYellow = false) //расчет положения карточек
    {
        float screenWidth = this.screenWidth;
        float maxCards = this.maxCards;

        if (setYellow)
        {
            screenWidth = (this.screenWidth * (this.maxCards - 1 - Mathf.CeilToInt(this.maxCards / 5f)) / (this.maxCards - 1));// - ((Screen.width / 1080f) / (Screen.height / 1920f)) / 2f;
            //maxCards = Mathf.Max(Mathf.Round(5f * screenWidth / 11f), 4f);
            maxCards = Mathf.Max(Mathf.Round(this.maxCards * screenWidth / this.screenWidth), 4f);// maxCards - Mathf.CeilToInt(((Screen.width / 1080f) / (Screen.height / 1920f)) / 4f);// Mathf.Max(Mathf.Round(4f * screenWidth / (11f - 11f / 4f)), 4f);   
        }

        float countLvl = Mathf.CeilToInt(nameCards.Count / maxCards);
        int cardNumber = 0;

        for (int p = 0; p < countLvl; p++)
        {
            float count;
            if ((countLvl - 1) == p)
            {
                //count = nameCards.Count - (countLvl - 1f) * Mathf.CeilToInt(nameCards.Count / countLvl);
                count = nameCards.Count - (countLvl - 1f) * maxCards;
            }
            else
            {
                //count = Mathf.CeilToInt(nameCards.Count / countLvl);
                count = Mathf.Min(maxCards, nameCards.Count);// > 5 ? 5 : nameCards.Count;//  Mathf.CeilToInt(nameCards.Count / countLvl);
            }

            for (int i = 0; i < count; i++)
            {
                //float x = (screenWidth / (maxCards - 1f)) * (i - (count - 1) / 2f
                float x = (screenWidth / (maxCards - 1f)) * (i - (maxCards - 1) / 2f);
                nameCards[cardNumber].position = new Vector3(x, -p * distanceY, 0);
                cardNumber++;
            }
        }
    }

    void SetPosition() //задать положение карточек
    {
        for (int n = 0; n < NameCards.Count; n++)
            NameCards[n].count = 0;

        for (int i = 0; i < 4; i++) //задаем положение обычным картам
        {
            int typeCard = i;
            for (int c = 0; c < player.typeCard[typeCard].cards.Count; c++)
            {
                Cards cards = player.typeCard[typeCard].cards[c];

                for (int n = 0; n < NameCards.Count; n++)
                    if (cards.nameCard == NameCards[n].nameCards && typeCard == NameCards[n].typeCard)
                    {
                        cards.transform.parent = typeCardsTransform[1].transform;//трансформ общих карт
                        cards.positionCard.SetPosition = NameCards[n].position - new Vector3(0, NameCards[n].count / 20f, 0);

                        if (cards.animationCard.spriteRenderer == null) cards.animationCard.spriteRenderer = cards.GetComponentInChildren<SpriteRenderer>();//когда будет переделаны карты, поменять
                        if (cards.animationCard.spriteRenderer != null) cards.animationCard.spriteRenderer.sortingOrder = NameCards[n].count;

                        NameCards[n].count++;
                        break;
                    }
            }
        }

        { //задаем положение купленным достопримечательностям 
            int typeCard = (int)TypeCards.yellow;
            for (int c = 0; c < player.typeCard[typeCard].cards.Count; c++)
            {
                Cards cards = player.typeCard[typeCard].cards[c];

                for (int n = 0; n < CardsTransforms[typeCard].Count; n++)
                    if (cards.nameCard == CardsTransforms[typeCard][n].nameCards && typeCard == CardsTransforms[typeCard][n].typeCard)
                    {
                        cards.transform.parent = typeCardsTransform[0].transform;//трансформ достопримечательностей
                        cards.positionCard.SetPosition = CardsTransforms[typeCard][n].position;

                        if (cards.animationCard.spriteRenderer == null) cards.animationCard.spriteRenderer = cards.GetComponentInChildren<SpriteRenderer>();//когда будет переделаны карты, поменять
                        if (cards.animationCard.spriteRenderer != null) cards.animationCard.spriteRenderer.sortingOrder = CardsTransforms[typeCard][n].count;
                        break;
                    }
            }
        }

        for (int c = 0; c < player.marketYellow.cards.Count; c++) //задаем положение не купленным достопримечательностям 
        {
            int typeCard = (int)TypeCards.yellow;

            Cards cards = player.marketYellow.cards[c];

            for (int n = 0; n < CardsTransforms[typeCard].Count; n++)
                if (cards.nameCard == CardsTransforms[typeCard][n].nameCards)
                {
                    cards.transform.parent = typeCardsTransform[0].transform;//трансформ достопримечательностей
                    cards.positionCard.SetPosition = CardsTransforms[typeCard][n].position;

                    if (cards.animationCard.spriteRenderer == null) cards.animationCard.spriteRenderer = cards.GetComponentInChildren<SpriteRenderer>();//когда будет переделаны карты, поменять
                    if (cards.animationCard.spriteRenderer != null) cards.animationCard.spriteRenderer.sortingOrder = CardsTransforms[typeCard][n].count;
                    break;
                }
        }
    }

    void SetPositionTransform() //задать положение трансформ
    {
        float maxCard = Mathf.Max(Mathf.Round(maxCards * (maxCards - 1 - Mathf.CeilToInt(maxCards / 5f)) / (maxCards - 1)), 4f);

        float startX = (1f - (maxCards - 1 - Mathf.CeilToInt(maxCards / 5f)) / (maxCards - 1)) * screenWidth / 2f;
        float startY = 11f - Mathf.Max((2f - Mathf.CeilToInt(CardsTransforms[(int)TypeCards.yellow].Count / maxCard)) * distanceY / 2f, 0);

        typeCardsTransform[0].GetComponent<SetTransform>().SetPosition = new Vector3(startX, startY, 0);

        startY -= Mathf.Max(Mathf.CeilToInt(CardsTransforms[(int)TypeCards.yellow].Count / Mathf.Max(Mathf.Round(4 * screenWidth / 11f), 4)), 2) * distanceY;

        //int lvlCardsAll = 0;
        //for (int i = 0; i < CardsTransforms.Count - 1; i++)
        //    lvlCardsAll += CardsTransforms[i].Count;

        typeCardsTransform[1].GetComponent<SetTransform>().SetPosition = new Vector3(0, startY, 0);
    }

    void SkipAnimationSetPositionTransform()
    {
        for (int typeCard = 0; typeCard < 5; typeCard++) //задаем положение обычным картам
        {
            for (int c = 0; c < player.typeCard[typeCard].cards.Count; c++)
            {
                player.typeCard[typeCard].cards[c].positionCard.SetPositionNow = player.typeCard[typeCard].cards[c].positionCard.SetPosition;
            }
        }

        for (int c = 0; c < player.marketYellow.cards.Count; c++) //задаем положение не купленным достопримечательностям 
        {
            player.marketYellow.cards[c].positionCard.SetPositionNow = player.marketYellow.cards[c].positionCard.SetPosition;
        }


        float maxCard = Mathf.Max(Mathf.Round(maxCards * (maxCards - 1 - Mathf.CeilToInt(maxCards / 5f)) / (maxCards - 1)), 4f);

        float startX = (1f - (maxCards - 1 - Mathf.CeilToInt(maxCards / 5f)) / (maxCards - 1)) * screenWidth / 2f;
        float startY = 11f - (2f - Mathf.CeilToInt(CardsTransforms[(int)TypeCards.yellow].Count / maxCard)) * distanceY;

        typeCardsTransform[0].GetComponent<SetTransform>().SetPositionNow = new Vector3(startX, startY, 0);

        startY -= Mathf.Max(Mathf.CeilToInt(CardsTransforms[(int)TypeCards.yellow].Count / Mathf.Max(Mathf.Round(4 * screenWidth / 11f), 4)), 2) * distanceY;

        typeCardsTransform[1].GetComponent<SetTransform>().SetPositionNow = new Vector3(0, startY, 0);
    }

}
