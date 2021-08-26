using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

public class AudioManager : Singleton<AudioManager>
{
    public float TransitionTime;

    [Header("Audio Sources")]
    public AudioSource MusicAudioSource;
    public AudioSource SFXAudioSource;

    [Header("Audio Mixers")]
    public AudioMixerGroup MenuMusic;
    public AudioMixerGroup GameMusic;

    [Header("Snapshots")]
    public AudioMixerSnapshot MenuSnap;
    public AudioMixerSnapshot GameSnap;

    [Header("Music Clips")]
    public AudioClip MenuClip;
    public AudioClip GameClip;

    [Header("Pitch SFX Variation")]
    public float MinPitch;
    public float MaxPitch;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void MenuTransition()
    {
        MusicAudioSource.outputAudioMixerGroup = MenuMusic;
        SetMusic(MenuClip);
        MenuSnap.TransitionTo(TransitionTime);
    }

    public void GameTransition()
    {
        MusicAudioSource.outputAudioMixerGroup = GameMusic;
        SetMusic(GameClip);
        GameSnap.TransitionTo(TransitionTime);
    }

    public void SetMusic(AudioClip clip)
    {
        MusicAudioSource.clip = clip;
        MusicAudioSource.Play();
    }

    public void PlayEffect(AudioClip clip)
    {
        SFXAudioSource.pitch = Random.Range(MinPitch, MaxPitch);
        SFXAudioSource.PlayOneShot(clip);
    }
}
