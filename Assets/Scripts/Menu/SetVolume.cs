using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer Mixer;

    public void SetVolumeLevel(float value, bool isMusic)
    {
        if (isMusic) { Mixer.SetFloat("MusicVol", Mathf.Log10(value) * 20); }
        else { Mixer.SetFloat("SFXVol", Mathf.Log10(value) * 20); }
    }
}
