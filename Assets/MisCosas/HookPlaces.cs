using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPlaces : MonoBehaviour
{
    public Rigidbody2D Player_rb;

    private DistanceJoint2D _joint;

    private bool _hooked = false;

    private Rope HairRef;

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
            HairRef = collision.GetComponent<Rope>();
            _joint.distance = HairRef.LockClosestPoint(Vector2.Distance(transform.position, Player_rb.transform.position));
            
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
    }
}
