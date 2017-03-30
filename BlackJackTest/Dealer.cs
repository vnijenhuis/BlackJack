using System;
using System.Collections.Generic;

public class Dealer
{

    public List<Card> DealerCards { get; set; }
    public Int32 HandValue { get; set; }
    public Int32 OpenCardValue { get; set; }

    public Dealer()
    {
        this.DealerCards = new List<Card>();
        this.HandValue = 0;
        this.OpenCardValue = 0;
    }

    internal void DrawStartingHands(Player player, Deck blackJackDeck)
    {
        for (int i = 0; i < 3; i++)
        {
            Card card = blackJackDeck.DrawCard();
            if (i == 1)
            {
                this.HandValue += card.CardValue;
            } else
            {
                if (card.ValueName.Equals("Ace"))
                {
                    Console.WriteLine("You drawed {0}! Do you want the value to be 1 or 10?", card.CardName);
                    card.CardValue = int.Parse(Console.ReadLine());
                    player.HandValue += card.CardValue;
                }
                else
                {
                    Console.WriteLine("You drawed {0}!", card.CardName);
                    player.HandValue += card.CardValue;
                }
            }
        }
    }

    public void ResetHand()
    {
        this.HandValue = 0;
        this.OpenCardValue = 0;
        this.DealerCards = new List<Card>();
    }

    public void Hit(Deck blackjackDeck)
    {
        Card card = blackjackDeck.DrawCard();
        this.DealerCards.Add(card);
        if (card.ValueName.Equals("Ace"))
        {
            Console.WriteLine("You drawed {0}! Do you want the value to be 1 or 10?", card.CardName);
            card.CardValue = int.Parse(Console.ReadLine());
            this.HandValue += card.CardValue;
        }
        else
        {
            Console.WriteLine("You drawed {0}!", card.CardName);
            this.HandValue += card.CardValue;
        }
    }

    public void OpenCard(Deck blackjackDeck)
    {
        Card card = blackjackDeck.DrawCard();
        this.DealerCards.Add(card);
        if (card.ValueName.Equals("Ace"))
        {
            Console.WriteLine("You drawed {0}! Do you want the value to be 1 or 10?", card.CardName);
            card.CardValue = int.Parse(Console.ReadLine());
            this.HandValue += card.CardValue;
            this.OpenCardValue = card.CardValue;
        }
        else
        {
            Console.WriteLine("You drawed {0}!", card.CardName);
            this.HandValue += card.CardValue;
            this.OpenCardValue = card.CardValue;
        }
    }
}
