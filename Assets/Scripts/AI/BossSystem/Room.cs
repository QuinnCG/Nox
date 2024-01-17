using UnityEngine;

namespace Game.AI.BossSystem
{
	public class Room : MonoBehaviour
	{
		[SerializeField]
		private Door[] _doors;

		[SerializeField]
		private GameObject BossPrefab;

		[SerializeField]
		private Transform SpawnPoint;

		[SerializeField]
		private Trigger Trigger;

		private Boss _boss;

		private void Awake()
		{
			Trigger.OnTrigger += _ => SpawnBoss();
		}

		private void Start()
		{
			OpenAllDoors();
		}

		private void SpawnBoss()
		{
			var instance = Instantiate(BossPrefab, SpawnPoint.position, Quaternion.identity, transform);
			_boss = instance.GetComponent<Boss>();

			_boss.Health.OnDeath += _ => OnBossDeath();
			CloseAllDoors();
		}

		private void OnBossDeath()
		{
			OpenAllDoors();
		}

		private void OpenAllDoors()
		{
			foreach (var door in _doors)
			{
				door.Open();
			}
		}

		private void CloseAllDoors()
		{
			foreach (var door in _doors)
			{
				door.Close();
			}
		}
	}
}
