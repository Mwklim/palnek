using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    List<List<CardsTransform>> CardsTransforms = new List<List<CardsTransform>>();
    List<CardsTransform> NameCards = new List<CardsTransform>();

    [SerializeField] float screenWidth = 11f;
    [SerializeField] float maxCards = 7f;
    [SerializeField] float distanceY = 1.2f;

    GameManager game;// = GameManager.game;

    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            List<CardsTransform> cardsTransform = new List<CardsTransform>();
            CardsTransforms.Add(cardsTransform);
        }
    }

    void Start()
    {
        game = GameManager.game;

        screenWidth = 11f * (Screen.width / 1080f) / (Screen.height / 1920f);
        maxCards = Mathf.Max(Mathf.Round(5f * screenWidth / 11f), 5f);

        for (int i = 0; i < 4; i++)
            SetSorting(i);
        SetUniteCards();
        SetPositionCards();
        SetPositionTransform();


        SkipAnimationSetPositionTransform();
    }

    void Update()
    {
        SetPositionCards();
    }

    void SetSorting(int typeCard)//сортировка карточек каждого типа
    {
        List<CardsTransform> nameCards = new List<CardsTransform>();

        for (int c = 0; c < game.cardMarket[typeCard].cards.Count; c++)
        {
            bool add = false;
            for (int n = 0; n < nameCards.Count; n++)
            {
                if (game.cardMarket[typeCard].cards[c].nameCard == nameCards[n].nameCards)
                {
                    add = true;
                    break;
                }
            }

            if (!add)
            {
                CardsTransform cardsTransform = new CardsTransform();
                cardsTransform.nameCards = game.cardMarket[typeCard].cards[c].nameCard;
                cardsTransform.dice = game.cardMarket[typeCard].cards[c].dice[0] + (game.cardMarket[typeCard].cards[c].dice.Length - 1) / 10f;
                cardsTransform.typeCard = typeCard;
                nameCards.Add(cardsTransform);
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
                float x = (screenWidth / (maxCards - 1f)) * (i - (count - 1) / 2f);
                nameCards[cardNumber].position = new Vector3(x, -p * distanceY, 0);
                cardNumber++;
            }
        }

        NameCards = nameCards;
    }

    void SetPositionCards()//Задать каждой карточке рассчитанное положение 
    {
        for (int n = 0; n < NameCards.Count; n++)
            NameCards[n].count = 0;

        for (int typeCard = 0; typeCard < 4; typeCard++)
            for (int c = 0; c < game.cardMarket[typeCard].cards.Count; c++)
            {
                Cards cards = game.cardMarket[typeCard].cards[c];

                for (int n = 0; n < NameCards.Count; n++)
                    if (cards.nameCard == NameCards[n].nameCards && typeCard == NameCards[n].typeCard)
                    {
                        cards.transform.parent = gameObject.transform;
                        cards.positionCard.SetPosition = NameCards[n].position - new Vector3(0, Mathf.Max(NameCards[n].count - 1, 0) / 20f, 0);
                        if (NameCards[n].count == 0) cards.lastСard = true;//последняя карточка остается для обозначения, что все уже раскуплено, эту карточку купить нельзя
                        if (cards.animationCard.spriteRenderer == null) cards.animationCard.spriteRenderer = cards.GetComponentInChildren<SpriteRenderer>();
                        if (cards.animationCard.spriteRenderer != null) cards.animationCard.spriteRenderer.sortingOrder = NameCards[n].count;

                        NameCards[n].count++;
                        break;
                    }
            }
    }

    void SetPositionTransform()//Центруем маркет
    {
        int lvlCardsAll = 0;
        for (int i = 0; i < CardsTransforms.Count; i++)
            lvlCardsAll += CardsTransforms[i].Count;

        lvlCardsAll = Mathf.CeilToInt(lvlCardsAll / maxCards);

        float startY = (lvlCardsAll / 2f) + 2.5f;
        gameObject.GetComponent<SetTransform>().SetPosition = new Vector3(0, startY, 0);
    }

    void SkipAnimationSetPositionTransform()//Центруем маркет
    {
        for (int typeCard = 0; typeCard < 4; typeCard++)
            for (int c = 0; c < game.cardMarket[typeCard].cards.Count; c++)
            {
                Cards cards = game.cardMarket[typeCard].cards[c];
                cards.positionCard.SetPositionNow = cards.positionCard.SetPosition;
            }

        int lvlCardsAll = 0;
        for (int i = 0; i < CardsTransforms.Count; i++)
            lvlCardsAll += CardsTransforms[i].Count;

        lvlCardsAll = Mathf.CeilToInt(lvlCardsAll / maxCards);

        float startY = (lvlCardsAll / 2f) + 2.5f;
        gameObject.GetComponent<SetTransform>().SetPositionNow = new Vector3(0, startY, 0);
    }

}
