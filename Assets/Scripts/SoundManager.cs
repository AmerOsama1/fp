using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip CardClip;
    public AudioClip LoseClip;
    public AudioClip WinClip;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
  
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // تشغيل صوت عادي
    public void PlaySoundclip(AudioClip clip, bool play, AudioSource source)
    {
        if (clip == null || source == null)
            return;

        source.clip = clip;

        if (play)
            source.Play();
        else
            source.Stop();
    }

    // تشغيل صوت OneShot
    public void PlaySoundclipOneShot(AudioClip clip, AudioSource source)
    {
        if (clip == null || source == null)
            return;

        source.PlayOneShot(clip);
    }

    // تشغيل صوت في مكان معين
    public void PlaySoundclipOnPlace(AudioClip clip)
    {
        if (clip == null)
            return;

        AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    // تحديث إعدادات الصوت
    public void UpdateAudioSettings()
    {
        bool soundOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        bool musicOn = PlayerPrefs.GetInt("Music", 1) == 1;

        if (sfxSource != null)
            sfxSource.mute = !soundOn;

        if (musicSource != null)
            musicSource.mute = !musicOn;
    }
}