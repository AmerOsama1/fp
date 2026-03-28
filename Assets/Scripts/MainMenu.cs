using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Toggle soundToggle;
    public Toggle vibrationToggle;
    public Toggle ambientToggle;
    public Toggle musicToggle;

    void Start()
    {
        soundToggle.isOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        ambientToggle.isOn = PlayerPrefs.GetInt("Ambient", 1) == 1;
        musicToggle.isOn = PlayerPrefs.GetInt("Music", 1) == 1;
    }

  public void ToggleSound(bool value)
{
    PlayerPrefs.SetInt("Sound", value ? 1 : 0);
    SoundManager.Instance.UpdateAudioSettings();
}

public void ToggleMusic(bool value)
{
    PlayerPrefs.SetInt("Music", value ? 1 : 0);
    SoundManager.Instance.UpdateAudioSettings();
}
    public void ToggleVibration(bool value)
    {
        PlayerPrefs.SetInt("Vibration", value ? 1 : 0);
    }

    public void ToggleAmbient(bool value)
    {
        PlayerPrefs.SetInt("Ambient", value ? 1 : 0);
    }

 

    public void Play(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}