using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //using a singleton
    public static SoundManager instance;

    [SerializeField] private AudioSource soundObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Play(AudioClip clip, Transform spawn, float volume)
    {
        //spawn sound in an object
        AudioSource audiosource = Instantiate(soundObject, spawn.position, Quaternion.identity);
        //assign clip
        audiosource.clip = clip;
        //assign volume
        audiosource.volume = volume;
        //variation on pitch
        audiosource.pitch += Random.Range(-0.15f, 0.15f);
        //play sound
        audiosource.Play();
        //get length of clip
        float clipLength = audiosource.clip.length;
        //destroy object at the end
        Destroy(audiosource.gameObject, clipLength);
    }
}
