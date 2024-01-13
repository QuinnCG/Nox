using Cinemachine;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// This class acts as the player's "controller" class.
	/// It manages input from the player and feeds that to the possessed character.
	/// It is also responsible for some other things unique to the player.
	/// </summary>
	public class PlayerManager : MonoBehaviour
	{
		[SerializeField, Required, AssetList(Path = "/Prefabs/Characters")]
		private GameObject DefaultCharacter;
		[SerializeField, Required]
		private CinemachineVirtualCamera VirtualCamera;

		public Character PossessedCharacter { get; private set; }
		public bool InPossessionMode { get; private set; }

		private Movement _movement;

		private void Start()
		{
			var instance = Instantiate(DefaultCharacter, transform.position, Quaternion.identity);
			PossessCharacter(instance.GetComponent<Character>());
		}

		private void Update()
		{
			// While possessing.
			if (PossessedCharacter != null)
			{
				UpdateMoveInput();
				UpdateDashInput();
			}

			// While in possession mode.
			if (InPossessionMode)
			{
				UpdatePossessionInput();
			}
		}

		public void EnterPossessionMode()
		{
			if (!InPossessionMode)
			{
				InPossessionMode = true;
			}
		}

		public void ExitPossessionMode()
		{
			if (InPossessionMode)
			{
				InPossessionMode = false;
			}
		}

		private void UpdateMoveInput()
		{
			var dirInput = new Vector2()
			{
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			}.normalized;

			_movement.Move(dirInput);
		}

		private void UpdateDashInput()
		{
			if (Input.GetKeyDown(KeyCode.LeftAlt)
				|| Input.GetKeyDown(KeyCode.LeftShift)
				|| Input.GetMouseButtonDown(1))
			{
				PossessedCharacter.Dash();
			}
		}

		private void UpdatePossessionInput()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				float radius = 0.5f;

				Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, radius);
				foreach (var collider in colliders)
				{
					if (collider.TryGetComponent(out Character character)
						&& character != PossessedCharacter)
					{
						PossessCharacter(character);
						break;
					}
				}
			}
		}

		private void PossessCharacter(Character character)
		{
			if (PossessedCharacter != null)
				PossessedCharacter.UnPossess();

			character.Possess();
			PossessedCharacter = character;
			OnPossessCharacter(character);

			VirtualCamera.Follow = character.transform;
		}

		private void OnPossessCharacter(Character character)
		{
			_movement = character.GetComponent<Movement>();
		}
	}
}
