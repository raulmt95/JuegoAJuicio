using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deleter : MonoBehaviour
{
    public float Lifetime = 0.5f;
    void Start()
    {
        Destroy(gameObject, Lifetime);
    }
}
