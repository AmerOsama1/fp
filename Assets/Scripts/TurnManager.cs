using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public PlayerBase player;
    public PlayerBase dealer;
    public DeckManager deckManager;
    public PlayerBase splitPlayer;

    public bool playerTurn = true;
    bool gameEnded    = false;
    bool isDoubleDown = false;

    // Split
    bool isSplit          = false;
    bool playingSecondHand = false;

    // Surrender
    bool hasSurrendered = false;

    // Insurance
    bool insuranceOffered = false;
    bool insuranceTaken   = false;
    int  insuranceBet     = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (player == null || dealer == null || deckManager == null)
            Debug.LogError("TurnManager references missing!");
    }

    // ─── Start Game ───────────────────────────────────────────
    public void StartGame()
    {
        if (MoneyManager.Instance.currentBet <= 0) return;

        gameEnded         = false;
        isDoubleDown      = false;
        isSplit           = false;
        playingSecondHand = false;
        hasSurrendered    = false;
        insuranceTaken    = false;
        insuranceBet      = 0;

        deckManager.StartRound();
        playerTurn = true;

        // عرض الإنشورانس لو الديلر شايل Ace
        if (dealer.cards.Count > 0 && dealer.cards[0].rank == CardRank.Ace)
            OfferInsurance();
    }

    // ─── Hit ─────────────────────────────────────────────────
    public void PlayerHit()
    {
        if (!playerTurn || gameEnded) return;

        PlayerBase activeHand = playingSecondHand ? splitPlayer : player;
        deckManager.GiveCard(activeHand);

        if (activeHand.IsBust())
        {
            if (isSplit && !playingSecondHand)
                SwitchToSecondHand();
            else
            {
                playerTurn = false;
                gameEnded  = true;
                GameManager.instance.PlayerLose();
            }
        }
    }

    // ─── Stand ───────────────────────────────────────────────
    public void PlayerStand()
    {
        if (!playerTurn || gameEnded) return;

        if (isSplit && !playingSecondHand)
        {
            SwitchToSecondHand();
            return;
        }

        playerTurn = false;
        RevealDealerCards();
        StartCoroutine(DealerTurn());
    }

    // ─── Double Down ─────────────────────────────────────────
    public void PlayerDoubleDown()
    {
        if (!playerTurn || gameEnded) return;

        PlayerBase activeHand = playingSecondHand ? splitPlayer : player;

        if (activeHand.cards.Count != 2)
        {
            Debug.Log("Double only on first move");
            return;
        }

        isDoubleDown = true;
        MoneyManager.Instance.currentBet *= 2;

        deckManager.GiveCard(activeHand);

        if (activeHand.IsBust())
        {
            playerTurn = false;
            gameEnded  = true;
            GameManager.instance.PlayerLose();
            return;
        }

        PlayerStand();
    }

    // ─── Split ───────────────────────────────────────────────
    public void PlayerSplit()
    {
        if (!playerTurn || gameEnded) return;

        if (player.cards.Count != 2)
        {
            Debug.Log("Split requires exactly 2 cards.");
            return;
        }

        if (player.cards[0].rank != player.cards[1].rank)
        {
            Debug.Log("Split requires two cards of the same rank.");
            return;
        }

        if (splitPlayer == null)
        {
            Debug.LogError("splitPlayer not assigned in Inspector!");
            return;
        }

        int extraBet = MoneyManager.Instance.currentBet;
        if (MoneyManager.Instance.playerMoney < extraBet)
        {
            Debug.Log("Not enough balance to split.");
            return;
        }

        // خصم رهان ثاني
        MoneyManager.Instance.playerMoney -= extraBet;

        isSplit           = true;
        playingSecondHand = false;

        // نقل الكارت الثاني لـ splitPlayer
      if (player.cards.Count < 2)
{
    Debug.Log("Split requires exactly 2 cards.");
    return;
}

CardData movedCard = player.cards[1];
CardView movedView = player.GetCardView(movedCard);

if (movedView == null)
{
    Debug.LogError("Split failed: missing CardView!");
    return;
}

player.RemoveCard(movedCard);
splitPlayer.AddCard(movedCard, movedView);

if (!splitPlayer.isBot)
    movedView.transform.SetParent(splitPlayer.playerHandUI);
else
    movedView.transform.SetParent(splitPlayer.botHandPoint);

        // كل يد تاخد كارت جديد
        deckManager.GiveCard(player);
        deckManager.GiveCard(splitPlayer);

        Debug.Log("Split done - now playing hand 1");
    }

    void SwitchToSecondHand()
    {
        playingSecondHand = true;
        Debug.Log("Now playing split hand 2");
    }

    // ─── Surrender ───────────────────────────────────────────
    public void PlayerSurrender()
    {
        if (!playerTurn || gameEnded) return;

        if (player.cards.Count != 2)
        {
            Debug.Log("Surrender only allowed on opening hand.");
            return;
        }

        hasSurrendered = true;
        playerTurn     = false;
        gameEnded      = true;

        int half = MoneyManager.Instance.currentBet / 2;
        MoneyManager.Instance.playerMoney += half;
        MoneyManager.Instance.currentBet   = 0;

        MoneyManager.Instance.SaveMoneyPublic();
        MoneyManager.Instance.UpdateUIPublic();

        GameManager.instance.PlayerSurrender();
    }

    // ─── Insurance ───────────────────────────────────────────
    void OfferInsurance()
    {
        insuranceOffered = true;
        Debug.Log("Insurance offered - dealer shows Ace");
        // استدعي الـ UI بتاعك هنا عشان تظهر زرار الإنشورانس
    }

    public void TakeInsurance()
    {
        if (!insuranceOffered) return;

        insuranceBet = MoneyManager.Instance.currentBet / 2;

        if (MoneyManager.Instance.playerMoney < insuranceBet)
        {
            Debug.Log("Not enough for insurance.");
            return;
        }

        MoneyManager.Instance.playerMoney -= insuranceBet;
        insuranceTaken   = true;
        insuranceOffered = false;

        MoneyManager.Instance.SaveMoneyPublic();
        MoneyManager.Instance.UpdateUIPublic();

        Debug.Log("Insurance taken: $" + insuranceBet);
    }

    public void DeclineInsurance()
    {
        insuranceTaken   = false;
        insuranceBet     = 0;
        insuranceOffered = false;
        Debug.Log("Insurance declined.");
    }

    void ResolveInsurance()
    {
        if (!insuranceTaken) return;

        bool dealerBJ = dealer.GetHandValue() == 21 && dealer.cards.Count == 2;
        if (dealerBJ)
        {
            MoneyManager.Instance.playerMoney += insuranceBet * 3;
            MoneyManager.Instance.SaveMoneyPublic();
            MoneyManager.Instance.UpdateUIPublic();
            Debug.Log("Insurance wins! +" + (insuranceBet * 3));
        }
        else
        {
            Debug.Log("Insurance lost.");
        }

        insuranceTaken = false;
        insuranceBet   = 0;
    }

    // ─── Dealer Turn ─────────────────────────────────────────
    IEnumerator DealerTurn()
    {
        yield return new WaitForSeconds(1f);

        while (dealer.GetHandValue() < 17 && !gameEnded)
        {
            deckManager.GiveCard(dealer);
            yield return new WaitForSeconds(1f);
        }

        if (!gameEnded)
            EndGame();
    }

    // ─── End Game ────────────────────────────────────────────
    void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        ResolveInsurance();

        int  dealerValue = dealer.GetHandValue();
        bool dealerBust  = dealer.IsBust();

        if (isSplit)
        {
            ResolveSplitHands(dealerValue, dealerBust);
            return;
        }

        int playerValue = player.GetHandValue();

        if (dealerBust || playerValue > dealerValue)
            GameManager.instance.PlayerWin();
        else if (playerValue < dealerValue)
            GameManager.instance.PlayerLose();
        else
            GameManager.instance.Draw();
    }

    void ResolveSplitHands(int dealerValue, bool dealerBust)
    {
        int hand1 = player.GetHandValue();
        int hand2 = splitPlayer.GetHandValue();

        bool h1Win  = !player.IsBust()      && (dealerBust || hand1 > dealerValue);
        bool h2Win  = !splitPlayer.IsBust() && (dealerBust || hand2 > dealerValue);
        bool h1Push = !player.IsBust()      && hand1 == dealerValue;
        bool h2Push = !splitPlayer.IsBust() && hand2 == dealerValue;

        GameManager.instance.ResolveSplitResult(h1Win, h1Push, h2Win, h2Push);
    }

    // ─── Helpers ─────────────────────────────────────────────
    void RevealDealerCards()
    {
        foreach (CardView card in dealer.GetComponentsInChildren<CardView>())
            if (!card.isFaceUp) card.FlipCard();
    }

    public bool IsSplitActive()       => isSplit;
    public bool IsPlayingSecondHand() => playingSecondHand;
    public bool IsInsuranceOffered()  => insuranceOffered;
}
