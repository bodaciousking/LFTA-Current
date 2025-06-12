using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class SoundPlayer : MonoBehaviour
{
    public List<AudioClip> sounds = new List<AudioClip>();
    public List<AudioClip> randomSounds = new List<AudioClip>();
    AudioSource mySource;

    private void Start()
    {
        mySource = GetComponent<AudioSource>();
    }

    public void PlayClip(int index)
    {
        //float pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        AudioManager.Instance.PlaySound(sounds[index], transform.position, 1, 1);
    }

    public void PlayRandomSound(string indicies)
    {
        if (!Regex.IsMatch(indicies, "[0-9]"))
            return;

        int randomNumber = UnityEngine.Random.Range(0, indicies.Length);
        char clipIndexChar = indicies[randomNumber];
        int clipIndex = int.Parse(clipIndexChar.ToString());
        mySource.clip = randomSounds[clipIndex];

        float pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        AudioManager.Instance.PlaySound(randomSounds[clipIndex], transform.position, 1, pitch);
        mySource.Play();
    }
}
