using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager instance;

    void Awake()
    {
        instance = this;
    }

    public void Vibrate()
    {
        if (PlayerPrefs.GetInt("Vibration", 1) == 0)
            return;

        Handheld.Vibrate();
    }
}