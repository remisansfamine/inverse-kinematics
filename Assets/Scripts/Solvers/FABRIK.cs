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

    Vector3 initialPosition = Vector3.zero;

    private Joint firstEffector = null;
    private Joint endEffector = null;

    void BackwardSolve(in Vector3 goal)
    {
        endEffector.Position = goal;

        for (int i = joints.Count - 2; i >= 0; i--)
        {
            float boneLength = Vector3.Distance(joints[i].Position, joints[i + 1].Position);
            float lambda = boneLength != 0f ? lengths[i] / boneLength : 0f;

            joints[i].Position = Vector3.LerpUnclamped(joints[i + 1].Position, joints[i].Position, lambda);
        }
    }

    void ForwardSolve()
    {
        firstEffector.Position = initialPosition;

        for (int i = 0; i < joints.Count - 1; i++)
        {
            float boneLength = Vector3.Distance(joints[i].Position, joints[i + 1].Position);
            float lambda = boneLength != 0f ? lengths[i] / boneLength : 0f;

            joints[i + 1].Position = Vector3.LerpUnclamped(joints[i].Position, joints[i + 1].Position, lambda);
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

    public override void SetJoints(in List<Joint> newJoints)
    {
        joints = newJoints;

        firstEffector = joints.First();
        endEffector = joints.Last();

        initialPosition = firstEffector.Position;

        ComputeLengths();
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
                joints[i + 1].transform.position = Vector3.LerpUnclamped(joints[i].Position, goal, lambda);
            }
        }
        else
        {
            float sqrReachDistance = Vector3.SqrMagnitude(endEffector.Position - goal);
            float sqrTolerance = tolerance * tolerance;

            for (int iterationCount = 0; iterationCount < maxIterationCount && sqrReachDistance >= sqrTolerance; iterationCount++)
            {
                BackwardSolve(goal);
                ForwardSolve();
                sqrReachDistance = Vector3.SqrMagnitude(endEffector.Position - goal);
            }
        }
    }
}
