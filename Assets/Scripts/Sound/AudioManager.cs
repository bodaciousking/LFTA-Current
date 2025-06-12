using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Settings")]
    public int poolSize = 10;
    public bool use3DAudio = true;

    private List<AudioSource> audioPool = new List<AudioSource>();

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = new GameObject("PooledAudioSource_" + i);
            go.transform.parent = this.transform;

            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = use3DAudio ? 1f : 0f;

            go.SetActive(false);
            audioPool.Add(source);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();
        if (source != null)
        {
            GameObject go = source.gameObject;
            go.transform.position = position;
            go.SetActive(true);

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();

            StartCoroutine(DisableAfterPlay(source));
        }
        else
        {
            Debug.LogWarning("No available audio sources in pool!");
        }
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var source in audioPool)
        {
            if (!source.isPlaying)
                return source;
        }
        return null;
    }

    private IEnumerator DisableAfterPlay(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
    }
}