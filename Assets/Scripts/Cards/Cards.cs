using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Cards : MonoBehaviour, IPointerClickHandler
{
    public string nameCard; //��� �����
    public int player = -1;//��� �����? (-1 - ������)

    public SetTransform positionCard;
    public AnimationCards animationCard;

    public BoxCollider2D boxCollider2D;

    public TypeCards typeCard; //��� ����� (�������, ����� � �.�.)
    public TypeBuildings typeBuilding; //��� ����� (�������, ���� � �.�.)

    public int price; //���� �������������
    public int receivedMoney;//������� ����� �� ��������� �������� ����� �� �������� 

    public bool last�ard; //��������� �������� � ������? (������������ ��� �������, ��� �������� �����������)
    public bool availability; //�������� ��� �������?
    public bool built; //���������? (��� ����, ����� ������ = true)
    public bool repair; //�� �������? 

    public int[] dice;//��������� �������� ������� ��� ���������

    /// <summary>
    /// ������� ��������� �������� ����� (��������� �����, ����������� ������� (����, ��������� � �.�.)) ���������� ��� �������� ��� ����������� ��� ������� 
    /// </summary>
    /// <param name="player">�����, ��� ����� �����������</param>
    /// <param name="parameter">������������� �������� ��� ������� ���� (����� �������� �������, ���������� ����� � �.�.)</param>
    /// <returns>��� ������������ �������� ��� ����������� �������</returns>
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
                        case "Demolition company"://���������� ������ ����������� ��������������������� (����� ������) ������, ������� ����� 
                            if (availability)
                                GameManager.game.�ardProperty[0] = nameCard;
                            break;
                        case "Transport company"://���������� ��� ����� ������, ������� �����, ����� ���������������������� � ���������� ����
                            if (availability)
                                GameManager.game.�ardProperty[0] = nameCard;
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
                                    GameManager.game.�ardProperty[0] = nameCard;

                                if (player == GameManager.game.playerProperty)
                                    GameManager.game.�ardProperty[1] = nameCard;
                            }
                            break;
                        case "Building renovation company":
                            if (availability)
                                GameManager.game.�ardProperty[0] = nameCard;
                            break;
                        case "Conference center":
                            if (availability)
                                GameManager.game.�ardProperty[0] = nameCard;
                            break;
                    }
                break;

            case Stages.Building:
                if (availability)
                    GameManager.game.�ardProperty[0] = nameCard;
                break;

            default:

                break;
        }

        GameManager.game.property = true;
    }

    /// <summary>
    /// ������� �������������
    /// </summary>
    /// <param name="player">�����, ���������� �����</param>
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
    /// �������� �� ����������� ������
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
                        case "Demolition company"://���������� ������ ����������� ��������������������� (����� ������) ������, ������� ����� 
                            if (typeCard == TypeCards.yellow && built && player == GameManager.game.playerTurn && price > 0)
                            {
                                availability = true;
                            }
                            else availability = false;
                            break;
                        case "Transport company"://���������� ��� ����� ������, ������� �����, ����� ���������������������� � ���������� ����
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
                if (last�ard)
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



