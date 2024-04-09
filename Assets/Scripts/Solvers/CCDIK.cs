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
                 UpdateJoint(joints[i], effector.Position, in goal);
        }
    }

    public void UpdateJoint(Joint joint, in Vector3 effector, in Vector3 goal)
    {
        Vector3 jointPosition = joint.Position;

        Vector3 jointToEffector = effector - jointPosition;
        Vector3 jointToGoal = goal - jointPosition;

        Quaternion fromToRotation = Quaternion.FromToRotation(jointToEffector, jointToGoal);
        joint.Rotation = fromToRotation * joint.Rotation;

        // Get the joint associated constraint and apply it if there is any
        Constraint constraint = contraints.FirstOrDefault(constraint => constraint.ConstrainedJoint == joint.transform);

        if (constraint is null)
            return;

        ApplyConstraint(joint, fromToRotation, constraint);
    }

    protected void ApplyConstraint(Joint joint, in Quaternion fromToRotation, in Constraint constraint)
    {
        // Get the constraint axis in the joint-goal-effector space
        Vector3 currentAxis = fromToRotation * constraint.ConstraintAxis;
        Quaternion rotationBack = Quaternion.FromToRotation(currentAxis, constraint.ConstraintAxis);

        joint.Rotation = rotationBack * joint.Rotation;

        Vector3 axis;
        float angle;
        joint.Rotation.ToAngleAxis(out angle, out axis);

        // Clamp the angle of the initial rotation with the constraint
        float clampedAngle = Mathf.Clamp(angle, constraint.ConstraintMinAngle, constraint.ConstraintMaxAngle);
        joint.Rotation = Quaternion.AngleAxis(clampedAngle, axis);
    }
}
