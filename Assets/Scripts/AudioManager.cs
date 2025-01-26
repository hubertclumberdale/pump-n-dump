using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource customerEffectsSource;  // Add this line

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

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float customerSoundVolume = 0.3f;  // Add volume control for customer sounds
    [Range(0f, 1f)] public float mainEffectsVolume = 1f;      // Add volume control for other effects

    [Header("Sound Settings")]
    [Range(0f, 1f)] public float pitchVariation = 0.2f;  // Add this for pitch randomization

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Configure audio sources
            if (effectsSource != null)
            {
                effectsSource.volume = mainEffectsVolume;
            }
            if (customerEffectsSource != null)
            {
                customerEffectsSource.volume = customerSoundVolume;
            }
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

    private float GetRandomPitch()
    {
        return 1f + Random.Range(-pitchVariation, pitchVariation);
    }

    public void PlayCustomerMove()
    {
        if (customerMoveSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, customerMoveSounds.Count);
            customerEffectsSource.pitch = GetRandomPitch();
            customerEffectsSource.PlayOneShot(customerMoveSounds[randomIndex], customerSoundVolume);
        }
    }

    public void PlayPlayerNext()
    {
        if (playerNextSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, playerNextSounds.Count);
            effectsSource.PlayOneShot(playerNextSounds[randomIndex], mainEffectsVolume);
        }
    }

    public void PlayWinSound()
    {
        effectsSource.PlayOneShot(winSound, mainEffectsVolume);
    }

    public void PlayMarketCrashSound()
    {
        effectsSource.PlayOneShot(marketCrashSound, mainEffectsVolume);
    }

    public void PlayCopCatchSound()
    {
        effectsSource.PlayOneShot(copCatchSound, mainEffectsVolume);
    }

    public void PlayDrawCard()
    {
        if (drawCardSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, drawCardSounds.Count);
            effectsSource.PlayOneShot(drawCardSounds[randomIndex], mainEffectsVolume);
        }
    }

    public void PlayCardSound(AudioClip cardSound)
    {
        effectsSource.PlayOneShot(cardSound, mainEffectsVolume);
    }
}
