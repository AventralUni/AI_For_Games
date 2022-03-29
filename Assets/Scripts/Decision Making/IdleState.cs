using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{

    public ChaseState chaseState;
    public bool canSeePlayer;


    public override State runCurrentState()
    {
        //throw new System.NotImplementedException();

        if (canSeePlayer)
        {
            return chaseState;
        }
        else
        {
            return this;
        }
        
    }
}
