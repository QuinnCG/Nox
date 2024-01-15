using Game.DamageSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TextCore.Text;

namespace Game.Player
{
	/// <summary>
	/// This class acts as the player's "controller" class.
	/// It manages input from the player and feeds that to the possessed character.
	/// It is also responsible for some other things unique to the player.
	/// </summary>
	[RequireComponent(typeof(InputReader), typeof(PossessionManager))]
	public class PlayerManager : MonoBehaviour
	{
		public static PlayerManager Instance { get; private set; }

		[SerializeField, Required, AssetList(Path = "/Prefabs/Characters")]
		private GameObject DefaultCharacter;

		[SerializeField, Required]
		private Transform CameraTarget;

		[SerializeField, Tooltip("Higher values will make the camera lean more towards the crosshair.")]
		private float CameraCrosshairBias = 0.3f;

		public Character PossessedCharacter => _possession.PossessedCharacter;
		public bool InPossessionMode => _possession.InPossessionMode;

		public event Action<Character> OnCharacterPossessed, OnCharacterUnpossessed;

		private InputReader _input;
		private PossessionManager _possession;

		// Possessed components.
		private Movement _movement;
		private Health _health;

		private void Awake()
		{
			Instance = this;
			_input = GetComponent<InputReader>();
			_possession = GetComponent<PossessionManager>();

			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;

			_possession.OnCharacterPossessed += character =>
			{
				OnCharacterPossessed?.Invoke(character);

				_movement = character.GetComponent<Movement>();
				_health = character.GetComponent<Health>();
				_health.SetDisplayCriticalIndiactor(false);
			};

			_possession.OnCharacterUnpossessed += character =>
			{
				_health.SetDisplayCriticalIndiactor(true);
				OnCharacterUnpossessed?.Invoke(character);
			};

			// 'Subscribe' methods to specific events,
			// so that they are executed when their host event is triggered.
			_input.OnMove += OnMove;
			_input.OnDash += OnDash;
			_input.OnAttack += OnAttack;

			//_input.OnEnterPossessionMode += OnEnterPossessionMode;
			//_input.OnExitPossessionMode += OnExitPossessionMode;

			_input.OnEnterPossessionMode += _possession.EnterPossessionMode;
			_input.OnExitPossessionMode += _possession.ExitPossessionMode;
		}

		private void Update()
		{
			// Position camera target.
			Vector2 start = _possession.PossessedCharacter.transform.position;
			Vector2 end = CrosshairManager.Instance.CurrentPosition;

			var result = Vector2.Lerp(start, end, CameraCrosshairBias);
			CameraTarget.position = result;
		}

		/* INPUT */
		private void OnMove(Vector2 dir)
		{
			_movement.Move(dir);
		}

		private void OnDash()
		{
			PossessedCharacter.Dash();
		}

		private void OnAttack()
		{
			if (InPossessionMode)
			{
				_possession.PossessSelected();
				return;
			}

			Vector2 target = CrosshairManager.Instance.CurrentPosition;
			PossessedCharacter.Attack(target);
		}
	}
}
