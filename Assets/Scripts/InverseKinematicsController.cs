using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InverseKinematicsController : MonoBehaviour
{
    [SerializeField] protected InverseKinematicsDescriptor solverScript = null;
    protected InverseKinematicsDescriptor solverInstance = null;

    [SerializeField] protected List<Joint> joints = new List<Joint>();

    // Store the constraints to an external list to avoid memory allocation for bones that do not use constraint
    [SerializeField] protected List<Constraint> constraints = new List<Constraint>();

    // Transforms used to initialize the joints list
    [SerializeField] protected Transform baseBone = null;
    [SerializeField] protected Transform effector = null;
    [SerializeField] protected Transform goal = null;

    protected virtual void OnValidate()
    {
        List<Transform> bonesTransforms = new List<Transform>();
        Transform current = effector;
        
        // Cover all the transforms between the effector and the basebone
        // Every transform must be inserted at the index 0 to reverse the parent hierarchy of the bones
        while (current && current != baseBone)
        {
            bonesTransforms.Insert(0, current);
            current = current.parent;
        }

        bonesTransforms.Insert(0, baseBone);

        // Resize dynamically the list to avoid losing data during the OnValidate 
        int deltaSize = joints.Count - bonesTransforms.Count;
        if (deltaSize < 0)
        {
            joints.AddRange(bonesTransforms.GetRange(0, -deltaSize).Select(x => new Joint(x)));
        }
        else
        {
            joints.RemoveRange(bonesTransforms.Count, deltaSize);
        }
        
        // Setup the transform of the joints with the new transform
        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].transform = bonesTransforms[i];
        }

    }

    protected virtual void Start()
    {
        if (!solverScript)
            return;

        // Create an instance of the solver to avoid sharing data with all solvers
        solverInstance = Instantiate(solverScript);

        // Initialize the solver to allow precomputing
        solverInstance.Initialize(in joints);
        solverInstance.SetConstraints(in constraints);
    }

    protected virtual void LateUpdate()
    {
        if (goal)
            solverInstance?.UpdateJoints(goal.position);
    }
}
