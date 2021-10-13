using System.Collections.Generic;
public class ListChoosingOfDice
{
    public int noDice = -1;
    public List<int> dice = new List<int>();

    public int Dice
    {
        get
        {
            int _dice = 0;
            foreach(int i in dice)
            {
                _dice += i;
            }
            return _dice;
        }
    }
}
