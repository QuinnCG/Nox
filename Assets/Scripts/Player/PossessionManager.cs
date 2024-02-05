using DG.Tweening;
using FMODUnity;
using Game.AI;
using Game.AI.BossSystem;
using Game.DamageSystem;
using Game.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;
using Cinemachine;

namespace Game.Player
{
	public class PossessionManager : MonoBehaviour
	{
		[SerializeField, BoxGroup("Camera Shake Settings")]
		private CameraShakeManager cameraShake;

		[SerializeField, BoxGroup("Camera Shake Settings")]
		private float shakeIntensity = 10f;

		[SerializeField, BoxGroup("Camera Shake Settings")]
		private float shakeTime = 1f;

		[SerializeField, Required]
		private GameObject DefaultCharacter;

		[Space, SerializeField, Required]
		private GameObject PossessionGhost;

		[SerializeField]
		private GameObject PossessVFX;

		[SerializeField, Unit(Units.MetersPerSecond)]
		private float GhostSpeed = 60f;

		[SerializeField]
		private float PossessionModeTimeScale = 0.3f;

		[SerializeField]
		private float IndicatorYOffset = 0.4f;

		[field: SerializeField]
		public float MaxPossessionMeter { get; set; } = 100f;

		[SerializeField]
		private float DefaultPossessionMeter = 65f;

		[SerializeField]
		private float PossessionMeterMultiplier = 0.3f;

		public static PossessionManager Instance { get; private set; }

		public float CurrentPossessionMeter { get; private set; }

		public Character PossessedCharacter { get; private set; }
		public Vector2 Position
		{
			get
			{
				if (PossessedCharacter == null) return Vector2.zero;
				return PossessedCharacter.transform.position;
			}
		}
		public bool InPossessionMode { get; private set; }
		public bool PossessingNewTarget { get; private set; }

		public Health PossessedHealth { get; private set; }

		public event Action<Character> OnCharacterPossessed, OnCharacterUnpossessed;

		// The character closest to the crosshair while in possession mode.
		private Character _selectedCharacter;
		// The icon above the head of the selected character.
		private GameObject _selectedIndicator;
		// The icon above the head of the currently possessed character.
		private GameObject _possessedIndicator;

		private InputReader _input;
		private bool _possessingOriginal;

		private void Awake()
		{
			Instance = this;

			_input = GetComponent<InputReader>();
		}
		private void Start()
		{
			SpawnOriginalBody(Vector2.zero);
			PlayerManager.Instance.OnDamageEnemy += OnEnemyDamaged;

			SceneManager.Instance.OnPreSceneLoad += () =>
			{
				CurrentPossessionMeter = DefaultPossessionMeter;

				if (PossessedCharacter != null)
				{
					UnPossess(PossessedCharacter);

					if (PossessedCharacter != null)
					{
						Destroy(PossessedCharacter.gameObject);
					}
				}
			};
		}

		private void Update()
		{
			if (InPossessionMode)
			{
				var nearest = FindNearestTarget();
				if (nearest != _selectedCharacter)
				{
					HidePossessIndicator();
					_selectedCharacter = nearest;
					ShowPossessIndicator(nearest);
				}
			}

#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.K))
			{
				PossessedCharacter.GetComponent<Health>().Kill();
			}
#endif
		}

		/* PUBLIC METHODS */
		public void EnterPossessionMode()
		{
			if (!InPossessionMode)
			{
				InPossessionMode = true;
				Time.timeScale = PossessionModeTimeScale;

				ShowSelfIndicator();
			}
		}

		public void ExitPossessionMode()
		{
			if (InPossessionMode && !PossessingNewTarget)
			{
				InPossessionMode = false;
				Time.timeScale = 1f;

				if (_selectedCharacter)
				{
					HidePossessIndicator();
					_selectedCharacter = null;
				}

				HideSelfIndicator();
			}
		}

