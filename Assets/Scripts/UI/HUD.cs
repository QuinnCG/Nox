﻿using DG.Tweening;
using Game.DamageSystem;
using Game.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HUD : MonoBehaviour
	{
		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthFadeIn = 1f;

		[SerializeField, BoxGroup("Animation Settings")]
		private float PlayerHealthFadeOut = 2f;

		[SerializeField, BoxGroup("References"), Required]
		private Slider PlayerHealth;

		[SerializeField, BoxGroup("References"), Required]
		private Slider PossessionMeter;

		[SerializeField, BoxGroup("References"), Required]
		private GameObject BossContainer;

		[SerializeField, BoxGroup("References"), Required]
		private Slider BossHealth;

		private Character _lastCharacter;
		private Health _health;

		private void Start()
		{
			PossessionManager.Instance.OnCharacterPossessed += OnCharacterPossessed;
			OnCharacterPossessed(PossessionManager.Instance.PossessedCharacter);

			HidePlayerHealth();
			HideBoss();
		}

		private void Update()
		{
			if (_health)
			{
				PlayerHealth.value = _health.Current / _health.Max;
			}

			var p = PossessionManager.Instance;
			PossessionMeter.value = p.CurrentPossessionMeter / p.MaxPossessionMeter;
		}

		private void OnCharacterPossessed(Character character)
		{
			if (character == null)
			{
				// Handle the case when the character is null.
				return;
			}

			if (_lastCharacter != null)
			{
				var lastHealth = _lastCharacter.GetComponent<Health>();
				if (lastHealth != null)
				{
					lastHealth.OnDamaged -= OnDamaged;
					lastHealth.OnReachMaxHealth -= FadeOutPlayerHealth;
				}
			}

			character.TryGetComponent(out _health);
			if (_health != null)
			{
				_health.OnDamaged += OnDamaged;
				_health.OnReachMaxHealth += FadeOutPlayerHealth;

				if (_health.IsCritical)
				{
					FadeInPlayerHealth();
				}
			}

			_lastCharacter = character;
		}

		private void OnDamaged(float damage)
		{
			FadeInPlayerHealth();
		}

		private void FadeInPlayerHealth()
		{
			var renderers = PlayerHealth.GetComponentsInChildren<Image>();

			renderers[0].DOKill();
			renderers[1].DOKill();

			renderers[0].DOFade(1f, PlayerHealthFadeIn).SetEase(Ease.Linear);
			renderers[1].DOFade(1f, PlayerHealthFadeIn).SetEase(Ease.Linear);
		}

		private void FadeOutPlayerHealth()
		{
			var renderers = PlayerHealth.GetComponentsInChildren<Image>();

			renderers[0].DOKill();
			renderers[1].DOKill();

			renderers[0].DOFade(0f, PlayerHealthFadeOut).SetEase(Ease.Linear);
			renderers[1].DOFade(0f, PlayerHealthFadeOut).SetEase(Ease.Linear);
		}

		private void HidePlayerHealth()
		{
			var renderers = PlayerHealth.GetComponentsInChildren<Image>();

			var color = renderers[0].color;
			color.a = 0f;
			renderers[0].color = color;

			color = renderers[1].color;
			color.a = 0f;
			renderers[1].color = color;
		}

		private void ShowBoss()
		{
			
		}

		private void HideBoss()
		{
			
		}
	}
}
