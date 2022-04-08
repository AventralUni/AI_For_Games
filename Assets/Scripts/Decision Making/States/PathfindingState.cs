using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingState : State
{
    public FlockingState flockingState;
    public bool canSeeTower;

    public override State runCurrentState()
    {
        if (canSeeTower)
        {
            return flockingState;
        }
        else
        {
            return this;
        }

    }
}
