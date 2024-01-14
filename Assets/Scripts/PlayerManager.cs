using Game.DamageSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TextCore.Text;

namespace Game
{
	/// <summary>
	/// This class acts as the player's "controller" class.
	/// It manages input from the player and feeds that to the possessed character.
	/// It is also responsible for some other things unique to the player.
	/// </summary>
	[RequireComponent(typeof(InputReader))]
	public class PlayerManager : MonoBehaviour
	{
		public static PlayerManager Instance { get; private set; }

		[SerializeField, Required, AssetList(Path = "/Prefabs/Characters")]
		private GameObject DefaultCharacter;

		[SerializeField, Required]
		private Transform CameraTarget;

		[SerializeField]
		private float PossessionRadius = 3f;

		[SerializeField, Unit(Units.Percent), Tooltip("Higher values will make the camera lean more towards the crosshair.")]
		private float CameraCrosshairBias = 0.3f;

		public Character PossessedCharacter { get; private set; }
		public bool InPossessionMode { get; private set; }

		public event Action<Character> OnCharacterPossessed;

		// Possessed components.
		public Movement Movement { get; private set; }
		public Health Health { get; private set; }

		private InputReader _input;

		// The character closest to the crosshair while in possession mode.
		private Character _selectedCharacter;
		private GameObject _selectedIndicator;
		private GameObject _possessedIndicator;

		private void Awake()
		{
			Instance = this;
			_input = GetComponent<InputReader>();

			// 'Subscribe' methods to specific events,
			// so that they are executed when their host event is triggered.
			_input.OnMove += OnMove;
			_input.OnDash += OnDash;
			_input.OnAttack += OnAttack;

			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;
		}

		private void Start()
		{
			var instance = Instantiate(DefaultCharacter, transform.position, Quaternion.identity);
			PossessCharacter(instance.GetComponent<Character>());
		}

		private void Update()
		{
			// While in possession mode.
			if (InPossessionMode)
			{
				FindCharacterNearestToCrosshair();

				if (_selectedCharacter)
				{
					ShowPossessTargetIndicator(_selectedCharacter);
				}
			}

			// Possesion mode controls.
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				EnterPossessionMode();
			}
			else if (Input.GetKeyUp(KeyCode.Tab))
			{
				ExitPossessionMode();
			}

			// Position camera target.
			Vector2 start = PossessedCharacter.transform.position;
			Vector2 end = CrosshairManager.Instance.CurrentPosition;

			var result = Vector2.Lerp(start, end, CameraCrosshairBias);
			CameraTarget.position = result;
		}

		/* INPUT */
		private void OnMove(Vector2 dir)
		{
			Movement.Move(dir);
		}

		private void OnDash()
		{
			PossessedCharacter.Dash();
		}

		private void OnAttack()
		{
			if (InPossessionMode)
			{
				PossessSelectedCharacter();
				// Exit method to avoid following code from being executed.
				return;
			}

			Vector2 target = CrosshairManager.Instance.CurrentPosition;
			PossessedCharacter.Attack(target);
		}

		/* POSSESSION MODE */
		private void EnterPossessionMode()
		{
			if (!InPossessionMode)
			{
				InPossessionMode = true;
				Time.timeScale = 0.3f;

				const string key = "PossessedIndicator.prefab";
				Vector2 position = GetIndicatorPosition(PossessedCharacter);

				_possessedIndicator = Addressables.InstantiateAsync(key, position, Quaternion.identity, PossessedCharacter.transform)
					.WaitForCompletion();
			}
		}

		private void ExitPossessionMode()
		{
			if (InPossessionMode)
			{
				InPossessionMode = false;
				Time.timeScale = 1f;

				if (_selectedCharacter)
				{
					HidePossessTargetIndicator();
					_selectedCharacter = null;
				}

				if (_possessedIndicator)
				{
					Destroy(_possessedIndicator);
				}
			}
		}

		private void ShowPossessTargetIndicator(Character character)
		{
			HidePossessTargetIndicator();

			const string key = "PossessionTargetIndicator.prefab";
			Vector2 position = GetIndicatorPosition(character);

			_selectedIndicator = Addressables.InstantiateAsync(key, position, Quaternion.identity, character.transform)
				.WaitForCompletion();
		}

		private void HidePossessTargetIndicator()
		{
			if (_selectedIndicator != null)
			{
				Destroy(_selectedIndicator);
			}
		}

		// Store the character nearest to the crosshair.
		private void FindCharacterNearestToCrosshair()
		{
			Vector2 crosshairPos = CrosshairManager.Instance.CurrentPosition;
			Collider2D[] colliders = Physics2D.OverlapCircleAll(crosshairPos, PossessionRadius, LayerMask.GetMask("Character"));

			// TODO: Replace circle cast with box cast in shape of camera frustum.

			float nearestDst = float.PositiveInfinity;
			Character nearestChar = null;

			foreach (var collider in colliders)
			{
				if (!DoesCharacterMeetPossessionCriteria(collider.GetComponent<Character>()))
					continue;

				float dst = Vector2.Distance(crosshairPos, collider.transform.position);
				if (dst < nearestDst)
				{
					nearestDst = dst;
					nearestChar = collider.GetComponent<Character>();
				}
			}

            if (nearestChar != _selectedCharacter)
            {
				HidePossessTargetIndicator();
				_selectedCharacter = nearestChar;
			}
		}

		private bool DoesCharacterMeetPossessionCriteria(Character character)
		{
			if (character == PossessedCharacter) return false;
			if (!character.GetComponent<Health>().IsCritical) return false;

			return true;
		}

		private void PossessSelectedCharacter()
		{
			if (_selectedCharacter)
			{
				PossessCharacter(_selectedCharacter);
			}
		}

		private void PossessCharacter(Character character)
		{
			PossessedCharacter = character;

			Movement = character.GetComponent<Movement>();
			Health = character.GetComponent<Health>();

			OnCharacterPossessed?.Invoke(character);
		}

		private Vector2 GetIndicatorPosition(Character character)
		{
			// TODO: Scale offset with bounds size.

			var bounds = character.GetComponent<Collider2D>().bounds;
			float offset = 0.2f;
			Vector2 position = bounds.center + (Vector3.up * (bounds.extents.y + offset));

			return position;
		}
	}
}
