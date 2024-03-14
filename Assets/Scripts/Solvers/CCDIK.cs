using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CCDIK", menuName = "Inverse Kinematics/CCDIK", order = 1)]
public class CCDIK : InverseKinematicsDescriptor
{
    [SerializeField] int iterationCount = 5;

    public override void UpdateJoints(ref List<Joint> joints, in Vector3 goal)
    {
        for (int i = 0; i < iterationCount; i++) 
        {
            Vector3 endEffector = joints.Last().Position;
            for (int j = joints.Count - 1; j >= 0; j--) 
            {
                Joint joint = joints[j];
                Vector3 directionToEffector = endEffector - joint.Position;
                Vector3 directionToGoal = goal - joint.Position;

                joint.Position = Quaternion.FromToRotation(directionToEffector, directionToGoal) * joint.Position;
            }
        }
    }

}
