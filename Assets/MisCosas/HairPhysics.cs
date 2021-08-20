using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairPhysics : MonoBehaviour
{
    public float HairWidth = 0.2f;
    public int segmentLength = 15;
    public float VertexDistance = 0.25f;
    public int ConstrainPrecision = 60;

    private LineRenderer _lineRenderer;
    private List<HairSegment> _hairSegments = new List<HairSegment>();


    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Vector3 hairStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for(int i = 0; i < segmentLength; i++)
        {
            _hairSegments.Add(new HairSegment(hairStartPoint));
            hairStartPoint.y -= VertexDistance;
        }
    }

    void Update()
    {
        DrawHair();
    }

    private void FixedUpdate()
    {
        Simulation();
    }

    private void Simulation()
    {
        Vector2 _gravity = new Vector2(0f, -1.5f);

        //SIMULATION
        for (int i = 1; i < segmentLength; i++)
        {
            HairSegment firstSegment = _hairSegments[i];
            Vector2 Velocity = firstSegment.posNow - firstSegment.oldPos;
            firstSegment.oldPos = firstSegment.posNow;
            firstSegment.posNow += Velocity;
            firstSegment.posNow += _gravity * Time.deltaTime;
            _hairSegments[i] = firstSegment;
        }

        //CONSTRAINS
        for (int i = 0; i < ConstrainPrecision; i++)
        {
            ApplyConstrains();
        }
    }

    private void ApplyConstrains()
    {
        HairSegment firstSegment = _hairSegments[0];
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _hairSegments[0] = firstSegment;
        for (int i = 0; i < segmentLength - 1; i++)
        {
            HairSegment firstSeg = _hairSegments[i];
            HairSegment secondSeg = _hairSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = dist - VertexDistance;
            Vector2 ChangeDir = (firstSeg.posNow - secondSeg.posNow).normalized;

            //if(dist > _hairSegLen)
            //    ChangeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            //else if(dist < _hairSegLen)
            //    ChangeDir = (secondSeg.posNow - firstSeg.posNow).normalized;

            Vector2 ChangeAmmount = ChangeDir * error;

            if(i != 0)
            {
                firstSeg.posNow -= ChangeAmmount * 0.5f;
                _hairSegments[i] = firstSeg;
                secondSeg.posNow = ChangeAmmount * 0.5f;
                _hairSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += ChangeAmmount;
                _hairSegments[i + 1] = secondSeg;
            }
        }

    }

    private void DrawHair()
    {
        float lineWidth = HairWidth;
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth * 0.8f;

        Vector3[] hairPositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            hairPositions[i] = _hairSegments[i].posNow;

        }

        _lineRenderer.positionCount = hairPositions.Length;
        _lineRenderer.SetPositions(hairPositions);
    }

    public struct HairSegment
    {
        public Vector2 posNow;
        public Vector2 oldPos;

        public HairSegment(Vector2 pos)
        {
            posNow = pos;
            oldPos = pos;
        }
    }
}
