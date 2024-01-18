using Game.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Common
{
	public class Trigger : MonoBehaviour
	{
		[SerializeField, EnumToggleButtons]
		private TriggerFilter Filter;

		[Space, SerializeField]
		private UnityEvent<GameObject> OnTriggerEvent;

		public event Action<GameObject> OnTrigger;

		public void OnTriggerEnter2D(Collider2D collider)
		{
			bool isPlayer = collider.gameObject == PlayerManager.Instance.PossessedCharacter.gameObject;
			bool isCharacter = collider.TryGetComponent(out Character _);

			if (Filter == TriggerFilter.Any && !isCharacter)
				return;

			if (Filter == TriggerFilter.Player && !isPlayer)
				return;

			if (Filter == TriggerFilter.Enemy && (isPlayer || !isCharacter))
				return;

			Invoke(gameObject);
		}

		private void Invoke(GameObject gameObject)
		{
			OnTriggerEvent?.Invoke(gameObject);
			OnTrigger?.Invoke(gameObject);
		}
	}
}
