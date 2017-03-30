using System;

public class Card
{
    public String CardType { get; set; }
    public String CardName { get; set; }
    public String ValueName { get; set; }
    public Int32 CardValue { get; set; }

    public Card(String type, String valueName, Int32 value)
    {
        CardValue = value;
        CardType = type;
        ValueName = valueName;
        CardName = ValueName + " of " + CardType;
	}
}
