using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    public AudioClip weaponFire;
    public AudioClip barrierHit;
    public AudioClip aiDeath;
    public AudioClip backgroundMusic;
    public AudioClip aiCompleted;

    [Range(0f, 1f)] public float musicVolume = 0.3f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Music AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // SFX AudioSource
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
    }

    void Start()
    {
        if (backgroundMusic != null)
            PlayMusic(backgroundMusic);
    }

    public void PlayWeaponFire(AudioClip clip) => PlaySound(clip, 1f);
    public void PlayBarrierHit(AudioClip clip) => PlaySound(clip, 0.6f);
    public void PlayAIDeath(AudioClip clip) => PlaySound(clip, 0.5f);

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayAICompleted(AudioClip clip) => PlaySound(clip, 0.8f);

    void PlaySound(AudioClip clip, float volume)
    {
        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(clip, volume);
    }
}