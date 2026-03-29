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
    public GameObject Chips;
    const string MoneyKey = "PlayerMoney";

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
        if(Chips!=null) Chips.SetActive(false);
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
        ShowChips();
        SaveMoney();
        UpdateUI();
    }
    public void ShowChips(){
         if(Chips!=null) Chips.SetActive(true);
    }

    public void Draw()
    {
        playerMoney += currentBet;
        SaveMoney();
        UpdateUI();
    }


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
