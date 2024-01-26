using Game.AI.BossSystem;
using Game.Common;
using Game.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.RoomSystem
{
	public class Room : MonoBehaviour
	{
		public static Room Current { get; private set; }

		[SerializeField]
		private Door[] Doors;

		[SerializeField, Required]
		private GameObject BossPrefab;

		[SerializeField, Required]
		private Transform BossSpawnPoint;

		[field: SerializeField, Required]
		public Transform PlayerSpawnPoint { get; private set; }

		private BossBrain _boss;

		private void Awake()
		{
			Current = this;
		}

		private void SpawnBoss()
		{
			var instance = Instantiate(BossPrefab, BossSpawnPoint.position, Quaternion.identity, transform);
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
