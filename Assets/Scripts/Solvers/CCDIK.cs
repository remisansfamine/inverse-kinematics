using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CCDIK", menuName = "Inverse Kinematics/CCDIK", order = 1)]
public class CCDIK : InverseKinematicsDescriptor
{
    [SerializeField] int maxIterationCount = 5;

    private List<Joint> joints = null;
    private List<Constraint> contraints = null;

    public override void Initialize(in List<Joint> newJoints)
    {
        joints = newJoints;
    }

    public override void SetConstraints(in List<Constraint> newContraints)
    {
        contraints = newContraints;
    }

    public override void UpdateJoints(in Vector3 goal)
    {
        Joint effector = joints.Last();
        for (int iterationCount = 0; iterationCount < maxIterationCount; iterationCount++) 
        {
            for (int i = joints.Count - 1; i >= 0; i--)
                 UpdateBone(effector.Position, joints[i], in goal);
        }
    }

    public void UpdateBone(Vector3 effector, Joint joint, in Vector3 goal)
    {
        Vector3 jointPosition = joint.Position;

        Vector3 jointToEffector = effector - jointPosition;
        Vector3 jointToGoal = goal - jointPosition;

        Quaternion fromToRotation = Quaternion.FromToRotation(jointToEffector, jointToGoal);
        joint.Rotation = fromToRotation * joint.Rotation;

        Constraint constraint = contraints.FirstOrDefault(constraint => constraint.ConstrainedJoint == joint.transform);

        if (constraint is null)
            return;

        Vector3 currentAxis = fromToRotation * constraint.ConstraintAxis;
        Quaternion rotationBack = Quaternion.FromToRotation(currentAxis, constraint.ConstraintAxis);

        joint.Rotation = rotationBack * joint.Rotation;

        Vector3 axis;
        float angle;
        joint.Rotation.ToAngleAxis(out angle, out axis);

        float clampedAngle = Mathf.Clamp(angle, constraint.ConstraintMinAngle, constraint.ConstraintMaxAngle);
        joint.Rotation = Quaternion.AngleAxis(clampedAngle, axis);
    }
}
