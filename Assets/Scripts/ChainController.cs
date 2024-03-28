using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainController : InverseKinematicsController
{
    public void ReadjustBones()
    {
        for (int i = 0; i < joints.Count - 1; i++)
        {
            Bone bone = joints[i].bone;

            if (!bone.primitive)
                continue;

            Vector3 boneDirection = joints[i + 1].Position - joints[i].Position;

            bone.primitive.transform.up = boneDirection.normalized;
            bone.primitive.transform.position = (joints[i].Position + joints[i + 1].Position) * 0.5f;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        ReadjustBones();
    }

    protected override void Start()
    {
        base.Start();

        ReadjustBones();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        ReadjustBones();
    }
}
