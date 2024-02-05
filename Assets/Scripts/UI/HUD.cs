using DG.Tweening;
using Game.DamageSystem;
using Game.Player;
using Game.RoomSystem;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HUD : MonoBehaviour
	{
		public static HUD Instance { get; private set; }

		[SerializeField, BoxGroup("References"), Required]
		private Slider PlayerHealth;

		[SerializeField, BoxGroup("References"), Required]
		private Slider PossessionMeter;

		[SerializeField, BoxGroup("References"), Required]
		private GameObject BossContainer;

		[SerializeField, BoxGroup("References"), Required]
		private Slider BossHealth;

		[SerializeField, BoxGroup("References"), Required]
		private TextMeshProUGUI BossTitle;

		[SerializeField, BoxGroup("References"), Required]
		private GameObject HelpText;

		[SerializeField, BoxGroup("References"), Required]
		private CriticalHealthOverlay criticalHealthOverlay;

		[field: SerializeField, BoxGroup("References"), Required]
		public Image Blackout { get; private set; }

		[field: SerializeField, BoxGroup("References"), Required]
		public GameObject GameOverRoot { get; private set; }

		private Health _health;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			PossessionManager.Instance.OnCharacterPossessed += OnCharacterPossessed;
			OnCharacterPossessed(PossessionManager.Instance.PossessedCharacter);

			HideBoss();
		}

		private void Update()
		{
			if (_health)
			{
				PlayerHealth.value = _health.Current / _health.Max;

				// Check for critical health to show the overlay
				if (_health.IsCritical && criticalHealthOverlay != null)
				{
					criticalHealthOverlay.ShowCriticalOverlay();
				}
				else
				{
					criticalHealthOverlay.HideCriticalOverlay();
				}
			}

			var p = PossessionManager.Instance;
			PossessionMeter.value = p.CurrentPossessionMeter / p.MaxPossessionMeter;

			HelpText.SetActive(PossessionMeter.value > 0.5);

			if (Room.Current != null && Room.Current.Boss != null)
			{
				var hp = Room.Current.Boss.Health;
				BossHealth.value = hp.Current / hp.Max;
			}
		}

		public void InitiateGameOver()
		{
			if (GameManager.Instance.InGameOver) return;

			Time.timeScale = 0f;
			GameManager.Instance.InGameOver = true;

			CrosshairManager.Instance.enabled = false;
			GameOverRoot.SetActive(true);

			if (Room.Current != null)
			{
				Room.Current.StopBossMusic();
			}
		}

		public void RetryButton()
		{
			GameManager.Instance.InGameOver = false;
			GameOverRoot.SetActive(false);
			Time.timeScale = 1f;

			CrosshairManager.Instance.enabled = true;
			RoomManager.Instance.Reload();
		}

		public void OnBossRoomStart(Room room)
		{
			BossTitle.text = room.Boss.Title;
			ShowBoss();

			room.OnBossDeath += () =>
			{
				HideBoss();
			};
		}

		private void OnCharacterPossessed(Character character)
		{
			if (character == null)
			{
				// Handle the case when the character is null.
				return;
			}

			_health = character.GetComponent<Health>();
		}

		private void ShowBoss()
		{
			BossContainer.SetActive(true);
		}

		private void HideBoss()
		{
			BossContainer.SetActive(false);
		}
	}
}
