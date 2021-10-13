using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager game;

    public event Action UpdatePositionCards = () => { };
    public event Action UpdateAnimationCards = () => { };
    public event Action UpdateCheckSelection = () => { };

    [Header("Prafab Cards")] //�������, �� ������� �������� �������� ������� �������� ��� ������ ����
    public List<GameObject> Cards_red;
    public List<GameObject> Cards_blue;
    public List<GameObject> Cards_green;
    public List<GameObject> Cards_purple;
    public List<GameObject> Cards_yellow;
    public List<GameObject> Cards_start;
    /////////////////////////////////////////////////////////////////////////////

    [Header("Market Settings")]
    public List<TypeCard> cardMarket;//����� ���� ����, ����� ������

    [Header("Players Settings")]
    public GameObject player;
    public GameObject playerTransform;
    public GameObject playerIcon;
    public Player[] players;
    public int playerCount;//���������� � 1
    public int playerTurn;//���������� � 0

    Stages _stage = Stages.ChoosingTheNumberOfDice;
    Stages nextStage;

    public GameObject DiceObject;
    int _numberDice = 1;//���������� ��������� �������
    int[] _dice = { 0, 0, 0, 0 }; //1, 2, 3 ������ � ��� �������� ��� �����
    int _noDice = -1;// ����� ������, ������� ����������� �� ������� (�� 0 �� 2) ��� -1 - ����������� ���, ����� 1��

    public List<ListChoosingOfDice> listChoosingOfDice;//�������� �������� ������� ��� ������
    public int numberDiceTest = 0;//��������������� ������ ���������� ����� ��� ������� �������� �������� ������� (��������)
    public int numberTypeCardTest = 0;//��� ����� ��� ��������������� �������

    bool replay = false;

    List<string> blueCardsProperties;
    List<string> greenCardsProperties;
    List<string> purpleCardsProperties;

    public List<string> CardsProperties
    {
        get
        {
            switch (_stage)
            {
                case Stages.BlueCardsProperties:
                    return blueCardsProperties;

                case Stages.GreenCardsProperties:
                    return greenCardsProperties;

                case Stages.PurpleCardsProperties:
                    return purpleCardsProperties;

                default:
                    return new List<string>();
            }
        }
    }

    bool expectation = false;//��������
    public bool property = false;//������������ �� � ������� �� ���� ��������?
    float timeExpectation = 0;//����� ��������, ���� ��� ��������

    int[] diceProperties;//�������������� ������ ��� ������� ��������

    public int playerProperty;//������� (��� ������) � ������ �... 
    public string[] �ardProperty;//������, �������, ������� ��� ������ ([0]) � ������� ([1]) �����


    int maxPlayers;
    float screenWidth;

    public Stages Stage
    {
        get => _stage;
        set
        {
            _stage = value;

            expectation = false;
            property = false;

            playerProperty = -1;
            �ardProperty = new string[] { "", "" };
        }
    }
    public int NumberDice
    {
        get => _numberDice;
        set
        {
            _numberDice = value;
            Stage = Stages.RollTheDice;
        }
    }


    public int Dice
    {
        get
        {
            if (_noDice == -1)
                return _dice[0];

            int dice = 0;
            for (int i = 0; i < _dice.Length; i++)
            {
                if (i != _noDice)
                    dice += _dice[i];
            }
            return dice;
        }
    }

    private void DiceSelection(int number, int selection)
    {
        _dice[number] = selection;

        switch (Stage)
        {
            case Stages.ChoosingOfDice:
                Stage = Stages.RedBuildings;
                break;
        }
    }

    public void DiceAdd()
    {
        DiceSelection(3, 2);
    }

    public void DiceNullify(int number)
    {
        DiceSelection(number, 0);
    }

    public void Properties(int player, string[] nameCompany)
    {
        playerProperty = player;
        �ardProperty = nameCompany;

        property = true;
    }

    public void �onfirm()
    {
        property = true;
    }

    private void Awake()
    {
        game = this;

        cardMarket = new List<TypeCard>();
        for (int i = 0; i < 5; i++)
        {
            TypeCard cards = new TypeCard();
            cardMarket.Add(cards);
        }

        InstantiatePlayer();

        InstantiateCardsMarket(Cards_red, TypeCards.red);
        InstantiateCardsMarket(Cards_green, TypeCards.green);
        InstantiateCardsMarket(Cards_blue, TypeCards.blue);
        InstantiateCardsMarket(Cards_purple, TypeCards.purple, playerCount + 1);

        for (int i = 0; i < Cards_yellow.Count; i++)
        {
            for (int p = 0; p < playerCount; p++)
            {
                GameObject cards = Instantiate(Cards_yellow[i]);
                players[p].marketYellow.cards.Add(cards.GetComponent<Cards>());
            }
        }

        for (int i = 0; i < Cards_start.Count; i++)
        {
            for (int p = 0; p < playerCount; p++)
            {
                GameObject cards = Instantiate(Cards_start[i]);
                Cards cards_c = cards.GetComponent<Cards>();
                cards_c.built = true;
                cards_c.player = playerCount;
                players[p].typeCard[(int)cards_c.typeCard].cards.Add(cards_c);
            }
        }
    }

    void InstantiateCardsMarket(List<GameObject> Cards_n, TypeCards typeCards, int count = 7)
    {
        for (int i = 0; i < Cards_n.Count; i++)
        {
            for (int p = 0; p < count; p++)
            {
                GameObject cards = Instantiate(Cards_n[i]);
                cardMarket[(int)typeCards].cards.Add(cards.GetComponent<Cards>());
            }
        }
    }

    void InstantiatePlayer()
    {
        players = new Player[playerCount];
        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObject = Instantiate(player);
            players[i] = playerObject.GetComponent<Player>();
            players[i].transform.parent = playerTransform.transform;
            players[i].playerNumber = i;
        }
    }

    void Start()
    {
        maxPlayers = 5;
        screenWidth = 33f * (Screen.width / 1080f) / (Screen.height / 1920f);

        for (int i = 0; i < players.Length; i++)
        {
            if (i == playerTurn)
            {
                players[i].GetComponent<SetTransform>().SetPositionNow = new Vector3(0, -24, 0);
                players[i].GetComponent<SetTransform>().SetPosition = new Vector3(0, -21, 0);
            }
            else
            {
                float x = (screenWidth / (maxPlayers - 1f)) * (i - (players.Length - 1) / 2f);
                players[i].GetComponent<SetTransform>().SetPositionNow = new Vector3(x, 24, 0);
                players[i].GetComponent<SetTransform>().SetPosition = new Vector3(x, 24, 0);
            }
        }
    }

    void Update()
    {
        UpdatePositionCards();
        UpdateAnimationCards();

        switch (Stage)
        {
            case Stages.ChoosingTheNumberOfDice:
                ChoosingTheNumberOfDice();
                break;

            case Stages.RollTheDice:
                RollTheDice();
                break;

            case Stages.ChoosingOfDice:
                ChoosingOfDice();
                break;

            case Stages.RedBuildings:
                RedBuildings();
                break;

            case Stages.BlueBuildings:
                BlueBuildings();
                break;
            case Stages.BlueCardsProperties:
                BlueCardsProperties();
                break;

            case Stages.GreenBuildings:
                GreenBuildings();
                break;
            case Stages.GreenCardsProperties:
                GreenCardsProperties();
                break;

            case Stages.PurpleBuildings:
                PurpleBuildings();
                break;
            case Stages.PurpleCardsProperties:
                PurpleCardsProperties();
                break;

            case Stages.YellowBuildings:
                YellowBuildings();
                break;


            case Stages.Building:
                Building();
                break;

            case Stages.EndBuilding:
                EndBuilding();
                break;

            case Stages.NextPlayerDetermination:
                NextPlayerDetermination();
                break;

            case Stages.Pause:
                Pause();
                break;

        }
    }

    void ChoosingTheNumberOfDice()
    {
        if (!expectation)
        {
            _noDice = 2;
            _dice = new int[] { 0, 0, 0, 0 };

            bool trainstation = (players[playerTurn].CheckNameCard("Train Station", (int)TypeCards.yellow) > 0);
            if (trainstation)
            {
                //������������ ���� ������
                expectation = true; //��������� ������� ������
            }
            else
            {
                NumberDice = 1;
            }
        }
    }

    void RollTheDice()
    {
        if (!expectation)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < NumberDice)
                    _dice[i] = UnityEngine.Random.Range(1, 7);
                else
                    _dice[i] = 0;
            }

            Vector3[] positionStartDice = new Vector3[]
            {
                new Vector3(-5.6f,-12.8f,-0.5f),
                new Vector3(0.6f,-14.2f,-0.5f),
                new Vector3(-2.5f,-13.5f,-0.5f)
            };

            for (int i = 0; i < NumberDice; i++) //������ ��������, ����� ������ ������ �������
            {
                GameObject diceObject = Instantiate(DiceObject);
                diceObject.GetComponent<Dice>().dice = _dice[i];
                diceObject.transform.position = positionStartDice[i];
            }

            expectation = true;
            timeExpectation = 3f;//������������ ��������
        }
        else
        {
            timeExpectation -= Time.deltaTime;
            if (timeExpectation <= 0)
            {
                if (NumberDice > 1)
                {
                    listChoosingOfDice = new List<ListChoosingOfDice>();
                    Stage = Stages.ChoosingOfDice;
                }
                else
                    Stage = Stages.RedBuildings;
            }
        }
    }
    void ChoosingOfDice()
    {
        if (!expectation)
        {
            _noDice = _numberDice > 1 ? 2 : -1;

            listChoosingOfDice = new List<ListChoosingOfDice>();//������ ��� ������ ��������� ������

            bool aquapark = (players[playerTurn].CheckNameCard("Aquapark", (int)TypeCards.yellow) > 0 && _numberDice > 1);
            bool tvtower = (players[playerTurn].CheckNameCard("TV tower", (int)TypeCards.yellow) > 0 && !replay);
            bool port = (players[playerTurn].CheckNameCard("Port", (int)TypeCards.yellow) > 0 && _numberDice > 1);

            if (aquapark || tvtower || port)
            {
                int numberChoices = 0;


                if (_numberDice > 1)
                {
                    for (int i = 0; i < 1 + (_numberDice < 3 ? 0 : 2); i++) //���������� ��� �������� � ��������� � listChoosingOfDice ������������ 
                    {
                        ListChoosingOfDice dice = new ListChoosingOfDice();

                        int noDice = 2 - i;
                        for (int d = 0; d < 3; d++)
                        {
                            if (noDice != d)
                            {
                                dice.dice.Add(_dice[d]);
                            }
                            else dice.noDice = noDice;
                        }

                        bool repetition = false;
                        for (int k = 0; k < listChoosingOfDice.Count; k++)
                        {
                            if ((dice.dice[0] == listChoosingOfDice[k].dice[0] && dice.dice[1] == listChoosingOfDice[k].dice[1]) ||
                                (dice.dice[0] == listChoosingOfDice[k].dice[1] && dice.dice[1] == listChoosingOfDice[k].dice[0]))
                            {
                                repetition = true;
                            }
                        }

                        if (!repetition)
                            listChoosingOfDice.Add(dice);
                    }

                    if (aquapark)
                    {
                        if (listChoosingOfDice.Count > 1)//���� ������ ���� ����� �� ���� 3 �������, �������� �� ����� 
                            numberChoices++;
                    }

                    if (port)
                    {
                        int countList = listChoosingOfDice.Count;
                        for (int k = 0; k < countList; k++)
                        {
                            if (listChoosingOfDice[k].Dice >= 10)
                            {
                                ListChoosingOfDice dice = new ListChoosingOfDice();
                                dice.dice.Add(listChoosingOfDice[k].dice[0]);
                                dice.dice.Add(listChoosingOfDice[k].dice[1]);
                                dice.dice.Add(2);
                                dice.noDice = listChoosingOfDice[k].noDice;

                                listChoosingOfDice.Add(dice);

                                numberChoices++;
                            }
                        }
                    }
                }
                else //��������� ���� ����� -> ����� ����� ���������� � �������� ���������
                {
                    ListChoosingOfDice dice = new ListChoosingOfDice();
                    dice.dice.Add(_dice[0]);
                    dice.noDice = -1;
                    listChoosingOfDice.Add(dice);
                }

                if (tvtower)
                {
                    ListChoosingOfDice dice = new ListChoosingOfDice();
                    listChoosingOfDice.Add(dice);

                    numberChoices++;
                }


                if (numberChoices > 0)
                {
                    for (int p = 0; p < players.Length; p++)
                        players[p].ResetMoneyTest();

                    for (int k = 0; k < listChoosingOfDice.Count; k++)//�������� ������ ��� ���������
                    {
                        if (listChoosingOfDice[k].dice.Count > 0)
                        {
                            _noDice = listChoosingOfDice[k].noDice;

                            _dice[3] = listChoosingOfDice[k].dice.Count > 2 ? 2 : 0;

                            numberTypeCardTest = 0;
                            RedBuildings(false);
                            numberTypeCardTest = 1;
                            BlueBuildings(false);
                            numberTypeCardTest = 2;
                            GreenBuildings(false);
                            numberTypeCardTest = 3;
                            PurpleBuildings(false);

                            _dice[3] = 0;
                        }
                    }

                    expectation = true; //��������� ������� ������
                }
                else
                    Stage = Stages.RedBuildings;
            }
            else
                Stage = Stages.RedBuildings;
        }
    }

    void RedBuildings(bool nextStage = true)
    {
        for (int i = playerTurn - 1; true; i--)
        {
            if (i < 0) i += playerCount;
            if (i == playerTurn) break;

            int dice = Dice;
            int typeCard = (int)TypeCards.red;
            for (int c = 0; c < players[i].typeCard[typeCard].cards.Count; c++)
            {
                players[i].typeCard[typeCard].cards[c].Properties(i, dice);
            }
        }

        if (nextStage)
            Stage = Stages.BlueBuildings;
    }

    void BlueBuildings(bool nextStage = true)
    {
        blueCardsProperties = new List<string>();
        for (int i = playerTurn - 1; true; i--)
        {
            if (i < 0) i += playerCount;
            int dice = Dice;
            int typeCard = (int)TypeCards.blue;
            for (int c = 0; c < players[i].typeCard[typeCard].cards.Count; c++)
            {
                string properties = players[i].typeCard[typeCard].cards[c].Properties(i, dice);
                if (properties != "") blueCardsProperties.Add(properties);
            }

            if (i == playerTurn) break;
        }

        if (nextStage)
            Stage = Stages.BlueCardsProperties;
    }
    void BlueCardsProperties()
    {
        if (blueCardsProperties.Count > 0)
        {
            switch (blueCardsProperties[0])
            {
                case "Trawler":
                    Trawler(blueCardsProperties[0]);
                    break;
            }
        }
        else
            Stage = Stages.GreenBuildings;
    }

    void GreenBuildings(bool nextStage = true)
    {
        greenCardsProperties = new List<string>();
        int dice = Dice;
        int typeCard = (int)TypeCards.green;
        for (int c = 0; c < players[playerTurn].typeCard[typeCard].cards.Count; c++)
        {
            string properties = players[playerTurn].typeCard[typeCard].cards[c].Properties(playerTurn, dice);
            if (properties != "") greenCardsProperties.Add(properties);
        }

        if (nextStage)
            Stage = Stages.GreenCardsProperties;
    }
    void GreenCardsProperties()
    {
        if (greenCardsProperties.Count > 0)
        {
            switch (greenCardsProperties[0])
            {
                case "Demolition company":
                    DemolitionCompany(greenCardsProperties[0]);
                    break;

                case "Credit bureau":
                    CreditBureau(greenCardsProperties[0]);
                    break;

                case "Transport company":
                    TransportCompany(greenCardsProperties[0]);
                    break;
            }
        }
        else
            Stage = Stages.PurpleBuildings;
    }

    void PurpleBuildings(bool nextStage = true)
    {
        purpleCardsProperties = new List<string>();
        int dice = Dice;
        int typeCard = (int)TypeCards.purple;
        for (int c = 0; c < players[playerTurn].typeCard[typeCard].cards.Count; c++)
        {
            string properties = players[playerTurn].typeCard[typeCard].cards[c].Properties(playerTurn, dice);
            if (properties != "") purpleCardsProperties.Add(properties);
        }

        if (nextStage)
            Stage = Stages.PurpleCardsProperties;
    }

    void PurpleCardsProperties()
    {
        if (purpleCardsProperties.Count > 0)
        {
            switch (purpleCardsProperties[0])
            {
                case "Telecentre":
                    Telecentre(purpleCardsProperties[0]);
                    break;
                case "Business center":
                    BusinessCenter(purpleCardsProperties[0]);
                    break;
                case "Building renovation company":
                    BuildingRenovationCompany(purpleCardsProperties[0]);
                    break;
                case "Conference center":
                    ConferenceCenter(purpleCardsProperties[0]);
                    break;
            }
        }
        else
            Stage = Stages.YellowBuildings;
    }

    void YellowBuildings()
    {
        bool townHall = (players[playerTurn].CheckNameCard("Town hall", (int)TypeCards.yellow) > 0);

        if (players[playerTurn].money == 0 && townHall) players[playerTurn].money++;
        Stage = Stages.Building;
    }

    void Building()
    {
        if (!expectation)
        {
            UpdateCheckSelection();

            playerProperty = -1;
            �ardProperty = new string[] { "", "" };

            expectation = true;
        }
        else if (property)
        {
            if (�ardProperty[0] != "")
            {
                BuyCard(�ardProperty[0]);
                Stage = Stages.Pause;

                nextStage = Stages.EndBuilding;
                timeExpectation = 2f;
            }
            else
            {
                bool airport = (players[playerTurn].CheckNameCard("Airport", (int)TypeCards.yellow) > 0);
                if (airport)
                {
                    players[playerTurn].money += 10;
                }
                Stage = Stages.EndBuilding;
            }
        }
    }

    void EndBuilding()
    {
        if (!expectation)
        {
            bool ventureFund = (players[playerTurn].CheckNameCard("Venture fund", (int)TypeCards.purple) > 0 && players[playerTurn].money > 0);

            if (ventureFund)
            {
                //������������ ���� ������
                playerProperty = -1;
                �ardProperty = new string[] { "", "" };

                expectation = true; //��������� ������� ������
            }
            else
                Stage = Stages.NextPlayerDetermination;
        }
        else if (property)
        {
            if (playerProperty > 0)
            {
                players[playerTurn].money--;
                int typeCard = (int)TypeCards.purple;
                for (int c = players[playerTurn].typeCard[typeCard].cards.Count - 1; c >= 0; c--)
                {
                    if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == "Venture fund")
                    {
                        players[playerTurn].typeCard[typeCard].cards[c].receivedMoney++;
                    }
                }
            }
            Stage = Stages.NextPlayerDetermination;
        }
    }
    void NextPlayerDetermination()
    {
        if (!expectation)
        {
            replay = false;

            bool nextPlayer = true;
            bool tvTower = (players[playerTurn].CheckNameCard("TV tower", (int)TypeCards.yellow) > 0);


            if (NumberDice > 1 && tvTower)
            {
                if (_noDice >= 0 && _noDice < 3)
                    _dice[_noDice] = 0;

                nextPlayer = !(_dice[0] == _dice[1] || _dice[0] == _dice[2] || _dice[1] == _dice[2]);
            }

            if (nextPlayer)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (i == playerTurn)
                    {
                        float x = (screenWidth / (maxPlayers - 1f)) * (i - (players.Length - 1) / 2f);
                        players[i].GetComponent<SetTransform>().SetPosition = new Vector3(0, -24, 0);
                        players[i].GetComponent<SetTransform>().SetPositionLast = new Vector3(x, 24, 0);
                    }
                }

                playerTurn++;
                if (playerTurn >= playerCount) playerTurn -= playerCount;

                expectation = true;
                timeExpectation = 1f;//������������ �������� ��� ����� ���� ��������� ������
            }
            else
            {
                Stage = Stages.ChoosingTheNumberOfDice;
            }
        }
        else
        {
            timeExpectation -= Time.deltaTime;
            if (timeExpectation <= 0)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (i == playerTurn)
                    {
                        players[i].GetComponent<SetTransform>().SetPositionNow = new Vector3(0, -24, 0);
                        players[i].GetComponent<SetTransform>().SetPosition = new Vector3(0, -21, 0);
                    }
                }

                Stage = Stages.Pause;

                nextStage = Stages.ChoosingTheNumberOfDice;
                timeExpectation = 1f;
            }
        }
    }

    void Pause()
    {
        timeExpectation -= Time.deltaTime;
        if (timeExpectation <= 0)
        {
            Stage = nextStage;
        }
    }

    #region ������ ����������� �������

    void Trawler(string nameCompany)
    {
        if (!expectation)
        {
            diceProperties = new int[] { 0, 0 };
            for (int i = 0; i < diceProperties.Length; i++)
            {
                diceProperties[i] = UnityEngine.Random.Range(1, 7);
            }
            //������ ��������, ����� ������ ������ �������
            expectation = true;
            timeExpectation = 3f;
        }
        else
        {
            timeExpectation -= Time.deltaTime;
            if (timeExpectation <= 0)
            {
                int typeCard = (int)TypeCards.blue;
                for (int i = playerTurn - 1; true; i--)
                {
                    if (i < 0) i += playerCount;
                    int dice = diceProperties[0] + diceProperties[1];

                    for (int c = 0; c < players[i].typeCard[typeCard].cards.Count; c++)
                    {
                        if (players[i].typeCard[typeCard].cards[c].nameCard == nameCompany)
                            players[i].typeCard[typeCard].cards[c].Properties(i, dice);
                    }

                    if (i == playerTurn) break;
                }
                while (true) { if (!blueCardsProperties.Remove(nameCompany)) break; }
                expectation = false;
            }
        }
    }

    void DemolitionCompany(string nameCompany)
    {
        if (!expectation)
        {
            bool townHall = (players[playerTurn].CheckNameCard("Town hall", (int)TypeCards.yellow) > 0);
            if (players[playerTurn].CheckTypeCard(TypeCards.yellow) > (townHall ? 1 : 0))//������, ��� ����� �0 �� �����������
            {
                UpdateCheckSelection(); //������������ ���� ������ ����������������������

                playerProperty = playerTurn;
                �ardProperty = new string[] { "", "" };

                expectation = true;
            }
            else
                greenCardsProperties.Remove(nameCompany);
        }
        else if (property)
        {
            int typeCard = (int)TypeCards.green;
            for (int c = 0; c < players[playerTurn].typeCard[typeCard].cards.Count; c++)
            {
                if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == nameCompany)
                {
                    players[playerTurn].typeCard[typeCard].cards[c].Properties(playerTurn);//�� �������� +8 �����
                    break;
                }
            }

            typeCard = (int)TypeCards.yellow;
            for (int c = 0; c < players[playerTurn].typeCard[typeCard].cards.Count; c++)
            {
                if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == �ardProperty[0])
                {
                    players[playerTurn].typeCard[typeCard].cards[c].built = false;
                    players[playerTurn].typeCard[typeCard].cards[c].player = -1;

                    players[playerTurn].marketYellow.cards.Add(players[playerTurn].typeCard[typeCard].cards[c]);
                    players[playerTurn].typeCard[typeCard].cards.RemoveAt(c);
                    break;
                }
            }
            greenCardsProperties.Remove(nameCompany);

            expectation = false;
            property = false;
        }
    }
    void CreditBureau(string nameCompany)
    {
        int typeCard = (int)TypeCards.green;
        for (int c = 0; c < players[playerTurn].typeCard[typeCard].cards.Count; c++)
        {
            if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == nameCompany)
            {
                players[playerTurn].typeCard[typeCard].cards[c].Properties(playerTurn);//�� ������ -2
                break;
            }
        }

        greenCardsProperties.Remove(nameCompany);

        expectation = false;
        property = false;
    }
    void TransportCompany(string nameCompany)
    {
        if (!expectation)
        {
            if (players[playerTurn].CheckTypeCard(TypeCards.red) + players[playerTurn].CheckTypeCard(TypeCards.blue) + players[playerTurn].CheckTypeCard(TypeCards.green) > 1)
            {
                //������������ ����� ������
                UpdateCheckSelection();  //������������ ���� ������ ������� ���� � ������, �������� ����� �������� ��������

                playerProperty = -1;
                �ardProperty = new string[] { "", "" };

                expectation = true;
            }
            else
                greenCardsProperties.Remove(nameCompany);
        }
        else if (property)
        {
            int typeCard = (int)TypeCards.green;
            for (int c = 0; c < players[playerTurn].typeCard[typeCard].cards.Count; c++)
            {
                if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == nameCompany)
                {
                    players[playerTurn].typeCard[typeCard].cards[c].Properties(playerTurn);//�� �������� +4 ������
                    break;
                }
            }

            GiveCard(�ardProperty[0], playerTurn, playerProperty);

            greenCardsProperties.Remove(nameCompany);

            expectation = false;
            property = false;
        }
    }

    void Telecentre(string nameCompany)
    {
        if (!expectation)
        {
            //�������� ��������, ���� �� ������ � ����������� �������, ����� �������� ������ �� ��� (���� ������ ���� ������ � ������, ��� � ���� ������ ���� ���������, ���������� ����� � ������� �����������???)
            if (true)
            {
                //������������ ���� ������ ������, �������� ������ ������

                playerProperty = -1;
                �ardProperty = new string[] { "", "" };

                expectation = true;
            }
            else
                purpleCardsProperties.Remove(nameCompany);
        }
        else if (property)
        {
            int typeCard = (int)TypeCards.purple;
            for (int c = 0; c < players[playerTurn].typeCard[typeCard].cards.Count; c++)
            {
                if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == nameCompany)
                {
                    players[playerTurn].typeCard[typeCard].cards[c].Properties(playerTurn, playerProperty);//playerTurn - ����������� ������, playerProperty - �������� ������
                    break;
                }
            }

            purpleCardsProperties.Remove(nameCompany);

            expectation = false;
            property = false;
        }
    }
    void BusinessCenter(string nameCompany)
    {
        if (!expectation)
        {
            if (players[playerTurn].CheckTypeCard(TypeCards.red) + players[playerTurn].CheckTypeCard(TypeCards.blue) + players[playerTurn].CheckTypeCard(TypeCards.green) > 1)
            {
                //������������ ���� ������ ������
                UpdateCheckSelection(); //������������ ���� ������ ������

                playerProperty = -1;
                �ardProperty = new string[] { "", "" };

                expectation = true;
            }
            else
                purpleCardsProperties.Remove(nameCompany);
        }
        else if (property)
        {
            if (�ardProperty[0] != "" && �ardProperty[1] != "")// 0 - ���������� �����, 1 - ���������� �����
            {
                GiveCard(�ardProperty[0], playerTurn, playerProperty);
                GiveCard(�ardProperty[1], playerProperty, playerTurn);
            }

            purpleCardsProperties.Remove(nameCompany);

            expectation = false;
            property = false;
        }
    }
    void BuildingRenovationCompany(string nameCompany)
    {
        if (!expectation)
        {
            UpdateCheckSelection(); //������������ ���� ������ ��������, ������� ����� ������� �� ������ (���� ������ - ������)
            playerProperty = -1;
            �ardProperty = new string[] { "", "" };

            expectation = true;
        }
        else if (property)
        {
            for (int p = playerTurn - 1; true; p--)
            {
                if (p < 0) p += playerCount;

                for (int typeCard = 0; typeCard < 5; typeCard++)//����� ���� "�ardProperty[0]", ������� ����� ������� �� ������
                {
                    for (int c = players[p].typeCard[typeCard].cards.Count - 1; c >= 0; c--)
                    {
                        if (players[p].typeCard[typeCard].cards[c].nameCard == �ardProperty[0] && !players[p].typeCard[typeCard].cards[c].repair)
                        {
                            players[p].typeCard[typeCard].cards[c].repair = true;

                            if (p != playerTurn)
                            {
                                int moneyTake = Mathf.Min(players[p].money, 1);
                                players[p].money -= moneyTake;
                                players[playerTurn].money += moneyTake;
                            }
                        }
                    }
                }
                if (p == playerTurn) break;
            }

            purpleCardsProperties.Remove(nameCompany);

            expectation = false;
            property = false;
        }
    }
    void ConferenceCenter(string nameCompany)
    {
        if (!expectation)
        {
            int repair = 0;
            for (int typeCard = 0; typeCard < 5; typeCard++)//����� ����, ������� ����� ������� � �������
            {
                for (int c = players[playerTurn].typeCard[typeCard].cards.Count - 1; c >= 0; c--)
                {
                    if (players[playerTurn].typeCard[typeCard].cards[c].repair)
                    {
                        repair++;
                    }
                }
            }

            if (repair > 0)
            {
                //��������� �� �������� ���� ������
                UpdateCheckSelection(); //������������ ���� ������ ��������, ������� ����� ����� � �������
                playerProperty = -1;
                �ardProperty = new string[] { "", "" };

                expectation = true;
            }
            else purpleCardsProperties.Remove(nameCompany);
        }
        else if (property)
        {
            if (�ardProperty[0] != "")
            {
                int typeCard;
                for (typeCard = 0; typeCard < 5; typeCard++)//����� ���� "�ardProperty[0]", ������� ����� ������� � �������
                {
                    for (int c = players[playerTurn].typeCard[typeCard].cards.Count - 1; c >= 0; c--)
                    {
                        if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == �ardProperty[0] && players[playerTurn].typeCard[typeCard].cards[c].repair)
                        {
                            players[playerTurn].typeCard[typeCard].cards[c].repair = false;
                        }
                    }
                }

                typeCard = (int)TypeCards.purple;
                for (int c = players[playerTurn].typeCard[typeCard].cards.Count - 1; c >= 0; c--) //���������� ����� �� �����
                {
                    if (players[playerTurn].typeCard[typeCard].cards[c].nameCard == nameCompany)
                    {
                        players[playerTurn].typeCard[(int)TypeCards.purple].cards[c].built = false;
                        players[playerTurn].typeCard[(int)TypeCards.purple].cards[c].player = -1;

                        cardMarket[typeCard].cards.Add(players[playerTurn].typeCard[(int)TypeCards.purple].cards[c]);
                        players[playerTurn].typeCard[typeCard].cards.RemoveAt(c);
                        break;
                    }
                }
            }

            purpleCardsProperties.Remove(nameCompany);

            expectation = false;
            property = false;
        }
    }


    void GiveCard(string nameCard, int givePlaye, int takePlayer)
    {
        bool remuve = false;
        for (int typeCard = 0; typeCard < 5; typeCard++)//����� ����� "nameCard", ������� ���������� �� ������ � givePlaye � ������ � takePlayer
        {
            for (int c = players[givePlaye].typeCard[typeCard].cards.Count - 1; c >= 0; c--)
            {
                if (players[givePlaye].typeCard[typeCard].cards[c].nameCard == nameCard)
                {
                    players[givePlaye].typeCard[typeCard].cards[c].player = takePlayer;
                    players[takePlayer].typeCard[typeCard].cards.Add(players[givePlaye].typeCard[typeCard].cards[c]);
                    players[givePlaye].typeCard[typeCard].cards.RemoveAt(c);
                    remuve = true;
                    break;
                }
            }
            if (remuve) break;
        }
    }

    #endregion


    void BuyCard(string nameCard)
    {
        bool buy = false;
        for (int typeCard = 0; typeCard < 4; typeCard++)//����� ����� "nameCard", ������� ���������� �� ������ � givePlaye � ������ � takePlayer
        {
            for (int c = cardMarket[typeCard].cards.Count - 1; c >= 0; c--)//������� ������� ����� � �������� ��
            {
                if (cardMarket[typeCard].cards[c].nameCard == nameCard)
                {
                    players[playerTurn].typeCard[typeCard].cards.Add(cardMarket[typeCard].cards[c]);
                    cardMarket[typeCard].cards[c].Construction(playerTurn);
                    cardMarket[typeCard].cards.RemoveAt(c);
                    buy = true;
                    break;
                }
            }
            if (buy) break;
        }

        if (!buy)
        {
            int typeCard = (int)TypeCards.yellow;
            for (int c = players[playerTurn].marketYellow.cards.Count - 1; c >= 0; c--)
            {
                if (players[playerTurn].marketYellow.cards[c].nameCard == nameCard)
                {
                    players[playerTurn].typeCard[typeCard].cards.Add(players[playerTurn].marketYellow.cards[c]);
                    players[playerTurn].marketYellow.cards[c].Construction(playerTurn);
                    players[playerTurn].marketYellow.cards.RemoveAt(c);
                    break;
                }
            }
        }
    }

}