		public void PossessSelected()
		{
			if (_selectedCharacter)
			{
				Possess(_selectedCharacter);
			}
		}

		public void ReplenishPossessionMeter(float amount)
		{
			CurrentPossessionMeter = Mathf.Min(CurrentPossessionMeter + amount, MaxPossessionMeter);
		}

		public void ConsumePossessionMeter(float amount)
		{
			CurrentPossessionMeter = Mathf.Max(0f, CurrentPossessionMeter - amount);
		}

		public void Respawn()
		{
			Vector2 pos = Vector2.zero;

			if (PossessedCharacter != null)
			{
				var character = PossessedCharacter;

				pos = character.transform.position;
				UnPossess(PossessedCharacter);

				if (character != null)
				{
					Destroy(character);
				}
			}

			SpawnOriginalBody(pos);
			_input.enabled = true;
		}

		/* PRIVATE METHODS */
		private Character FindNearestTarget()
		{
			var cam = Camera.main;
			Vector2 camPos = cam.transform.position;
			var camSize = new Vector2(cam.orthographicSize * 2f * cam.aspect, cam.orthographicSize * 2f);

			Collider2D[] colliders = Physics2D.OverlapBoxAll(camPos, camSize, 0f, LayerMask.GetMask("Character"));

			float nearestDst = float.PositiveInfinity;
			Character nearestChar = null;

			Vector2 crosshairPos = CrosshairManager.Instance.CurrentPosition;

			foreach (var collider in colliders)
			{
				if (!IsPossessable(collider.GetComponent<Character>()))
					continue;

				float dst = Vector2.Distance(crosshairPos, collider.transform.position);
				if (dst < nearestDst)
				{
					nearestDst = dst;
					nearestChar = collider.GetComponent<Character>();
				}
			}

			return nearestChar;
		}

		private bool IsPossessable(Character character)
		{
			if (character == null) return false;

			if (CurrentPossessionMeter < character.PossessionMeterConsumption)
				return false;

			if (character.TryGetComponent(out BossBrain _))
				return false;

			var health = character.GetComponent<Health>();

			if (character == PossessedCharacter) return false;
			if (!health.IsCritical) return false;
			if (health.IsDead) return false;

			// Line of sight test.
			var start = PossessedCharacter.GetComponent<Collider2D>().bounds.center;
			var end = character.GetComponent<Collider2D>().bounds.center;
			var hit = Physics2D.Linecast(start, end, LayerMask.GetMask("Obstacle"));
			if (hit.collider != null) return false;

			return true;
		}

		private void Possess(Character character, bool skip = false)
		{
			_possessingOriginal = false;

			if (character.TryGetComponent(out EnemyBrain brain))
			{
				brain.enabled = false;
			}

			ConsumePossessionMeter(character.PossessionMeterConsumption);

			PossessingNewTarget = true;
			StartCoroutine(PossessSequence(character, skip));
		}

		private void UnPossess(Character character)
		{
			HideSelfIndicator();

			if (character.TryGetComponent(out EnemyBrain brain))
			{
				brain.enabled = true;
			}

			character.UnPossess();
			var health = character.GetComponent<Health>();
			health.OnDeath -= OnDeath;
			health.Kill();

			OnCharacterUnpossessed?.Invoke(character);

			if (_possessingOriginal)
			{
				// TODO: particles?
				Destroy(character.gameObject);
			}
		}

		private Vector2 GetIndicatorPosition(Character character)
		{
			var bounds = character.GetComponent<Collider2D>().bounds;
			Vector2 position = bounds.center + (Vector3.up * (bounds.extents.y + IndicatorYOffset));

			return position;
		}

		/* INDICATORS */
		private void ShowSelfIndicator()
		{
			const string key = "PossessedIndicator.prefab";
			Vector2 position = GetIndicatorPosition(PossessedCharacter);
			Transform parent = PossessedCharacter.transform;

			_possessedIndicator = Addressables
					.InstantiateAsync(key, position, Quaternion.identity, parent)
					.WaitForCompletion();
		}

