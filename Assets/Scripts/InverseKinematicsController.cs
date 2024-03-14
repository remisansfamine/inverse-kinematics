using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematicsController : MonoBehaviour
{
    [SerializeField] private InverseKinematicsDescriptor Solver = null;

    [SerializeField] private List<Joint> joints = new List<Joint>();

    [SerializeField] private Transform goal = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (goal)
            Solver?.UpdateJoints(ref joints, goal.position);
    }
}
