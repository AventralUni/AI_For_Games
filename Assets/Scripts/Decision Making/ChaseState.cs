using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{

    public AttackState attackState;
    public bool isInAttackRange;

    public override State runCurrentState()
    {
        //throw new System.NotImplementedException();
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
