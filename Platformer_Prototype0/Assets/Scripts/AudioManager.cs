using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [Header("BGM")]
    public AudioClip bgm1;
    [Header("Player Generated Audio Clips")]
    public AudioClip slashSound;
    public AudioClip bleedSound;
    public AudioClip upSpellSound;
    public AudioClip downSpellSound;
    public AudioClip sideSpellSound;
    public AudioClip dashSound;
    public AudioClip jumpSound;
    public AudioClip healSound;
    public AudioClip chimePickUpSound;
    public AudioClip popPickUpSound;
    [Header("Enemy basic Generated Audio Clips")]
    public AudioClip dmgedSound;
    public AudioClip spikeBounceSound;
    public AudioMixer audioMixer;

    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        //DontDestroyOnLoad(gameObject);

    }


    public void PlayCharSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip, GetVol(clip, 0));
    }

    public void PlayBaseMobSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip, SFXSource.volume * GetVol(clip, 1));
    }

    private float GetVol(AudioClip _clip, int i)
    {
        float vol = 1f;
        if (i == 0)
        {
            if (_clip == downSpellSound || _clip == sideSpellSound)
            {
                vol = 0.5f;
            }
            else if (_clip == bleedSound || _clip == healSound)
            {
                vol = 0.25f;
            }
            else if(_clip == upSpellSound || _clip == jumpSound)
            {
                vol = 0.1f;
            }
            else
            {
                vol = 1f;
            }
        }
        else if(i == 1)
        {
            if (_clip == spikeBounceSound)
            {
                vol = 0.05f;
            }
        }
          
               

        //Debug.Log(vol);
        return vol;
    }

}
