using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Blackjack/Card")]
public class CardData : ScriptableObject
{
    public CardSuit suit;
    public CardRank rank;
    public int value;
    public Sprite cardSprite;
}

public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public enum CardRank
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}