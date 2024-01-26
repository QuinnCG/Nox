using Game.AI.BossSystem;
using Game.Common;
using Game.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.RoomSystem
{
	public class Room : MonoBehaviour
	{
		[SerializeField]
		private Door[] Doors;

		[SerializeField, Required]
		private GameObject BossPrefab;

		[SerializeField, Required]
		private Transform SpawnPoint;

		[SerializeField, Required]
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
			_boss.Room = this;

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
