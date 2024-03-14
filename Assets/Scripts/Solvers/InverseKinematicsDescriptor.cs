using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Joint
{
    public Transform transform;

    public Vector3 Position { get => transform.position; set => transform.position = value; }
}

public class InverseKinematicsDescriptor : ScriptableObject
{
    public virtual void UpdateJoints(ref List<Joint> joints, in Vector3 goal) => throw new NotImplementedException();
}
