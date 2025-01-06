using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using UnityEngine.XR;
using System;
using System.Linq;

public class SpiderLegIK : MonoBehaviour
{
    [SerializeField] int chainLength;
    [SerializeField] Transform leafBone;

    [SerializeField] int iterations;
    [SerializeField] float delta;
    
    //TO be replaced
    private Transform Target;


    private Transform[] _bones;
    private Transform _rootBone;
    private Vector3[] _positions;
    private float[] _boneLenght;
    private Vector3[] _startDirectionSucc;
    private float CompleteLength;


    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        
    }

    void Initialize()
    {
        //initial array
        _bones = new Transform[chainLength + 1];
        _positions = new Vector3[chainLength + 1];
        _boneLenght = new float[chainLength];
        _startDirectionSucc = new Vector3[chainLength + 1];

        Target = transform;

        var current = leafBone;
        CompleteLength = 0;
        for(int i = 0; i < chainLength - 1; i++)
        {
            current = current.parent;
            _rootBone = current;
        }
        current = leafBone;
        for (var i = 0; i < chainLength; i++)
        {
            _bones[i] = current;

            if (i == 0)
            {
                _startDirectionSucc[i] = PositionInRootSpace(Target) - PositionInRootSpace(current);
            }
            else
            {
                _startDirectionSucc[i] = PositionInRootSpace(_bones[i + 1]) - PositionInRootSpace(current);
                _boneLenght[i] = _startDirectionSucc[i].magnitude;
                CompleteLength += _boneLenght[i];
            }
            if(i == chainLength - 1)
                _rootBone = current;

            current = current.parent;
        }


    }


    private void ResolveIK()
    {
        for (int i = 0; i < _bones.Length; i++)
            _positions[i] = PositionInRootSpace(_bones[i]);

        var targetPosition = PositionInRootSpace(Target);


        if ((targetPosition - PositionInRootSpace(_bones[0])).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            throw new NotImplementedException("Unable To Reach Behaviour Undefined");
        }
        else
        {
            for (int i = 0; i < _positions.Length - 1; i++)
                _positions[i + 1] = Vector3.Lerp(_positions[i + 1], _positions[i] + _startDirectionSucc[i], 1);

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = _positions.Length - 1; i > 0; i--)
                {
                    if (i == _positions.Length - 1)
                        _positions[i] = targetPosition; 
                    else
                        _positions[i] = _positions[i + 1] + (_positions[i] - _positions[i + 1]).normalized * _boneLenght[i]; //set in line on distance
                }

                //forward
                for (int i = 1; i < _positions.Length; i++)
                    _positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _boneLenght[i - 1];

                //close enough?
                if ((_positions[_positions.Length - 1] - targetPosition).sqrMagnitude < delta * delta)
                    break;
            }
        }

        //set position & rotation
        for (int i = 0; i < _positions.Length; i++)
        {
            SetPositionInRootSpace(_bones[i], _positions[i]);
        }
    }

    private Vector3 PositionInRootSpace(Transform current)
    {
        return Quaternion.Inverse(_rootBone.rotation) * (current.position - _rootBone.position);
    }

    private void SetPositionInRootSpace(Transform current, Vector3 position)
    {
        if (_rootBone == null)
            current.position = position;
        else
            current.position = _rootBone.rotation * position + _rootBone.position;
    }


    private void DrawBones(IEnumerable<Vector3> positions, Color col)
    {
        DrawCross(positions.ElementAt(0), Color.red, 0.05f);
        for (int i = 0; i < positions.Count() - 1; i++)
        {
            if (positions.ElementAt(i + 1) == null)
                Debug.Log(i + 1);
            Debug.DrawLine(positions.ElementAt(i), positions.ElementAt(i + 1), col, 0.01f, false);
            DrawCross(positions.ElementAt(i + 1), Color.red, 0.05f);
        }

    }
    private void DrawCross(Vector3 pos, Color color, float w = 0.1f)
    {
        Debug.DrawLine(new Vector3(pos.x - w, pos.y, pos.z + w), new Vector3(pos.x + w, pos.y, pos.z - w), color, 0.05f, false);
        Debug.DrawLine(new Vector3(pos.x + w, pos.y, pos.z + w), new Vector3(pos.x - w, pos.y, pos.z - w), color, 0.05f, false);
        Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z + w), new Vector3(pos.x, pos.y, pos.z - w), color, 0.05f, false);
        Debug.DrawLine(new Vector3(pos.x + w, pos.y, pos.z), new Vector3(pos.x - w, pos.y, pos.z), color, 0.05f, false);

    }
}
