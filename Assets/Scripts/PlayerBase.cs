using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class PlayerBase : MonoBehaviour
{
    public List<CardData> cards = new List<CardData>();
    public List<CardView> cardViews = new List<CardView>();
    public RectTransform playerHandUI;
    public Transform botHandPoint;
    public TextMeshProUGUI total_cards;
    public bool isBot;
    int total;


 void Update()
{
    if(!isBot){
    total_cards.text=total.ToString();}
}

    public int GetHandValue()
    {
         total = 0;
        int aceCount = 0;

        foreach (CardData card in cards)
        {
            if (card == null) continue;

            total += card.value;

            if (card.rank == CardRank.Ace)
                aceCount++;
        }

        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    public bool IsBust()
    {
        return GetHandValue() > 21;
    }

    public bool HasBlackjack()
    {
        return cards.Count == 2 && GetHandValue() == 21;
    }

    public CardView GetCardView(CardData data)
    {
        foreach (CardView card in cardViews)
        {
            if (card != null && card.cardData == data)
                return card;
        }

        return null;
    }

    public void AddCard(CardData data, CardView view)
    {
        if (data == null || view == null) return;

        cards.Add(data);
        cardViews.Add(view);
    }

    public void RemoveCard(CardData data)
    {
        cards.Remove(data);

        CardView view = GetCardView(data);
        if (view != null)
        {
            cardViews.Remove(view);
        }
    }

    public void ClearHand()
    {
        cards.Clear();

        foreach (CardView card in cardViews)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cardViews.Clear();
    }
}