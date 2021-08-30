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
        _slider.value = 0.75f;

        SetValueOnStart();
    }

    void SetValueOnStart()
    {
        //Setear el valor del slider al volumen
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
