using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InverseKinematicsController : MonoBehaviour
{
    [SerializeField] private InverseKinematicsDescriptor Solver = null;

    [SerializeField] private List<Joint> joints = new List<Joint>();
    [SerializeField] private List<Constraint> constraints = new List<Constraint>();

    [SerializeField] private Transform baseBone = null;

    [SerializeField] private Transform effector = null;

    [SerializeField] private Transform goal = null;

    private void OnValidate()
    {
        List<Transform> bonesTransforms = new List<Transform>();
        Transform current = effector;
        
        while (current != null && current != baseBone)
        {
            bonesTransforms.Insert(0, current);
            current = current.parent;
        }

        bonesTransforms.Insert(0, baseBone);

        int deltaSize = joints.Count - bonesTransforms.Count;
        if (deltaSize < 0)
        {
            joints.AddRange(bonesTransforms.GetRange(0, -deltaSize).Select(x => new Joint(x)));
        }
        else
        {
            joints.RemoveRange(bonesTransforms.Count, deltaSize);
        }
        
        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].transform = bonesTransforms[i];
        }
    }

    private void Start()
    {
        Solver?.SetJoints(in joints);
        Solver?.SetConstraints(in constraints);
    }

    void LateUpdate()
    {
        if (goal)
            Solver?.UpdateJoints(goal.position);
    }
}
