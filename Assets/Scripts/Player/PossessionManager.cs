using DG.Tweening;
using Game.DamageSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Game.Player
{
	public class PossessionManager : MonoBehaviour
	{
		[SerializeField, Required, AssetList(Path = "/Prefabs/Characters")]
		private GameObject DefaultCharacter;

		[Space, SerializeField, Required]
		private GameObject PossessionGhost;

		[SerializeField]
		private GameObject PossessVFX;

		[SerializeField]
		private float CastingTime = 0.35f;

		[SerializeField]
		private float PossessionModeTimeScale = 0.3f;

		[SerializeField]
		private float IndicatorYOffset = 0.4f;

		public Character PossessedCharacter { get; private set; }
		public bool InPossessionMode { get; private set; }
		public bool PossessingNewTarget { get; private set; }

		public event Action<Character> OnCharacterPossessed, OnCharacterUnpossessed;

		// The character closest to the crosshair while in possession mode.
		private Character _selectedCharacter;
		// The icon above the head of the selected character.
		private GameObject _selectedIndicator;
		// The icon above the head of the currently possessed character.
		private GameObject _possessedIndicator;

		private InputReader _input;

		private void Awake()
		{
			_input = GetComponent<InputReader>();
		}

		private void Start()
		{
			var instance = Instantiate(DefaultCharacter, transform.position, Quaternion.identity);
			Possess(instance.GetComponent<Character>(), skip: true);
		}

		private void Update()
		{
			// While in possession mode.
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
			var health = character.GetComponent<Health>();

			if (character == PossessedCharacter) return false;
			if (!health.IsCritical) return false;
			if (health.IsDead) return false;

			return true;
		}

		private void Possess(Character character, bool skip = false)
		{
			StartCoroutine(PossessSequence(character, skip));
		}

		private void Unpossess(Character character)
		{
			HideSelfIndicator();
			character.UnPossess();
			character.GetComponent<Health>().Kill();

			OnCharacterUnpossessed?.Invoke(character);
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

			const string key = "PossessionTargetIndicator.prefab";
			Vector2 position = GetIndicatorPosition(character);

			_selectedIndicator = Addressables
				.InstantiateAsync(key, position, Quaternion.identity, character.transform)
				.WaitForCompletion();
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
			PossessingNewTarget = true;

			if (!skip)
			{
				_input.enabled = false;

				Vector2 pos = PossessedCharacter.transform.position;
				GameObject ghost = Instantiate(PossessionGhost, pos, Quaternion.identity);

				Vector2 toTarget = character.transform.position - PossessedCharacter.transform.position;
				float xDir = Mathf.Sign(toTarget.x);
				ghost.transform.localScale = new Vector3(xDir, 1f, 1f);

				yield return ghost.transform
					.DOMove(character.transform.position, CastingTime)
					.SetEase(Ease.Linear)
					.WaitForCompletion();

				Destroy(ghost);

				pos = character.GetComponent<Collider2D>().bounds.center;
				var vfx =
					Instantiate(PossessVFX, pos, Quaternion.identity)
					.GetComponent<VisualEffect>();

				vfx.SetVector3("Direction", toTarget.normalized);

				Task.Run(() =>
				{
					while (vfx.aliveParticleCount != 0)
					{
						if (vfx.gameObject == null)
						{
							return;
						}
					}

					if (vfx.gameObject)
						Destroy(vfx.gameObject);
				});
			}

			if (PossessedCharacter != null)
			{
				Unpossess(PossessedCharacter);
			}

			PossessedCharacter = character;
			character.Possess();

			if (InPossessionMode)
			{
				ShowSelfIndicator();
			}

			OnCharacterPossessed?.Invoke(character);
			_input.enabled = true;

			PossessingNewTarget = false;
		}
	}
}
