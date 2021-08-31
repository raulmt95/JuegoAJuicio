using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    public float DragLimit;
    public GameObject SoundMarker;
    public bool IsMusic;
    
    public event Action<Quaternion> OnAngleChanged;
 
    Quaternion dragStartRotation;
    Quaternion dragStartInverseRotation;

    private SetVolume volumeSetter;
 
    private void Awake()
    {
        OnAngleChanged += (rotation) => transform.localRotation = rotation;
    }

    void Start()
    {
        volumeSetter = GetComponent<SetVolume>();
        //volumeSetter.SetVolumeLevel(CalculateRotationLevel(transform.localRotation.eulerAngles.z), IsMusic);
    }

    void Update()
    {
        if(transform.localRotation.eulerAngles.z > DragLimit && transform.localRotation.eulerAngles.z < 360 - DragLimit)
        {
            float aux = transform.localRotation.eulerAngles.z;
            aux = 360 - aux;
            if(aux > 360 / 2) { transform.localRotation = Quaternion.Euler(new Vector3(0, 0, DragLimit)); }
            else { transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -DragLimit)); }
        }

        SoundMarker.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, transform.localRotation.eulerAngles.z * -1));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragStartRotation = transform.localRotation;
        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint))
        {
            dragStartInverseRotation = Quaternion.Inverse(Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward));
        }
        else
        {
            Debug.LogWarning("Couldn't get drag start world point");
        }
    }
 
    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        volumeSetter.SetVolumeLevel(CalculateRotationLevel(transform.localRotation.eulerAngles.z), IsMusic);
    }
 
    float CalculateRotationLevel(float zRotation)
    {
        zRotation += DragLimit;

        if (zRotation == 360f) { return 0.0001f; }
        if (zRotation > DragLimit * 2) { zRotation = (360 - zRotation) * -1; }

        return zRotation / (DragLimit * 2);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint))
        {
            Quaternion currentDragAngle = Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward);
            if (OnAngleChanged != null)
            {
                OnAngleChanged(currentDragAngle * dragStartInverseRotation * dragStartRotation);
            }
        }
    }

    private bool DragWorldPoint(PointerEventData eventData, out Vector3 worldPoint)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out worldPoint);
    }
}