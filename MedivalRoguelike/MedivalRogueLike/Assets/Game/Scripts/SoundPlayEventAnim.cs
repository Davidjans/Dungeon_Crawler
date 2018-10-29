using UnityEngine;
using System.Collections;

public class SoundPlayEventAnim : MonoBehaviour
{


    public AudioClip SoundToPlay;
    public AudioSource source;


    void Awake()
    {

        source = GetComponent<AudioSource>();
    }

    void playSound()
    {

        source.PlayOneShot(SoundToPlay);
    }

    void Update()
    {





    }
} 