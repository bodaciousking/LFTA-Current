using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

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
        mySource.clip = sounds[index];
        mySource.Play();
       // Debug.Log(gameObject.name + " playing sound: " + mySource.clip.name);
    }

    public void PlayRandomSound(string indicies)
    {
        if (!Regex.IsMatch(indicies, "[0-9]"))
            return;

        int randomNumber = Random.Range(0, indicies.Length);
        char clipIndexChar = indicies[randomNumber];
        int clipIndex = int.Parse(clipIndexChar.ToString());
        mySource.clip = randomSounds[clipIndex];
        mySource.Play();
        //Debug.Log(gameObject.name + " playing random sound: " + mySource.clip.name);
    }
}
