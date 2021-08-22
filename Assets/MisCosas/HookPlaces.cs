using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPlaces : MonoBehaviour
{
    public Rigidbody2D Player_rb;

    private DistanceJoint2D _joint;

    private bool _hooked = false;

    private Hair HairRef;

    private void Start()
    {
        _joint = GetComponent<DistanceJoint2D>();

        _joint.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Hair") && !_hooked)
        {
            _hooked = true;
            _joint.enabled = true;
            //_joint.distance = Vector2.Distance(transform.position, Player_rb.transform.position);
            HairRef = collision.GetComponent<Hair>();
            _joint.distance = HairRef.LockClosestPoint(Vector2.Distance(transform.position, Player_rb.transform.position));
            HairRef.HookRef(transform.position);
            
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && _hooked && HairRef != null)
        {
            _joint.enabled = false;
            _hooked = false;

            HairRef.ReleaseBlock();
            HairRef = null;
        }

        if (HairRef)
            CheckDistanceHair();
    }

    private void CheckDistanceHair()
    {
        _joint.distance = HairRef.DistanceToHook();
    }
}
