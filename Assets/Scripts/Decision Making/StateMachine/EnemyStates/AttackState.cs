using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public ChaseState chaseState;
    public bool isInAttackRange;

    public override State runCurrentState()
    {
       

        return this;
    }
}
