using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{

    public AttackState attackState;
    public bool isInAttackRange;

    public override State runCurrentState()
    {
        
        if (isInAttackRange)
        {
            return attackState;
        }
        else
        {
            return this;
        }
        
    }
}
