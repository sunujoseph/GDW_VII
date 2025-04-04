using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI; 

public class SoundManager : MonoBehaviour
{
    //using a singleton
    public static SoundManager instance;

   
    [SerializeField] private AudioSource soundObject;
    [SerializeField] private Slider volumeSlider;
    private float volumeLevel = 1f; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
            volumeLevel = volumeSlider.value;

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
        audiosource.pitch += Random.Range(-0.075f, 0.075f);
        //play sound
        audiosource.Play();
        //get length of clip
        float clipLength = audiosource.clip.length;
        //destroy object at the end
        Destroy(audiosource.gameObject, clipLength);
    }

    private void UpdateVolume(float newVolume)
    {
        volumeLevel = newVolume; 
    }
}
