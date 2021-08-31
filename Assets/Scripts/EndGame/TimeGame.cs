using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utils;

public class TimeGame : Singleton<TimeGame>
{
    public int MaxMinutes = 10;
    public bool Debugging;
    private TextMeshProUGUI _text;
    private float _seconds;
    private int _minutes;
    private string _timeString;
    private bool _counting = true;
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _seconds = 0;
        _minutes = 0;
        if (Debugging) _minutes = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_counting) return;
        UpdateTime();
        UpdateText();

    }

    private void UpdateText()
    {
        if (_minutes < 10)
            _timeString = "0" + _minutes.ToString();
        else
            _timeString = _minutes.ToString();

        _timeString += ":";
        if (_seconds < 10 && _seconds.ToString("F0") != "10")
            _timeString += "0" + _seconds.ToString("F0");
        else
            _timeString += _seconds.ToString("F0");

        _text.text = _timeString;
    }

    private void UpdateTime()
    {
        _seconds += Time.deltaTime;
        if (_seconds > 60)
        {
            _minutes++;
            _seconds = 0;
        }
    }

    public bool OnTime()
    {
        _counting = false;

        if (_minutes >= MaxMinutes)
            return false;
        else
            return true;

    }
}
