using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<CardData> hand = new List<CardData>();
    public GameObject cardPrefab;
    public RectTransform handArea;


    public void AddCard(CardData card)
    {
        hand.Add(card);
        GameObject cardObj = Instantiate(cardPrefab, handArea);
        CardView view = cardObj.GetComponent<CardView>();
        view.Setup(card, true);
    }

    public int GetHandValue()
    {
        int total = 0;
        int aceCount = 0;

        foreach (CardData card in hand)
        {
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
        return hand.Count == 2 && GetHandValue() == 21;
    }

    public void ClearHand()
    {
        hand.Clear();

        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }
    }
}