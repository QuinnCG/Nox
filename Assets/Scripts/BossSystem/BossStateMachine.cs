using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Attack,
        Defeated
    }

    private BossState currentState;

    void Start()
    {
        // Start the boss in the Idle state
        TransitionToState(BossState.Idle);
    }

    void Update()
    {
        // Check for state transitions or perform state-specific actions
        switch (currentState)
        {
            case BossState.Idle:
                // Idle state logic with a transition condition placeholder
                if(true)
                {
                    TransitionToState(BossState.Attack);
                }
                break;

            case BossState.Attack:
                // Attack state logic with a transition condition placeholder
                if(true)
                {
                    TransitionToState(BossState.Defeated);
                }
                break;

            case BossState.Defeated:
                // Defeated state logic
                break;
        }
    }

    void TransitionToState(BossState newState)
    {
        // Exit the current state
        ExitState();

        // Enter the new state
        currentState = newState;
        EnterState();
    }

    void EnterState()
    {
        // Enter state logic (initialization) for each state
        switch (currentState)
        {
            case BossState.Idle:
                // Initialize Idle state
                break;

            case BossState.Attack:
                // Initialize Attack state
                break;

            case BossState.Defeated:
                // Initialize Defeated state
                break;
        }
    }

    void ExitState()
    {
        // Exit state logic (cleanup) for each state
        // This could include cleaning up any temporary variables or resetting values
    }
}

