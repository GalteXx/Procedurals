using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FabrikPlaneSolver
{
    private readonly Transform[] _bones;
    private readonly Transform _mainBody;
    private readonly Transform _target;
    private readonly int _iterations;
    private readonly float _delta;


    private float[] _bonesLenght;

    private Vector3[] _previousPassPositions;
    private Vector3[] _passPositions;

    private Vector3 Target
    {
        get
        {
            return Quaternion.AngleAxis(
                Vector3.Angle(_target.position - _bones[0].position, _mainBody.up),
                _bones[0].right) * 
                _mainBody.up * 
                Vector3.Distance(_target.position, _bones[0].position);
        }
    }

    public FabrikPlaneSolver(Transform[] bones, Transform target, Transform mainBody, int iterations, float delta)
    {
        _bones = bones;
        _iterations = iterations;
        _target = target;
        _delta = delta;
        _mainBody = mainBody;

        Initialize();
    }

    public void OnUpdate()
    {   
        SolveIK();
    }

    public Vector3[] SolveIK()
    {
        _passPositions = _bones.Select(x => x.position).ToArray();
        for (int i = 0; i < _iterations; i++)
        {
            BackwardsPass();
            ForwardPass();
            if (Vector3.Distance(_passPositions[^1], Target) <= _delta)
                break;
        }
        return _passPositions;
    }

    private void Initialize()
    {
        _bonesLenght = new float[_bones.Length - 1];
        _passPositions = new Vector3[_bones.Length];

        for (int i = 0; i < _bones.Length - 1; i++)
        {
            _bonesLenght[i] = Vector3.Distance(_bones[i].position, _bones[i + 1].position);
        }
    }

    private void BackwardsPass()
    {
        _previousPassPositions = _passPositions;

        _passPositions[^1] = Target;
        for (int i = _bones.Length - 2; i >= 0; i--)
        {
                _passPositions[i] = _passPositions[i + 1] +
                    (_previousPassPositions[i] - _passPositions[i + 1]).normalized * _bonesLenght[i];
        }
    }

    private void ForwardPass()
    {
        _previousPassPositions = _passPositions;

        _passPositions[0] = _bones[0].position;
        for(int i = 1; i < _bones.Length; i++)
        {
            _passPositions[i] = _passPositions[i - 1] +
                (_previousPassPositions[i] - _passPositions[i - 1]).normalized * _bonesLenght[i - 1];
        }
    }
}
