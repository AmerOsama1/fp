using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Image cardImage;
    public Sprite cardBack;
    public CardData cardData;
    public bool isFaceUp = false;


    public void Setup(CardData card, bool faceUp)
    {
        if (cardImage == null)
        {
            Debug.LogError("Card Image Missing");
            return;
        }
        cardData = card;
        isFaceUp = faceUp;

        if (cardData == null)
            return;

        if (faceUp)
            cardImage.sprite = cardData.cardSprite;
        else
            cardImage.sprite = cardBack;
    }

    public void FlipCard()
    {
        if (cardData == null || cardImage == null)
            return;

        isFaceUp = true;
        cardImage.sprite = cardData.cardSprite;
    }

    public void HideCard()
    {
        if (cardImage == null)
            return;

        isFaceUp = false;
        cardImage.sprite = cardBack;
    }
}