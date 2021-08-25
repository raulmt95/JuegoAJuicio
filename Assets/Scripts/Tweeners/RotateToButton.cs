using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class RotateToButton : MonoBehaviour, IPointerEnterHandler
{
    public GameObject ImageToRotate;
    public float RotationAngle;
    public float RotationSpeed;

    private Vector3 rotationVector;

    void Start()
    {
        rotationVector = new Vector3(0, 0, RotationAngle);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RotateImage();
    }

    private void RotateImage()
    {
        ImageToRotate.transform.DORotate(rotationVector, RotationSpeed).SetEase(Ease.Linear);
    }
}
