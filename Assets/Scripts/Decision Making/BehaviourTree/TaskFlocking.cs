using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class TaskFlocking : Node
{
    private Transform transform;

    public TaskFlocking(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (Vector3.Distance(transform.position, target.position) > 0.01f)
        {
            //do flocking towards the target

            
        }

        state = NodeState.RUNNING;
        return state;
    }
}
