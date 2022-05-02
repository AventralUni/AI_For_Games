using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingState : BaseState
{
    private bool canSeeTower;

    public PathfindingState(Enemy_SM stateMachine) : base("PathfindingState", stateMachine) {   }

    public override void Enter()
    {
        base.Enter();
        canSeeTower = false;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //transition to FlockingState if canSeeTower is true
        if (canSeeTower)
        {
           // stateMachine.ChangeState(((Enemy_SM)stateMachine).flockingState);
        }
    }

}
