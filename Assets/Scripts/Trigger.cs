using Game.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
	public class Trigger : MonoBehaviour
	{
		[SerializeField, EnumToggleButtons]
		private TriggerFilter Filter;

		[Space, SerializeField]
		private UnityEvent<GameObject> OnTriggerEvent;

		public event Action<GameObject> OnTrigger;

		public void OnTriggerEnter(Collider other)
		{
			Debug.Log("1");

			bool isPlayer = other.gameObject == PlayerManager.Instance.PossessedCharacter;
			bool isCharacter = other.TryGetComponent(out Character _);

			if (Filter == TriggerFilter.Any && !isCharacter)
				return;

			Debug.Log("2");

			if (Filter == TriggerFilter.Player && !isPlayer)
				return;

			Debug.Log("3");

			if (Filter == TriggerFilter.Enemy && (isPlayer || !isCharacter))
				return;

			Debug.Log("4");

			Invoke(gameObject);
		}

		private void Invoke(GameObject gameObject)
		{
			Debug.Log("Triggered!");

			OnTriggerEvent?.Invoke(gameObject);
			OnTrigger?.Invoke(gameObject);
		}
	}
}
