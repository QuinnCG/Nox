using Game.Common;
using Game.DamageSystem;
using UnityEngine;

namespace Game.AI.BossSystem
{
	public class Room : MonoBehaviour
	{
		[SerializeField]
		private Door[] Doors;

		[SerializeField]
		private GameObject BossPrefab;

		[SerializeField]
		private Transform SpawnPoint;

		[SerializeField]
		private Trigger Trigger;

		private BossBrain _boss;

		private void Awake()
		{
			Trigger.OnTrigger += _ =>
			{
				if (!_boss)
				{
					SpawnBoss();
				}
			};
		}

		private void Start()
		{
			OpenAllDoors();
		}

		private void SpawnBoss()
		{
			var instance = Instantiate(BossPrefab, SpawnPoint.position, Quaternion.identity, transform);
			_boss = instance.GetComponent<BossBrain>();
			_boss.GetComponent<Health>().OnDeath += _ => OnBossDeath();

			CloseAllDoors();
		}

		private void OnBossDeath()
		{
			OpenAllDoors();
		}

		private void OpenAllDoors()
		{
			foreach (var door in Doors)
			{
				door.Open();
			}
		}

		private void CloseAllDoors()
		{
			foreach (var door in Doors)
			{
				door.Close();
			}
		}
	}
}
