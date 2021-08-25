using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deleter : MonoBehaviour
{
    public float Lifetime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, Lifetime);
    }
}
