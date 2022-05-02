using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SM : StateManager
{

    public PathfindingState pathfindingState;
    public FlockingState flockingState;

    private void Awake()
    {
        pathfindingState = new PathfindingState(this);
        flockingState = new FlockingState(this);
    }

    protected override BaseState GetInitialState()
    {
        return pathfindingState;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
