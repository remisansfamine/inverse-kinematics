using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "FABRIK", menuName = "Inverse Kinematics/FABRIK", order = 1)]
public class FABRIK : InverseKinematicsDescriptor
{
    [SerializeField] int maxIterationCount = 5;
        
    [SerializeField] float tolerance = 0.1f;

    private float totalLength = 0f;
    private List<float> lengths = new List<float>();

    private List<Joint> joints = null;
    private List<Constraint> contraints = null;

    private Joint firstEffector = null;
    private Joint endEffector = null;

    [SerializeField] private bool useClampedLerp;

    private delegate Vector3 LerpDelegate(Vector3 a, Vector3 b, float t); // This defines what type of method you're going to call.
    private LerpDelegate lerpMethod;

    void BackwardSolve(in Vector3 goal)
    {
        endEffector.Position = goal;

        for (int i = joints.Count - 2; i >= 0; i--)
        {
            float boneLength = Vector3.Distance(joints[i].Position, joints[i + 1].Position);
            float lambda = boneLength != 0f ? lengths[i] / boneLength : 0f;

            joints[i].Position = lerpMethod(joints[i + 1].Position, joints[i].Position, lambda);
        }
    }

    void ForwardSolve(in Vector3 initialPosition)
    {
        firstEffector.Position = initialPosition;

        for (int i = 0; i < joints.Count - 1; i++)
        {
            float boneLength = Vector3.Distance(joints[i].Position, joints[i + 1].Position);
            float lambda = boneLength != 0f ? lengths[i] / boneLength : 0f;

            joints[i + 1].Position = lerpMethod(joints[i].Position, joints[i + 1].Position, lambda);

        }
    }

    void ComputeLengths()
    {
        lengths.Clear();
        totalLength = 0f;

        for (int i = 0; i < joints.Count - 1; i++)
        {
            float boneLength = Vector3.Distance(joints[i].Position, joints[i + 1].Position);

            lengths.Add(boneLength);
            totalLength += boneLength;
        }
    }

    public override void Initialize(in List<Joint> newJoints)
    {
        joints = newJoints;

        firstEffector = joints.First();
        endEffector = joints.Last();

        ComputeLengths();

        lerpMethod = useClampedLerp ? Vector3.Lerp : Vector3.LerpUnclamped;
    }

    public override void SetConstraints(in List<Constraint> newContraints)
    {
        contraints = newContraints;
    }

    public override void UpdateJoints(in Vector3 goal)
    {
        float sqrDistance = Vector3.SqrMagnitude(firstEffector.Position - goal);
        if (sqrDistance > totalLength * totalLength)
        {
            for (int i = 0; i < joints.Count - 1; i++)
            {
                float length = Vector3.Distance(joints[i].Position, goal);
                float lambda = length != 0f ? lengths[i] / length : 0;
                joints[i + 1].transform.position = lerpMethod(joints[i].Position, goal, lambda);
            }
        }
        else
        {
            Vector3 initialPosition = firstEffector.Position;

            float sqrReachDistance = Vector3.SqrMagnitude(endEffector.Position - goal);
            float sqrTolerance = tolerance * tolerance;

            for (int iterationCount = 0; iterationCount < maxIterationCount && sqrReachDistance >= sqrTolerance; iterationCount++)
            {
                BackwardSolve(goal);
                ForwardSolve(initialPosition);
                sqrReachDistance = Vector3.SqrMagnitude(endEffector.Position - goal);
            }
        }
    }
}
