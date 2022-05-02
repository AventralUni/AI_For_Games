using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public string name;
    protected StateManager stateManager;

    public BaseState(string name, StateManager stateManager)
    {
        this.name = name;
        this.stateManager = stateManager;
    }

    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }
}
