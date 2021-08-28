using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxRelative : MonoBehaviour
{
    [Range(0,1)]
    public float ParallaxRatio = 0f;

    public bool FollowY = false;

    private Camera _cam;
    private Vector2 _camPos;
    void Start()
    {
        _cam = GetComponentInParent<Camera>();
        _camPos = _cam.transform.position;
    }

    void Update()
    {
        float y_Value;

        if (FollowY)
            y_Value = _cam.transform.position.y - _camPos.y;
        else
            y_Value = 0f;

        Vector2 Desplazamiento = new Vector2((_cam.transform.position.x - _camPos.x) * ParallaxRatio, y_Value);

        Desplazamiento *= -1f;

        transform.Translate(Desplazamiento);

        _camPos = _cam.transform.position;
    }

}
