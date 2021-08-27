using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRotation : MonoBehaviour
{
    public float RotationSpeed;

    private bool _rotate = false;

    private void Update()
    {
        if (_rotate)
        {
            transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime);
        }
    }

    public void SetRotation(bool state) { _rotate = state; }
}
