using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bone
{
    public Transform primitive = null;
}

[System.Serializable]
public class Joint
{
    Joint() { }

    public Joint(Transform _transform)
    {
        transform = _transform;
    }

    public Transform transform = null;

    public Bone bone = null;

    public Vector3 Position { get => transform.position; set => transform.position = value; }
    public Vector3 LocalPosition { get => transform.localPosition; set => transform.localPosition = value; }
    public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }
    public Quaternion LocalRotation { get => transform.localRotation; set => transform.localRotation = value; }
}

[System.Serializable]
public class Constraint
{
    public Transform ConstrainedJoint => constrainedJoint;
    public Vector3 ConstraintAxis => constraintAxis;
    public float ConstraintMinAngle => constraintMinAngle;
    public float ConstraintMaxAngle => constraintMaxAngle;

    [SerializeField]
    private Transform constrainedJoint = null;

    [SerializeField]
    private Vector3 constraintAxis = Vector3.forward;

    [SerializeField]
    private float constraintMinAngle = -180f;

    [SerializeField]
    private float constraintMaxAngle = 180f;
}


public class InverseKinematicsDescriptor : ScriptableObject
{
    public virtual void Initialize(in List<Joint> newJoints) => throw new NotImplementedException();
    public virtual void SetConstraints(in List<Constraint> newConstraints) { }
    public virtual void UpdateJoints(in Vector3 goal) => throw new NotImplementedException();
}
