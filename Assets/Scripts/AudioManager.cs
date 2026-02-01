using UnityEngine;

[System.Serializable]
public class SFXGroup
{
    public AudioClip[] clips;   // multiple variants of the same SFX
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public float volume = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Groups")]
    public SFXGroup enemyDeathSFX;
    public SFXGroup playerHurtSFX;
    public SFXGroup score500SFX;
    public SFXGroup playerDeathSFX;

    [Header("Music")]
    public AudioClip musicClip;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        // music settings
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        musicSource.volume = 0.5f;
        musicSource.Play();
    }

    public void PlaySFX(SFXGroup sfxGroup)
    {
        if (sfxGroup == null) return;

        AudioClip clip = sfxGroup.GetRandomClip();
        if (clip == null) return;

        // Random pitch
        sfxSource.pitch = Random.Range(sfxGroup.minPitch, sfxGroup.maxPitch);
        sfxSource.PlayOneShot(clip, sfxGroup.volume);

        // Reset pitch for next sound
        sfxSource.pitch = 1f;
    }
}
