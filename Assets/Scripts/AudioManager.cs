using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Sound Effects")]
    [SerializeField] private List<AudioClip> customerMoveSounds;
    [SerializeField] private List<AudioClip> playerNextSounds;
    [SerializeField] private List<AudioClip> drawCardSounds;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip marketCrashSound;
    [SerializeField] private AudioClip copCatchSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        SwitchMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        SwitchMusic(gameplayMusic);
    }

    private void SwitchMusic(AudioClip newTrack)
    {
        musicSource.Stop();
        if (newTrack != null)
        {
            musicSource.clip = newTrack;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("No music track found!");
        }
    }

    public void PlayCustomerMove()
    {
        if (customerMoveSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, customerMoveSounds.Count);
            effectsSource.PlayOneShot(customerMoveSounds[randomIndex]);
        }
    }

    public void PlayPlayerNext()
    {
        if (playerNextSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, playerNextSounds.Count);
            effectsSource.PlayOneShot(playerNextSounds[randomIndex]);
        }
    }

    public void PlayWinSound()
    {
        effectsSource.PlayOneShot(winSound);
    }

    public void PlayMarketCrashSound()
    {
        effectsSource.PlayOneShot(marketCrashSound);
    }

    public void PlayCopCatchSound()
    {
        effectsSource.PlayOneShot(copCatchSound);
    }

    public void PlayDrawCard()
    {
        if (drawCardSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, drawCardSounds.Count);
            effectsSource.PlayOneShot(drawCardSounds[randomIndex]);
        }
    }

    public void PlayCardSound(AudioClip cardSound)
    {
        effectsSource.PlayOneShot(cardSound);
    }
}
