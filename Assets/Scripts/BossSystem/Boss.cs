using Game.DamageSystem;
using UnityEngine;

[RequireComponent(typeof(BossStateMachine))]
[RequireComponent(typeof(Health))]
public abstract class Boss : MonoBehaviour
{
	// Health-related variables
	protected float MaxHP => Health.Max;
	protected float CurrentHP => Health.Current;

	protected Health Health { get; private set; }

	// Boss state machine
	private BossStateMachine stateMachine;

	void Awake()
	{
		Health = GetComponent<Health>();

		// Initialize health and state machine
		stateMachine = GetComponent<BossStateMachine>();

		if (stateMachine == null)
		{
			Debug.LogError("BossBase: BossStateMachine component not found!");
		}
	}

	void Update()
	{
		// Check if the boss is defeated
		if (CurrentHP <= 0)
		{
			stateMachine.TransitionToState(BossState.Defeated);
		}
	}

	public void Attack()
	{
		// Perform the attack based on the current state
		switch (stateMachine.CurrentState)
		{
			case BossState.Idle:
				// Implement logic for attack during Idle state
				break;

			case BossState.Attack:
				// Implement logic for attack during Attack state, would this be AOE and projectile attacks only now?
				break;

			default:
				break;
		}
	}
}
