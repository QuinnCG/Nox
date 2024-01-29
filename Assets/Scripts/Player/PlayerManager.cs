using DG.Tweening;
using FMODUnity;
using Game.DamageSystem;
using Game.MovementSystem;
using Game.ProjectileSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Player
{
	/// <summary>
	/// This class acts as the player's "controller" class.
	/// It manages input from the player and feeds that to the possessed character.
	/// It is also responsible for some other things unique to the player.
	/// </summary>
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(PossessionManager))]
	public class PlayerManager : MonoBehaviour
	{
		public static PlayerManager Instance { get; private set; }

		[SerializeField, Required, AssetList(Path = "/Prefabs/Characters")]
		private GameObject DefaultCharacter;

		[SerializeField, Required]
		private Transform CameraTarget;

		[SerializeField, Tooltip("Higher values will make the camera lean more towards the crosshair.")]
		private float CameraCrosshairBias = 0.3f;

		[SerializeField]
		private float DamageImmunityDuration = 2.5f;

		[SerializeField]
		private EventReference DamagedSound;

		public event Action<DamageInfo> OnDamageEnemy;

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
				_movement = character.GetComponent<Movement>();
				_health = character.GetComponent<Health>();
				_health.SetDisplayCriticalIndiactor(false);

				_health.OnDamaged += OnDamaged;
			};

			_possession.OnCharacterUnpossessed += character =>
			{
				_health.SetDisplayCriticalIndiactor(true);
			};

			// 'Subscribe' methods to specific events,
			// so that they are executed when their host event is triggered.
			_input.OnMove += OnMove;
			_input.OnDash += OnDash;
			_input.OnAttack += OnAttack;
		}

		private void Update()
		{
			if (PossessionManager.Instance.PossessedCharacter == null)
			{
				return;
			}

			// Position camera target.
			Vector2 start = _possession.PossessedCharacter.transform.position;
			Vector2 end = CrosshairManager.Instance.CurrentPosition;

			var result = Vector2.Lerp(start, end, CameraCrosshairBias);
			CameraTarget.position = result;

			if (_input.IsPossessionModeActive && !_possession.InPossessionMode)
			{
				_possession.EnterPossessionMode();
			}
			else if (!_input.IsPossessionModeActive && _possession.InPossessionMode)
			{
				_possession.ExitPossessionMode();
			}
		}

		public void OnSpawnProjectile(Projectile projectile)
		{
			projectile.OnDamage += OnDamageEnemy;
		}

		/* INPUT */
		private void OnMove(Vector2 dir)
		{
			_movement.Move(dir);
		}

		private void OnDash()
		{
			PossessionManager.Instance.PossessedCharacter.Dash();
		}

		private void OnAttack()
		{
			if (PossessionManager.Instance.InPossessionMode && !_possession.PossessingNewTarget)
			{
				_possession.PossessSelected();
				return;
			}

			Vector2 target = CrosshairManager.Instance.CurrentPosition;
			PossessionManager.Instance.PossessedCharacter.Attack(target);
		}

		/* OTHER */

		private void OnDamaged(float amount)
		{
			if (!DamagedSound.IsNull)
			{
				RuntimeManager.PlayOneShotAttached(DamagedSound, Camera.main.gameObject);
			}

			_health.DisableDamage = true;
			OnImmunityStart();

			DOVirtual.DelayedCall(DamageImmunityDuration, () =>
			{
				_health.DisableDamage = false;
				OnImmunityEnd();
			});
		}

		private void OnImmunityStart()
		{

		}

		private void OnImmunityEnd()
		{

		}
	}
}
