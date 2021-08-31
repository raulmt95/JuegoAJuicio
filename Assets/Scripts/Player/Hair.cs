using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hair : MonoBehaviour
{
    [Header("Paramenters")]
    public float hairSegLen = 0.25f;
    public int segmentLength = 35;
    public float lineWidth = 0.1f;
    public float GravityForce = 1f;
    public float TimeDrop = 0.1f;
    public int Iterations = 30;
    public float GrowTime = 0.1f;
    [Range(0, 4)]
    public float DeathSpeed = 2f;

    [Header("References")]
    public Transform StartHair;
    public PlayerController Player;

    [Header("Button")]
    public KeyCode GrowHairButton = KeyCode.Mouse1;

    [Header("VFX")]
    public GameObject PeloPS;
    public GameObject MuchosPelosPS;

    [Header("Others")]

    private float _Grownth = 0.001f;
    private float _timer;

    private LineRenderer lineRenderer;
    private List<RopeSegment> hairSegments = new List<RopeSegment>();

    private int _blockPointIndex = 0; // 0 is dropped, any other value is hooked
    private Vector2 HookPos;

    private EdgeCollider2D _edgeCol;

    public bool _trapped;
    private HookPlaces _currentHook;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        _edgeCol = GetComponent<EdgeCollider2D>();
        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < segmentLength; i++)
        {
            hairSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= hairSegLen;
        }

        InvokeRepeating(nameof(DropHairHooked), 0f, TimeDrop);
    }

    // Update is called once per frame
    void Update()
    {
        DrawHair();

        if(Input.GetKey(GrowHairButton) && !_trapped)
            GrowHair();

        if (_trapped)
        {
            ReduceHair();
        }
    }

    private void GrowHair()
    {
        _timer += Time.deltaTime;
        if (_timer > GrowTime)
        {
            _timer = 0f;
            hairSegLen += _Grownth;
        }
    }

    private void ReduceHair()
    {
        _timer += Time.deltaTime;
        if (_timer > DeathSpeed)
        {
            _timer = 0f;
            hairSegLen -= _Grownth;
            if (hairSegLen <= 0)
            {
                hairSegLen = 0;
                Player.UnhookPlayer();
            }
        }
    }
    private void FixedUpdate()
    {
        Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector2 forceGravity = new Vector2(0f, GravityForce);

        for (int i = 1; i < segmentLength; i++)
        {
            RopeSegment firstSegment = hairSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            if(i != _blockPointIndex || _blockPointIndex == 0)
            { 
                firstSegment.posNow += velocity;
                firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            }
            hairSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < Iterations; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        CheckMaxLength();
        if(_blockPointIndex > 1)
            CheckBlockLength();

        for (int i = 0; i < segmentLength - 1; i++)
        {
            RopeSegment firstSeg = hairSegments[i];
            RopeSegment secondSeg = hairSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - hairSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > hairSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < hairSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                if (i + 1 == segmentLength - 1)
                {
                    firstSeg.posNow -= changeAmount;
                    hairSegments[i] = firstSeg;
                }
                else if (i == _blockPointIndex)
                {
                    secondSeg.posNow += changeAmount;
                    hairSegments[i + 1] = secondSeg;
                }
                else if (i + 1 == _blockPointIndex)
                {
                    firstSeg.posNow -= changeAmount;
                    hairSegments[i] = firstSeg;
                }
                else
                {
                    firstSeg.posNow -= changeAmount * 0.5f;
                    hairSegments[i] = firstSeg;
                    secondSeg.posNow += changeAmount * 0.5f;
                    hairSegments[i + 1] = secondSeg;
                }
            }
            else
            {
                secondSeg.posNow += changeAmount;
                hairSegments[i + 1] = secondSeg;
            }
        }

        if (_blockPointIndex != 0)
        {
            RopeSegment hookedSegment = hairSegments[_blockPointIndex];
            hookedSegment.posNow = HookPos;
            hairSegments[_blockPointIndex] = hookedSegment;
        }

        RopeSegment lastSegment = hairSegments[segmentLength - 1];
        lastSegment.posNow = StartHair.position;
        hairSegments[segmentLength - 1] = lastSegment;

    }

    private void CheckBlockLength()
    {
        RopeSegment firstSegment = hairSegments[0];
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RopeSegment blockSegment = hairSegments[_blockPointIndex];

        if ((blockSegment.posNow - firstSegment.posNow).magnitude > _blockPointIndex * hairSegLen)
            firstSegment.posNow = blockSegment.posNow - (blockSegment.posNow - firstSegment.posNow).normalized * _blockPointIndex * hairSegLen;

        hairSegments[0] = firstSegment;
        hairSegments[_blockPointIndex] = blockSegment;
    }

    private void CheckMaxLength()
    {
        RopeSegment firstSegment = hairSegments[0];
        if(_blockPointIndex == 0)
            firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RopeSegment lastSegment = hairSegments[segmentLength - 1];
        lastSegment.posNow = StartHair.position;

        if ((lastSegment.posNow - firstSegment.posNow).magnitude > segmentLength * hairSegLen)
            firstSegment.posNow = lastSegment.posNow - (lastSegment.posNow - firstSegment.posNow).normalized * segmentLength * hairSegLen;

        hairSegments[0] = firstSegment;
        hairSegments[segmentLength - 1] = lastSegment;
    }

    private void DrawCollider()
    {
        Vector2[] points = new Vector2[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            points[i] = hairSegments[i].posNow;
        }
        _edgeCol.points = points;
    }
    private void DrawHair()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = 0.4f * lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            ropePositions[i] = hairSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);

        DrawCollider();
    }

    #region HOOKS
    public float LockClosestPoint(float distance)
    {
        _blockPointIndex = 1;
        for (int i = 1; i < segmentLength; i++)
        {
            if (Vector2.Distance(hairSegments[i].posNow, Player.transform.position) < distance)
            {
                _blockPointIndex = i;
                break;
            }
        }

        Player.HookPlayer();

        return (segmentLength - _blockPointIndex) * hairSegLen;
    }

    public void HookRef(HookPlaces hook)
    {
        HookPos = hook.transform.position;
        _currentHook = hook;
        Instantiate(PeloPS, HookPos, Quaternion.identity);
    }

    public void ReleaseBlock()
    {
        _blockPointIndex = 0;
    }
    public float DistanceToHook()
    {
        return (segmentLength - _blockPointIndex + 2) * hairSegLen;
    }
    private void DropHairHooked()
    {
        if (_blockPointIndex > 2)
            _blockPointIndex--;
    }

    #endregion

    public void CutHair(float distance)
    {
        AudioManager.Instance.PlayHairCutSound();

        float newLength = 0.1f;
        int index = 0;
        for (int i = 1; i < segmentLength; i++)
        {
            if (Vector2.Distance(hairSegments[i].posNow, Player.transform.position) < distance)
            {
                newLength = (segmentLength - i) * hairSegLen;
                index = i;
                break;
            }
        }
        float newSegmentLenght = newLength / segmentLength;
        hairSegLen = newSegmentLenght;

        Instantiate(MuchosPelosPS, hairSegments[index].posNow, Quaternion.identity);
        Player.GetComponent<PlayerController>().UnhookPlayer();
        ReleaseHair();
    }

    public void TrapHair(bool state)
    {
        _trapped = state;
    }

    void Die()
    {
        Player.Die();
        ReleaseHair();
    }

    public void EnableCollider()
    {
        _edgeCol.enabled = true;
    }

    public void DisableCollider()
    {
        _edgeCol.enabled = false;
    }

    public void Unhook()
    {
        if (_currentHook != null)
        {
            _currentHook.Unhook();
            _currentHook = null;
            ReleaseBlock();
        }
    }

    private void ReleaseHair()
    {
        Unhook();
        TrapHair(false);
    }

    public bool isHooked()
    {
        if (_blockPointIndex == 0 && _currentHook == null)
            return false;
        else
            return true;
    }
    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}