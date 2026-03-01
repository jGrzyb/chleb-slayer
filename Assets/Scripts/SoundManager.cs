using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I;

    [Header("Music")]
    public AudioSource MenuMusic;
    public AudioSource LevelMusic;
    public AudioSource ShopMusic;
    public AudioSource NewsMusic;

    [Header("Exclusive SFX (one at a time)")]
    public AudioSource PlayerHurtSFX;
    public AudioSource PlayerAttackSFX;

    [Header("Shoot Pool")]
    public int shootPoolSize = 3;
    public AudioClip ShootClip;

    [Header("Enemy Hurt Pool")]
    public int enemyHurtPoolSize = 3;
    public AudioClip EnemyHurtClip;

    [Header("Enemy Death Pool")]
    public int enemyDeathPoolSize = 3;
    public AudioClip EnemyDeathClip;

    [Header("Generic Overlapping SFX Pool")]
    public int overlapPoolSize = 6;
    public AudioClip ItemPickupClip;
    public AudioClip PlaceTowerClip;

    [Header("Settings")]
    public bool stealOldest = true;
    [Range(0f, 0.1f)]
    public float overlapJitterMax = 0.05f;

    private AudioSource[] _allMusic;

    private AudioSource[] _shootPool;
    private int _shootIndex = 0;

    private AudioSource[] _enemyHurtPool;
    private int _enemyHurtIndex = 0;

    private AudioSource[] _enemyDeathPool;
    private int _enemyDeathIndex = 0;

    private AudioSource[] _overlapPool;
    private int _overlapIndex = 0;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        _allMusic = new[] { MenuMusic, LevelMusic, ShopMusic, NewsMusic };

        _shootPool      = BuildPool(shootPoolSize);
        _enemyHurtPool  = BuildPool(enemyHurtPoolSize);
        _enemyDeathPool = BuildPool(enemyDeathPoolSize);
        _overlapPool    = BuildPool(overlapPoolSize);
    }

    private AudioSource[] BuildPool(int size)
    {
        var pool = new AudioSource[size];
        for (int i = 0; i < size; i++)
        {
            pool[i] = gameObject.AddComponent<AudioSource>();
            pool[i].playOnAwake = false;
        }
        return pool;
    }

    public void PlayMusic(AudioSource music)
    {
        foreach (var m in _allMusic)
            if (m != null && m != music) m.Stop();
        if (music != null && !music.isPlaying) music.Play();
    }

    public void StopAllMusic()
    {
        foreach (var m in _allMusic) m?.Stop();
    }

    public void PlayExclusive(AudioSource sfx)
    {
        if (sfx == null) return;
        sfx.Stop();
        sfx.Play();
    }

    public void PlayShoot(AudioClip clip, float volume = 1f)      => PlayFromPool(clip, volume, _shootPool, ref _shootIndex);
    public void PlayEnemyHurt(AudioClip clip, float volume = 1f)  => PlayFromPool(clip, volume, _enemyHurtPool, ref _enemyHurtIndex);
    public void PlayEnemyDeath(AudioClip clip, float volume = 1f) => PlayFromPool(clip, volume, _enemyDeathPool, ref _enemyDeathIndex);

    public void PlayOverlap(AudioClip clip, float volume = 1f)    => PlayFromPool(clip, volume, _overlapPool, ref _overlapIndex);

    private void PlayFromPool(AudioClip clip, float volume, AudioSource[] pool, ref int index)
    {
        if (clip == null) return;
        AudioSource source = GetPoolSource(pool, ref index);
        if (source == null) return;
        StartCoroutine(PlayDelayed(source, clip, volume, Random.Range(0f, overlapJitterMax)));
    }

    private AudioSource GetPoolSource(AudioSource[] pool, ref int index)
    {
        foreach (var s in pool)
            if (!s.isPlaying) return s;

        if (!stealOldest) return null;

        AudioSource oldest = pool[index];
        index = (index + 1) % pool.Length;
        oldest.Stop();
        return oldest;
    }

    private IEnumerator PlayDelayed(AudioSource source, AudioClip clip, float volume, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }
}