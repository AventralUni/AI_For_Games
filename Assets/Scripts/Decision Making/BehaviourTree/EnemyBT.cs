using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyBT : BaseTree
{
    public static float fovRange = 6f;

    protected override Node SetUpTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInFOVRange(transform),
                new TaskFlocking(transform),
            }),
           new TaskPathfinding(transform),
        });

        return root;
    }
}