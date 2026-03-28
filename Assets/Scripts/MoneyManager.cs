using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int playerMoney = 1000;
    public int currentBet  = 0;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI betText;
   public GameObject[] COins;
    const string MoneyKey = "PlayerMoney";

    // حد فتح الشيبسات الكبيرة
    const int HighRollerThreshold = 2500;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        LoadMoney();
        UpdateUI();
        if (playerMoney <= 0) ResetMoney();
    }
    void Update(){
        if(playerMoney>1000){
            foreach (var item in COins)
            {
                item.SetActive(true);
                
            }
        }
        else{
             foreach (var item in COins)
            {
                item.SetActive(false);
                
            }
        }
    }

    // ─── Bet ─────────────────────────────────────────────────
    public void AddBet(int amount)
    {
        if (playerMoney >= amount && amount > 0)
        {
            currentBet   += amount;
            playerMoney  -= amount;
            SaveMoney();
            UpdateUI();
        }
    }

    public void ClearBet()
    {
        playerMoney += currentBet;
        currentBet   = 0;
        SaveMoney();
        UpdateUI();
    }

    // ─── Results ─────────────────────────────────────────────
    public void PlayerWin()
    {
        playerMoney += currentBet * 2;
        SaveMoney();
        UpdateUI();
    }

    public void PlayerLose()
    {
        SaveMoney();
        UpdateUI();
    }

    public void Draw()
    {
        playerMoney += currentBet;
        SaveMoney();
        UpdateUI();
    }

    // ─── Chips ───────────────────────────────────────────────
    /// <summary>
    /// يرجع الشيبسات المتاحة حسب رصيد اللاعب.
    /// استخدمها في الـ UI عشان تعرض الشيبسات الصح.
    /// </summary>
    public int[] GetAvailableChips()
    {
        if (playerMoney >= HighRollerThreshold)
            return new int[] { 5, 10, 25, 50, 100, 250, 500, 750, 1000 };
        else
            return new int[] { 5, 10, 25, 50, 100 };
    }

    // ─── Public wrappers (used by TurnManager) ────────────────
    public void SaveMoneyPublic()  => SaveMoney();
    public void UpdateUIPublic()   => UpdateUI();

    // ─── Internal ────────────────────────────────────────────
    void UpdateUI()
    {
        if (moneyText != null) moneyText.text = "$" + playerMoney;
        if (betText   != null) betText.text   = "Bet: $" + currentBet;
    }

    void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, playerMoney);
        PlayerPrefs.Save();
    }

    void LoadMoney()
    {
        playerMoney = PlayerPrefs.HasKey(MoneyKey)
            ? PlayerPrefs.GetInt(MoneyKey)
            : 1000;

        if (!PlayerPrefs.HasKey(MoneyKey)) SaveMoney();
    }

    public void ResetMoney()
    {
        playerMoney = 1000;
        currentBet  = 0;
        SaveMoney();
        UpdateUI();
        SceneManager.LoadScene("game");
    }
}
