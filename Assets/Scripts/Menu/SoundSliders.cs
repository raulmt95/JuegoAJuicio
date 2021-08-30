using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSliders : MonoBehaviour
{
    public AudioMixer Mixer;
    public bool IsMusic;
    public TMP_Text VolumeText;

    private Slider _slider;


    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        SetValueOnStart();
    }

    void SetValueOnStart()
    {
        float value;

        if (IsMusic && Mixer.GetFloat("MusicVol", out value)) { _slider.value = ((Mathf.Pow(10, value / 20))); }
        if (!IsMusic && Mixer.GetFloat("SFXVol", out value)) { _slider.value = ((Mathf.Pow(10, value / 20))); }
    }

    void SetVolumeLevel(float value, bool isMusic)
    {
        if (isMusic) { Mixer.SetFloat("MusicVol", Mathf.Log10(value) * 20); }
        else { Mixer.SetFloat("SFXVol", Mathf.Log10(value) * 20); }
    }

    public void SetVolumePercentage()
    {
        int volume = (int)((_slider.value / _slider.maxValue) * 100);
        VolumeText.text = volume.ToString() + "%";

        SetVolumeLevel(_slider.value, IsMusic);
    }
}
