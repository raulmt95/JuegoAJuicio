using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutterShake : MonoBehaviour
{
    public float duration;
    public float strength;
    public int vibrato;

    private void Start()
    {
        transform.DOShakePosition(duration, strength, vibrato, 90, false, false)
                 .SetLoops(-1)
                 .Play();
    }
}
