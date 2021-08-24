using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitButtonPrank : MonoBehaviour, IPointerEnterHandler
{
    public float NewScale;
    public float NewXPos;
    public float Duration;

    private bool _isSmall;
    private bool _hasMoved;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void DoScale()
    {
        if (!_isSmall)
        {
            transform.DOScale(NewScale, Duration);
            _isSmall = true;
        }
        else if(!_hasMoved)
        {
            transform.DOMoveX(NewXPos, Duration);
            _hasMoved = true;
            _button.interactable = true;
        }
        else
        {
            transform.DOShakePosition(250);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DoScale();
    }
}
