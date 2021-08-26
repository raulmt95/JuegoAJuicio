using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPlaces : MonoBehaviour
{
    public Rigidbody2D Player_rb;
    public bool Trap = false;

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
            HairRef = collision.GetComponent<Hair>();
            if (!HairRef.isHooked())
            {
                Hook();
            }
            else if(Trap)
            {
                HairRef.Unhook();
                Hook();
            }
            
        }
    }

    private void Hook()
    {
        _hooked = true;
        _joint.enabled = true;
        _joint.distance = HairRef.LockClosestPoint(Vector2.Distance(transform.position, Player_rb.transform.position));
        HairRef.HookRef(this);
    }

    private void Update()
    {
        if (HairRef)
            CheckDistanceHair();
    }

    public void Unhook()
    {
        if (HairRef != null)
        {
            _joint.enabled = false;
            _hooked = false;

            HairRef.ReleaseBlock();
            HairRef = null;
        }
    }

    private void CheckDistanceHair()
    {
        _joint.distance = HairRef.DistanceToHook();
    }
}
