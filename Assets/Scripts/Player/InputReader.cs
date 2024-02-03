using Game.InputSystem;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Player
{
	public class InputReader : MonoBehaviour
	{
		public event Action<Vector2> OnMove;
		public event Action OnDash;
		public event Action OnAttack;

		public bool IsPossessionModeActive { get; private set; }

		private PlayerControls _input;

		private void Awake()
		{
			// This is required to be constructed in Awake or Start.

			// The code for PlayerControls is dynamically generated
			// to match what we place in the asset file.
			_input = new PlayerControls();

			// Brackey's has a good video about the "New Input System".
			_input.PlayerMap.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>().normalized);
			_input.PlayerMap.Dash.performed += _ => OnDash?.Invoke();
			_input.PlayerMap.Attack.performed += _ => OnAttack?.Invoke();
		}

		private void OnEnable()
		{
			_input.Enable();
		}

		private void OnDisable()
		{
			_input.Disable();
		}

		private void Update()
		{
			Vector2 dir = _input.PlayerMap.Move.ReadValue<Vector2>();

			dir.Normalize();
			OnMove?.Invoke(dir);

			IsPossessionModeActive = _input.PlayerMap.PossessionMode.ReadValue<float>() > 0f;
		}
	}
}
