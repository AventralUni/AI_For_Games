using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    State currentState;

    // Update is called once per frame
    void Update()
    {
        runStateMachine();
    }

    private void runStateMachine()
    {
        //Takes the state, runs the logic and returns a state
        State nextState = currentState?.runCurrentState();

        if (nextState != null)
        {
            //Switch to next state
            switchNextState(nextState);
        }
    }

    private void switchNextState(State nextState)
    {
        currentState = nextState;
    }
}
