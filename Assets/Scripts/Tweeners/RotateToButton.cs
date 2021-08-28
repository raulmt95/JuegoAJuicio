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
    public float RotationTime;

    public List<GearRotation> GearsList;

    private Vector3 rotationVector;

    private bool _isRotating;

    void Start()
    {
        rotationVector = new Vector3(0, 0, RotationAngle);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.KillGearTween();

        RotateImage();
    }

    private void RotateImage()
    {
        if(ImageToRotate.transform.localRotation.eulerAngles != rotationVector)
        {
            Tween myTween = ImageToRotate.transform.DORotate(rotationVector, RotationTime).SetEase(Ease.Linear).OnComplete(StopGear);
            MenuManager.Instance.SetGeatTween(myTween);

            RotateGear();
        }
    }

    private void RotateGear()
    {
        foreach(GearRotation gear in GearsList)
        {
            gear.SetRotation(true);
        }
    }

    private void StopGear()
    {
        foreach (GearRotation gear in GearsList)
        {
            gear.SetRotation(false);
        }
    }
}
