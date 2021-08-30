using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.PlayButtonClickSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayButtonHoverSound();
    }
}
