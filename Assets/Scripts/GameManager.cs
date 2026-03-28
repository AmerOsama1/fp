using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject resultPopup;
    public TextMeshProUGUI resultText;

    SoundManager _SoundManager;

    bool gameEnded = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        _SoundManager = SoundManager.Instance;
    }

    // ─── Check Bust ──────────────────────────────────────────
    public void CheckPlayerState(PlayerBase player)
    {
        if (gameEnded || player == null) return;
        if (player.GetHandValue() > 21)
        {
            gameEnded = true;
            PlayerLose();
        }
    }

    // ─── Compare Hands ───────────────────────────────────────
    public void CompareHands(PlayerBase player, PlayerBase dealer)
    {
        if (gameEnded || player == null || dealer == null) return;

        int playerValue = player.GetHandValue();
        int dealerValue = dealer.GetHandValue();
        gameEnded = true;

        if      (dealerValue > 21)             PlayerWin();
        else if (playerValue > dealerValue)    PlayerWin();
        else if (playerValue < dealerValue)    PlayerLose();
        else                                   Draw();
    }

    // ─── Win ─────────────────────────────────────────────────
    public void PlayerWin()
    {
        if (!gameEnded) gameEnded = true;

        int bet = MoneyManager.Instance.currentBet;
        MoneyManager.Instance.PlayerWin();

        ShowResult("YOU WIN +$" + bet);
        PlaySound(_SoundManager?.WinClip);
        Vibrate();
        StartCoroutine(RestartAfterDelay());
    }

    // ─── Lose ────────────────────────────────────────────────
    public void PlayerLose()
    {
        if (!gameEnded) gameEnded = true;

        int bet = MoneyManager.Instance.currentBet;
        MoneyManager.Instance.PlayerLose();

        ShowResult("YOU LOSE -$" + bet);
        PlaySound(_SoundManager?.LoseClip);
        Vibrate();
        StartCoroutine(RestartAfterDelay());
    }

    // ─── Draw ────────────────────────────────────────────────
    public void Draw()
    {
        if (!gameEnded) gameEnded = true;

        MoneyManager.Instance.Draw();

        ShowResult("DRAW!");
        Vibrate();
        StartCoroutine(RestartAfterDelay());
    }

    // ─── Surrender ───────────────────────────────────────────
    /// <summary>
    /// يتفعّل لما اللاعب يعمل Surrender.
    /// نص الرهان اترجع بالفعل في TurnManager.
    /// </summary>
    public void PlayerSurrender()
    {
        if (!gameEnded) gameEnded = true;

        int lost = MoneyManager.Instance.currentBet / 2;
        MoneyManager.Instance.currentBet = 0;

        ShowResult("SURRENDER -$" + lost);
        Vibrate();
        StartCoroutine(RestartAfterDelay());
    }

    // ─── Split Result ────────────────────────────────────────
    /// <summary>
    /// يحسم نتيجة كل يد في السبليت بشكل منفصل.
    /// </summary>
    public void ResolveSplitResult(bool h1Win, bool h1Push, bool h2Win, bool h2Push)
    {
        if (!gameEnded) gameEnded = true;

        int bet = MoneyManager.Instance.currentBet; // نفس الرهان لكل يد

        int totalReturn = 0;

        // اليد الأولى
        if (h1Win)        totalReturn += bet * 2;
        else if (h1Push)  totalReturn += bet;

        // اليد الثانية
        if (h2Win)        totalReturn += bet * 2;
        else if (h2Push)  totalReturn += bet;

        MoneyManager.Instance.playerMoney += totalReturn;
        MoneyManager.Instance.currentBet   = 0;
        MoneyManager.Instance.SaveMoneyPublic();
        MoneyManager.Instance.UpdateUIPublic();

        // نص النتيجة
        string h1Text = h1Win ? "H1: Win" : h1Push ? "H1: Push" : "H1: Lose";
        string h2Text = h2Win ? "H2: Win" : h2Push ? "H2: Push" : "H2: Lose";
        ShowResult(h1Text + "\n" + h2Text);

        bool anyWin = h1Win || h2Win;
        PlaySound(anyWin ? _SoundManager?.WinClip : _SoundManager?.LoseClip);
        Vibrate();
        StartCoroutine(RestartAfterDelay());
    }

    // ─── Helpers ─────────────────────────────────────────────
    void ShowResult(string msg)
    {
        if (resultPopup != null && resultText != null)
        {
            resultText.text = msg;
            resultPopup.SetActive(true);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (_SoundManager != null && clip != null)
            _SoundManager.PlaySoundclipOneShot(clip, _SoundManager.sfxSource);
    }

    void Vibrate()
    {
        if (VibrationManager.instance != null)
            VibrationManager.instance.Vibrate();
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
