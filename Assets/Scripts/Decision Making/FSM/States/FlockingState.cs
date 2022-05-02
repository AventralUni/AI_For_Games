using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingState : BaseState
{
    private bool canSeeTower;

    public FlockingState(Enemy_SM stateMachine) : base("FlockingState", stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        canSeeTower = false;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //transition to FlockingState if canSeeTower is false
        if (!canSeeTower)
        {
           // stateMachine.ChangeState(((Enemy_SM)stateMachine).pathfindingState);
        }
    }
}