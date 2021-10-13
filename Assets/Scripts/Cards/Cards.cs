using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Cards : MonoBehaviour, IPointerClickHandler
{
    public string nameCard; //имя карты
    public int player = -1;//чья карта? (-1 - маркет)

    public SetTransform positionCard;
    public AnimationCards animationCard;

    public BoxCollider2D boxCollider2D;

    public TypeCards typeCard; //тип карты (красный, синий и т.д.)
    public TypeBuildings typeBuilding; //тип карты (магазин, кафе и т.д.)

    public int price; //цена стриотельства
    public int receivedMoney;//сколько монет по умолчанию получает игрок за карточку 

    public bool lastСard; //последняя карточка в стопке? (используется для отметки, что карточки закончились)
    public bool availability; //доступно для нажатия?
    public bool built; //построено? (для всех, кроме желтых = true)
    public bool repair; //на ремонте? 

    public int[] dice;//суммарное значение кубиков для активации

    /// <summary>
    /// функция активации свойства карты (получение монет, доступность свойств (порт, налоговая и т.д.)) возвращает имя свойства для дальнейшего его запуска 
    /// </summary>
    /// <param name="player">игрок, чья карта запускается</param>
    /// <param name="parameter">универсальный параметр для свойств карт (сумма выпавших кубиков, количество монет и т.д.)</param>
    /// <returns>имя специального свойства для дальнейшего запуска</returns>
    public abstract string Properties(int player, int parameter = 0);

    private void Awake()
    {
        positionCard = gameObject.AddComponent(typeof(SetTransform)) as SetTransform;
        animationCard = gameObject.AddComponent(typeof(AnimationCards)) as AnimationCards;
        boxCollider2D = gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        boxCollider2D.size = new Vector2(2f, 2f);
        GameManager.game.UpdateCheckSelection += CheckSelection;
    }

    private void OnDestroy()
    {
        GameManager.game.UpdateCheckSelection -= CheckSelection;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<string> cardProperties;
        switch (GameManager.game.Stage)
        {
            case Stages.GreenCardsProperties:
                cardProperties = GameManager.game.CardsProperties;
                if (cardProperties.Count > 0)
                    switch (cardProperties[0])
                    {
                        case "Demolition company"://активируем только построенные достопримечательности (кроме ратуши) игрока, который ходит 
                            if (availability)
                                GameManager.game.сardProperty[0] = nameCard;
                            break;
                        case "Transport company"://активируем все карты игрока, который ходит, кроме достопримечательностей и фиолетовых карт
                            if (availability)
                                GameManager.game.сardProperty[0] = nameCard;
                            break;
                    }
                break;


            case Stages.PurpleCardsProperties:
                cardProperties = GameManager.game.CardsProperties;
                if (cardProperties.Count > 0)
                    switch (cardProperties[0])
                    {
                        case "Business center":
                            if (availability)
                            {
                                if (player == GameManager.game.playerTurn)
                                    GameManager.game.сardProperty[0] = nameCard;

                                if (player == GameManager.game.playerProperty)
                                    GameManager.game.сardProperty[1] = nameCard;
                            }
                            break;
                        case "Building renovation company":
                            if (availability)
                                GameManager.game.сardProperty[0] = nameCard;
                            break;
                        case "Conference center":
                            if (availability)
                                GameManager.game.сardProperty[0] = nameCard;
                            break;
                    }
                break;

            case Stages.Building:
                if (availability)
                    GameManager.game.сardProperty[0] = nameCard;
                break;

            default:

                break;
        }

        GameManager.game.property = true;
    }

    /// <summary>
    /// функция строительства
    /// </summary>
    /// <param name="player">игрок, покупающий карту</param>
    public void Construction(int player)
    {
        if (GameManager.game.players[player].money >= price)
        {
            GameManager.game.players[player].money -= price;
            built = true;
            this.player = player;
        }
    }

    /// <summary>
    /// проверка на доступность выбора
    /// </summary>
    void CheckSelection()
    {
        List<string> cardProperties;

        switch (GameManager.game.Stage)
        {
            case Stages.GreenCardsProperties:
                cardProperties = GameManager.game.CardsProperties;
                if (cardProperties.Count > 0)
                    switch (cardProperties[0])
                    {
                        case "Demolition company"://активируем только построенные достопримечательности (кроме ратуши) игрока, который ходит 
                            if (typeCard == TypeCards.yellow && built && player == GameManager.game.playerTurn && price > 0)
                            {
                                availability = true;
                            }
                            else availability = false;
                            break;
                        case "Transport company"://активируем все карты игрока, который ходит, кроме достопримечательностей и фиолетовых карт
                            if (typeCard != TypeCards.yellow && typeCard != TypeCards.purple && built && player == GameManager.game.playerTurn)
                            {
                                availability = true;
                            }
                            else availability = false;
                            break;
                    }
                break;

            case Stages.PurpleCardsProperties:
                cardProperties = GameManager.game.CardsProperties;
                if (cardProperties.Count > 0)
                    switch (cardProperties[0])
                    {
                        case "Business center":
                            if (typeCard != TypeCards.yellow && typeCard != TypeCards.purple && built && (player == GameManager.game.playerTurn || player == GameManager.game.playerProperty))
                            {
                                availability = true;
                            }
                            else availability = false;
                            break;
                        case "Building renovation company":
                            if (typeCard != TypeCards.yellow && typeCard != TypeCards.purple && !built)
                            {
                                availability = true;
                            }
                            else availability = false;
                            break;
                        case "Conference center":
                            if (typeCard != TypeCards.yellow && typeCard != TypeCards.purple && built && player == GameManager.game.playerTurn && repair)
                            {
                                availability = true;
                            }
                            else availability = false;
                            break;
                    }
                break;

            case Stages.Building:
                if (lastСard)
                {
                    availability = false;
                    break;
                }

                if (player == -1 && GameManager.game.players[GameManager.game.playerTurn].money >= price)
                {
                    availability = true;
                }
                else availability = false;

                break;

            default:
                availability = false;
                break;
        }
    }
}



