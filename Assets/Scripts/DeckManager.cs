using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    SoundManager _SoundManager;

    public List<CardData> deck = new List<CardData>();
    public List<PlayerBase> players = new List<PlayerBase>();

    public Transform deckSpawnPoint;
    public GameObject cardPrefab;

    public int cardsPerPlayer = 2;

    AudioSource Sc;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        _SoundManager = SoundManager.Instance;
        Sc = GetComponent<AudioSource>();
        GenerateDeck();
        Shuffle();
       // Debug.Log("Deck Count: " + deck.Count);
    }

    void GenerateDeck()
    {
        deck.Clear();
        CardData[] cards = Resources.LoadAll<CardData>("Cards");
        foreach (CardData card in cards)
        {
            if (card != null)
                deck.Add(card);
        }
    }

    void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);

            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public CardData DrawCard()
    {
        if (deck.Count == 0)
        {
            //Debug.LogWarning("Deck Empty");
            return null;
        }

        CardData card = deck[0];
        deck.RemoveAt(0);

        return card;
    }

    public void StartRound()
    {
        foreach (PlayerBase p in players)
        {
            if (p != null)
                p.ClearHand();
        }

        Shuffle();

        StartCoroutine(DealStartCards());
    }

    IEnumerator DealStartCards()
    {
        for (int i = 0; i < cardsPerPlayer; i++)
        {
            foreach (PlayerBase player in players)
            {
                if (player == null) continue;

                GiveCard(player, i);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    public void GiveCard(PlayerBase player, int cardIndex = -1)
    {
        if (player == null || cardPrefab == null || deckSpawnPoint == null)
            return;

        CardData card = DrawCard();

        if (card == null) return;

        GameObject cardObj = Instantiate(
            cardPrefab,
            deckSpawnPoint.position,
            Quaternion.identity,
            deckSpawnPoint.parent
        );

        if (_SoundManager != null)
            _SoundManager.PlaySoundclipOneShot(_SoundManager.CardClip, Sc);

        CardView view = cardObj.GetComponent<CardView>();
        bool faceUp = true;

        if (player.isBot && cardIndex == 1)
            faceUp = false;

        view.Setup(card, faceUp);

        player.cards.Add(card);

        CardMover mover = cardObj.GetComponent<CardMover>();

        if (player.isBot)
        {
            mover.MoveToWorld(player.botHandPoint);
            StartCoroutine(SetParentAfterMove(cardObj, player.botHandPoint));
        }
        else
        {
            mover.MoveToWorld(player.playerHandUI);
            StartCoroutine(SetParentAfterMove(cardObj, player.playerHandUI));
        }

        GameManager.instance.CheckPlayerState(player);
    }

    IEnumerator SetParentAfterMove(GameObject card, Transform hand)
    {
        yield return new WaitForSeconds(0.4f);

        if (card != null && hand != null)
            card.transform.SetParent(hand, false);
    }

    public void PlayerHit()
    {
        if (players.Count == 0) return;

        PlayerBase player = players[0];

        GiveCard(player);

        if (VibrationManager.instance != null)
            VibrationManager.instance.Vibrate();
    }
}