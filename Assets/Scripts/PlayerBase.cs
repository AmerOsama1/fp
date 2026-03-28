using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class PlayerBase : MonoBehaviour
{
    public List<CardData> cards = new List<CardData>();

    public bool isBot;

    public RectTransform playerHandUI;
    public Transform botHandPoint;
    public TextMeshProUGUI total_cards;
    int total;


    // تخزين CardViews لتحسين الأداء
    public List<CardView> cardViews = new List<CardView>();
 void Update()
{
    if(!isBot){
    total_cards.text=total.ToString();}
}
    // حساب مجموع الكروت
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

        // تحويل Ace من 11 إلى 1 لو عدى 21
        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    // هل اللاعب خسر
    public bool IsBust()
    {
        return GetHandValue() > 21;
    }

    // هل بلاك جاك
    public bool HasBlackjack()
    {
        return cards.Count == 2 && GetHandValue() == 21;
    }

    // جلب CardView المرتبط بالـ CardData
    public CardView GetCardView(CardData data)
    {
        foreach (CardView card in cardViews)
        {
            if (card != null && card.cardData == data)
                return card;
        }

        return null;
    }

    // إضافة كرت
    public void AddCard(CardData data, CardView view)
    {
        if (data == null || view == null) return;

        cards.Add(data);
        cardViews.Add(view);
    }

    // إزالة كرت
    public void RemoveCard(CardData data)
    {
        cards.Remove(data);

        CardView view = GetCardView(data);
        if (view != null)
        {
            cardViews.Remove(view);
        }
    }

    // تصفير اليد لجولة جديدة
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