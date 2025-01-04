using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FabrikPlaneSolver : MonoBehaviour
{
    [SerializeField] Transform[] bones;

    [SerializeField] int iterations;
    [SerializeField] float delta;

    private float[] _bonesLenght;

    private Vector3[] _previousPassPositions;
    private Vector3[] _passPositions;

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        DrawBones(bones.Select(x => x.position), Color.white);
        for(int i = 0; i < iterations; i++)
        {
            _previousPassPositions = _passPositions;
            BackwardsPass();
            DrawBones(_passPositions.Skip(1), Color.green);
            _previousPassPositions = _passPositions;
            ForwardPass();
            DrawBones(_passPositions.SkipLast(1), Color.magenta);
            /*if (Vector3.Distance(_passPositions[^2], transform.position) <= delta)
                break;*/
        }
    }

    private void Initialize()
    {
        _bonesLenght = new float[bones.Length - 1];
        _passPositions = new Vector3[bones.Length + 1];

        for (int i = 0; i < bones.Length - 1; i++)
        {
            _bonesLenght[i] = Vector3.Distance(bones[i].position, bones[i + 1].position);
        }
    }

    private void BackwardsPass()
    {
        _passPositions[^1] = transform.position;

        for (int i = bones.Length - 1; i > 0; i--)
        {
            _passPositions[i] = _previousPassPositions[i + 1] +
                (bones[i-1].position - _previousPassPositions[i + 1]).normalized * _bonesLenght[i - 1];
        }
    }

    private void ForwardPass()
    {
        _passPositions[^1] = transform.position;
        _passPositions[0] = bones[0].position;
        
        for(int i = 0; i < bones.Length - 1; i++)
        {
            _passPositions[i + 1] = bones[i].position +
                (_previousPassPositions[i + 1] - bones[i].position).normalized * _bonesLenght[i];
        }
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
        Debug.DrawLine(new Vector3(pos.x, pos.y + w, pos.z), new Vector3(pos.x, pos.y - w, pos.z), color, 0.05f, false);
    }
}
