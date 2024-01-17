using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
    // Health-related variables
    public int maxHealth = 100;
    private int currentHealth;

    // Boss state machine
    private BossStateMachine stateMachine;

    void Start()
    {
        // Initialize health and state machine
        currentHealth = maxHealth;
        stateMachine = GetComponent<BossStateMachine>();

        if (stateMachine == null)
        {
            Debug.LogError("BossBase: BossStateMachine component not found!");
        }
    }

    void Update()
    {
        // Check if the boss is defeated
        if (currentHealth <= 0)
        {
            stateMachine.TransitionToState(BossStateMachine.BossState.Defeated);
        }
    }
    
    // I referenced these forms: https://medium.com/@tylerleehenry/creating-a-boss-in-unity-5c7d5ff30b4a
    //                          https://unitycodemonkey.com/video.php?v=qZC1VYWnHZ8

    // Method to take damage
    public void TakeDamage(int damage)
    {
        // Reduce health and clamp it to be non-negative
        currentHealth = Mathf.Max(0, currentHealth - damage);

        // You can add more logic here, like playing a hurt animation or sound
    }

    public void Attack()
    {
        // Perform the attack based on the current state
        switch (stateMachine.CurrentState)
        {
            case BossStateMachine.BossState.Idle:
                // Implement logic for attack during Idle state
                break;

            case BossStateMachine.BossState.Attack:
                // Implement logic for attack during Attack state, would this be AOE and projectile attacks only now?
                break;


            default:
                break;
        }
    }
}
