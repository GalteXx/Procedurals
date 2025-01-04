using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpiderAnimator : MonoBehaviour
{
    [SerializeField] private float[] angles;
    [Header("References")]
    [SerializeField] private Transform[] legRoots;
    [SerializeField] private Transform[] legEnds;
    void Start()
    {
        if (legEnds.Length != legRoots.Length || legRoots.Length != angles.Length * 2)
            throw new System.Exception("Incorrect configurations");
        for(int i = 0; i < legEnds.Length; i++)
        {
            var ik = this.AddComponent<ChainIKConstraint>();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
