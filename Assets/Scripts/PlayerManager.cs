using Cinemachine;
using Game.DamageSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
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

		private InputReader _input;

		// Possessed components.
		private Movement _movement;

		// The character closest to the crosshair while in possession mode.
		private Character _selectedCharacter;

		private void Awake()
		{
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
				var pastChar = _selectedCharacter;
				FindCharacterNearestToCrosshair();

				if (pastChar != _selectedCharacter)
				{
					if (pastChar)
					{
						SetCharacterOutline(pastChar, false);
					}

					if (_selectedCharacter)
					{
						SetCharacterOutline(_selectedCharacter, true);
					}
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

		/* PUBLIC METHODS */
		public void EnterPossessionMode()
		{
			if (!InPossessionMode)
			{
				InPossessionMode = true;
				Time.timeScale = 0.3f;
			}
		}

		public void ExitPossessionMode()
		{
			if (InPossessionMode)
			{
				InPossessionMode = false;
				Time.timeScale = 1f;

				if (_selectedCharacter)
				{
					SetCharacterOutline(_selectedCharacter, false);
					_selectedCharacter = null;
				}
			}
		}

		/* PRIVATE METHODS */
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
				PossessSelectedCharacter();
				// Exit method to avoid following code from being executed.
				return;
			}

			Vector2 target = CrosshairManager.Instance.CurrentPosition;
			PossessedCharacter.Attack(target);
		}

		private void SetCharacterOutline(Character character, bool enabled)
		{
			var sprite = character.GetComponentInChildren<SpriteRenderer>();
			sprite.material.SetInt("_Enabled", Convert.ToInt32(enabled));
		}

		// Store the character neartest to the crosshair.
		private void FindCharacterNearestToCrosshair()
		{
			Vector2 crosshairPos = CrosshairManager.Instance.CurrentPosition;
			Collider2D[] colliders = Physics2D.OverlapCircleAll(crosshairPos, PossessionRadius, LayerMask.GetMask("Character"));

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

			// Null is possibile if no characters are within the defined radius.
			// A possibile optimization would be to do a smaller circle. Failing that then you do the bigger one.
			_selectedCharacter = nearestChar;
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
			_movement = character.GetComponent<Movement>();
		}
	}
}
