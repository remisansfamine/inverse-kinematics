using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "FABRIK", menuName = "Inverse Kinematics/FABRIK", order = 1)]
public class FABRIK : InverseKinematicsDescriptor
{
    [SerializeField] int maxIterationCount = 5;
        
    [SerializeField] float epsilon = 0.1f;

    private float totalLength = 0f;
    private List<float> lengths = new List<float>();

    private List<Joint> joints = null;

    void BackwardSolve(in List<float> lengths, in Vector3 goal)
    {
        Joint endEffector = joints.Last();
        endEffector.Position = goal;

        for (int i = joints.Count - 2; i > 0; i--)
        {
            float boneLength = Vector3.Distance(joints[i].Position, joints[i + 1].Position);
            float lambda = boneLength != 0f ? lengths[i] / boneLength : 0f;

            joints[i].Position = Vector3.LerpUnclamped(joints[i + 1].Position, joints[i].Position, lambda);
        }
    }

    void ForwardSolve(in List<float> lengths)
    {
        for (int i = 0; i < joints.Count - 1; i++)
        {
            float boneLength = Vector3.Distance(joints[i].Position, joints[i + 1].Position);
            float lambda = boneLength != 0f ? lengths[i] / boneLength : 0f;

            joints[i + 1].Position = Vector3.LerpUnclamped(joints[i].Position, joints[i + 1].Position, lambda);
        }
    }

    public override void SetJoints(in List<Joint> newJoints)
    {
        joints = newJoints;

        Vector3 lastPosition = joints.Last().Position;
        for (int i = 0; i < joints.Count - 1; i++)
        {
            float boneLength = Vector3.Distance(joints[i].Position, lastPosition);

            lengths.Add(boneLength);
            totalLength += boneLength;

            lastPosition = joints[i].Position;
        }
    }

    public override void UpdateJoints(in Vector3 goal)
    {
        float distance = Vector3.SqrMagnitude(joints.First().Position - goal);
        if (distance > totalLength * totalLength)
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
            Joint endEffector = joints.Last();
            float reachDistance = Vector3.SqrMagnitude(endEffector.Position - goal);

            for (int iterationCount = 0; iterationCount < maxIterationCount && reachDistance >= epsilon * epsilon; iterationCount++)
            {
                BackwardSolve(lengths, goal);
                ForwardSolve(lengths);
                reachDistance = Vector3.SqrMagnitude(endEffector.Position - goal);
            }
        }
    }
}
