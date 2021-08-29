using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HookPlaces : MonoBehaviour
{
    public Rigidbody2D Player_rb;
    public bool Trap = false;

    private DistanceJoint2D _joint;

    private bool _hooked = false;

    private Hair HairRef;

    //New more dynamic hook
    private Transform Child;
    private Animator _anim;

    private void Start()
    {
        _joint = GetComponent<DistanceJoint2D>();
        _anim = GetComponent<Animator>();

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
        if(_anim)
            _anim.SetTrigger("Hook");
    }

    private void Update()
    {
        if (HairRef)
        {
            CheckDistanceHair();
            //if (!Trap)
            //    MoveHook();
        }

    }

    public void Unhook()
    {
        if (HairRef != null)
        {
            _joint.enabled = false;
            _hooked = false;

            HairRef.ReleaseBlock();
            HairRef = null;

            if(_anim)
                _anim.SetTrigger("Unhook");

            //ResetHook();
        }
    }

    private void CheckDistanceHair()
    {
        _joint.distance = HairRef.DistanceToHook();
    }

    //private void MoveHook()
    //{
    //    Vector2 PlayerVector = (transform.position - Player_rb.gameObject.transform.position);
    //    float angle = Vector2.Angle(Vector2.up, PlayerVector);
    //    angle = PlayerVector.x < 0 ? angle : -angle;
    //    transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, angle);
    //}

    //private void ResetHook()
    //{
    //    transform.DORotate(Vector3.zero, 1f).Play();
    //}
}
