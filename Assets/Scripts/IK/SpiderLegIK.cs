using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpiderLegIK : MonoBehaviour
{
    [Header("TargetBones")]
    [SerializeField] Transform rootBone;
    [SerializeField] Transform leafBone;
    [Header("IK parameters")]
    [SerializeField] float delta;
    [SerializeField] int iterations;

    private FabrikPlaneSolver _lastSegments;
    private Transform[] _bones;
    

    private void Awake()
    {
        _lastSegments = new(_bones, transform, iterations, delta);
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
