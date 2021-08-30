using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoundSliders : MonoBehaviour
{
    public TMP_Text VolumeText;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetVolumePercentage()
    {
        int volume = (int)((_slider.value / _slider.maxValue) * 100);
        VolumeText.text = volume.ToString() + "%";
    }
}
