using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingTweener : MonoBehaviour
{
    public float MovimientoMax = 5f;

    void Start()
    {
        transform.DOLocalMoveY(transform.position.y - MovimientoMax, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Play();
    }
}
