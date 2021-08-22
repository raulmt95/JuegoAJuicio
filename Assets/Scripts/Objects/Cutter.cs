using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter: MonoBehaviour
{
    public Rigidbody2D Player_rb;
    public float Cooldown = 1f;

    private bool _able = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Hair") && _able)
        {
            //_joint.distance = Vector2.Distance(transform.position, Player_rb.transform.position);
            collision.GetComponent<Hair>().CutHair(Vector2.Distance(transform.position, Player_rb.transform.position));
            _able = false;
            Invoke(nameof(Restore), Cooldown);
        }
    }

    private void Restore()
    {
        _able = true;
    }
}
