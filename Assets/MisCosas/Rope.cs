using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public float ropeSegLen = 0.25f;
    public int segmentLength = 35;
    public float lineWidth = 0.1f;
    public float GravityForce = 1f;
    public Transform StartHair;
    public GameObject Player;

    private float _timeToGrow = 0.1f;
    private float _Grownth = 0.001f;
    private float _timer;

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();

    private int _blockPointIndex;

    private EdgeCollider2D _edgeCol;


    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        _edgeCol = GetComponent<EdgeCollider2D>();
        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRope();

        if(Input.GetKey(KeyCode.R))
        {
            _timer += Time.deltaTime;
            if(_timer > _timeToGrow)
            {
                _timer = 0f;
                ropeSegLen += _Grownth;
            }
        }
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector2 forceGravity = new Vector2(0f, GravityForce);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            if(i != _blockPointIndex || _blockPointIndex == 0)
            { 
                firstSegment.posNow += velocity;
                firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            }
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        CheckMaxLength();
        if(_blockPointIndex != 0)
            CheckBlockLength();

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                if (i + 1 == segmentLength - 1)
                {
                    firstSeg.posNow -= changeAmount;
                    ropeSegments[i] = firstSeg;
                }
                else if (i == _blockPointIndex)
                {
                    secondSeg.posNow += changeAmount;
                    this.ropeSegments[i + 1] = secondSeg;
                }
                else if (i + 1 == _blockPointIndex)
                {
                    firstSeg.posNow -= changeAmount;
                    ropeSegments[i] = firstSeg;
                }
                else
                {
                    firstSeg.posNow -= changeAmount * 0.5f;
                    this.ropeSegments[i] = firstSeg;
                    secondSeg.posNow += changeAmount * 0.5f;
                    this.ropeSegments[i + 1] = secondSeg;
                }
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }


    }

    private void CheckBlockLength()
    {
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RopeSegment blockSegment = ropeSegments[_blockPointIndex];

        if ((blockSegment.posNow - firstSegment.posNow).magnitude > _blockPointIndex * ropeSegLen)
            firstSegment.posNow = blockSegment.posNow - (blockSegment.posNow - firstSegment.posNow).normalized * _blockPointIndex * ropeSegLen;

        this.ropeSegments[0] = firstSegment;
        this.ropeSegments[_blockPointIndex] = blockSegment;
    }

    private void CheckMaxLength()
    {
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RopeSegment lastSegment = ropeSegments[segmentLength - 1];
        lastSegment.posNow = StartHair.position;

        if ((lastSegment.posNow - firstSegment.posNow).magnitude > segmentLength * ropeSegLen)
            firstSegment.posNow = lastSegment.posNow - (lastSegment.posNow - firstSegment.posNow).normalized * segmentLength * ropeSegLen;

        this.ropeSegments[0] = firstSegment;
        this.ropeSegments[segmentLength - 1] = lastSegment;
    }

    private void DrawCollider()
    {
        Vector2[] points = new Vector2[segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            points[i] = ropeSegments[i].posNow;
        }
        _edgeCol.points = points;
    }
    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);

        DrawCollider();
    }

    public float LockClosestPoint(float distance)
    {
        _blockPointIndex = 0;
        for (int i = 0; i < segmentLength; i++)
        {
            if (Vector2.Distance(ropeSegments[i].posNow, Player.transform.position) < distance)
            {
                _blockPointIndex = i;
                Debug.Log(_blockPointIndex);
                break;
            }
        }

        return (segmentLength - _blockPointIndex + 2) * ropeSegLen;
    }

    public void ReleaseBlock()
    {
        _blockPointIndex = 0;
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}