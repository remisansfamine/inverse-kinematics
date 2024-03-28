using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InverseKinematicsController : MonoBehaviour
{
    [SerializeField] protected InverseKinematicsDescriptor solverScript = null;
    protected InverseKinematicsDescriptor solverInstance = null;

    [SerializeField] protected List<Joint> joints = new List<Joint>();
    [SerializeField] protected List<Constraint> constraints = new List<Constraint>();

    [SerializeField] protected Transform baseBone = null;

    [SerializeField] protected Transform effector = null;

    [SerializeField] protected Transform goal = null;

    protected virtual void OnValidate()
    {
        List<Transform> bonesTransforms = new List<Transform>();
        Transform current = effector;
        
        while (current && current != baseBone)
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

    protected virtual void Start()
    {
        if (!solverScript)
            return;

        solverInstance = Instantiate(solverScript);
        solverInstance.Initialize(in joints);
        solverInstance.SetConstraints(in constraints);
    }

    protected virtual void LateUpdate()
    {
        if (goal)
            solverInstance?.UpdateJoints(goal.position);
    }
}