		private void HideSelfIndicator()
		{
			if (_possessedIndicator)
			{
				Destroy(_possessedIndicator);
			}
		}

		private void ShowPossessIndicator(Character character)
		{
			HidePossessIndicator();

			if (character != null)
			{
				const string key = "PossessionTargetIndicator.prefab";
				Vector2 position = GetIndicatorPosition(character);

				_selectedIndicator = Addressables
								.InstantiateAsync(key, position, Quaternion.identity, character.transform)
								.WaitForCompletion();
			}
		}

		private void HidePossessIndicator()
		{
			if (_selectedIndicator != null)
			{
				Destroy(_selectedIndicator);
			}
		}

		private IEnumerator PossessSequence(Character character, bool skip)
		{
			if (!skip)
			{
				_input.enabled = false;

				Vector2 pos = PossessedCharacter.transform.position;
				GameObject ghost = Instantiate(PossessionGhost, pos, Quaternion.identity);

				Vector2 toTarget = character.transform.position - PossessedCharacter.transform.position;

				float xDir = Mathf.Sign(toTarget.x);
				ghost.transform.localScale = new Vector3(xDir, 1f, 1f);

				yield return ghost.transform
						.DOMove(character.transform.position, GhostSpeed)
						.SetEase(Ease.Linear)
						.SetSpeedBased(true)
						.WaitForCompletion();

				Destroy(ghost);

				pos = character.GetComponent<Collider2D>().bounds.center;
				VisualEffect vfx =
						Instantiate(PossessVFX, pos, Quaternion.identity)
						.GetComponent<VisualEffect>();

				vfx.SetVector3("Direction", toTarget.normalized);
				Destroy(vfx.gameObject, 0.6f);
			}

			if (PossessedCharacter != null)
			{
				UnPossess(PossessedCharacter);
			}

			// Actual possession.
			PossessedCharacter = character;
			PossessedCharacter.Possess();
			if (!skip)
			{
				cameraShake.ShakeCamera(shakeIntensity, shakeTime);
			}


			// Move possessed character to persistant scene.
			var persistantScene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(0);
			PossessedCharacter.transform.parent = null;
			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(PossessedCharacter.gameObject, persistantScene);

			RuntimeManager.SetListenerLocation(character.gameObject);

			var health = PossessedCharacter.GetComponent<Health>();
			health.FullHeal();
			health.OnPossessed();

			PossessedHealth = health;
			health.OnDeath += OnDeath;

			if (InPossessionMode)
			{
				ShowSelfIndicator();
			}

			OnCharacterPossessed?.Invoke(PossessedCharacter);
			_input.enabled = true;

			PossessingNewTarget = false;
		}

		private void OnEnemyDamaged(DamageInfo info)
		{
			ReplenishPossessionMeter(info.Damage * PossessionMeterMultiplier);
		}

		private void OnDeath(DamageType type)
		{
			_possessingOriginal = true;
			if (_possessingOriginal)
			{
				_input.enabled = false;
				HUD.Instance.InitiateGameOver();
			}
			else
			{
				GameObject previousBody = PossessedCharacter.gameObject;
				SpawnOriginalBody(previousBody.transform.position);

				if (previousBody != null)
				{
					//Debug.Log($"Destroying: {previousBody.name}!");
					Destroy(PossessedCharacter.gameObject);
				}
			}
		}

		private void SpawnOriginalBody(Vector2 pos)
		{
			//Debug.Log("Spawning original body!");

			GameObject instance = Instantiate(DefaultCharacter, pos, Quaternion.identity);
			Possess(instance.GetComponent<Character>(), skip: true);

			//Debug.Log("Spawned body!");
			//Debug.Log(instance.name);

			_possessingOriginal = true;
		}
	}
}
