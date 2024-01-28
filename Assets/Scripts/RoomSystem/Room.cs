using FMOD.Studio;
using FMODUnity;
using Game.AI.BossSystem;
using Game.DamageSystem;
using Game.Player;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RoomSystem
{
	public class Room : MonoBehaviour
	{
		public static Room Current { get; private set; }

		[SerializeField, Required]
		private Door ExitDoor;

		[SerializeField, Required]
		private GameObject BossPrefab;

		[SerializeField, Required]
		private Transform BossSpawnPoint;

		[field: SerializeField, Required]
		public Transform PlayerSpawnPoint { get; private set; }

		[SerializeField, Required]
		private Collider2D _collider;

		[SerializeField]
		private EventReference BossMusic;

		public event Action OnBossDeath;

		public BossBrain Boss { get; private set; }

		private EventInstance _bossMusic;
		private bool _exiting;

		private void Awake()
		{
			Current = this;
		}

		private void Start()
		{
			if (!BossMusic.IsNull)
			{
				_bossMusic = RuntimeManager.CreateInstance(BossMusic);
				_bossMusic.start();
			}

			SpawnBoss();
		}

		private void FixedUpdate()
		{
			if (_exiting) return;

			var collisions = new List<Collider2D>();
			_collider.Overlap(collisions);

			foreach (var collision in collisions)
			{
				if (collision.gameObject == PossessionManager.Instance.PossessedCharacter.gameObject)
				{
					_exiting = true;
					RoomManager.Instance.Next();

					break;
				}
			}
		}

		private void SpawnBoss()
		{
			var instance = Instantiate(BossPrefab, BossSpawnPoint.position, Quaternion.identity, transform);
			Boss = instance.GetComponent<BossBrain>();
			Boss.GetComponent<Health>().OnDeath += _ => BossDeath();
			Boss.Room = this;

			ExitDoor.Close();
		}

		private void BossDeath()
		{
			if (_bossMusic.isValid())
			{
				_bossMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}

			ExitDoor.Open();
			OnBossDeath?.Invoke();
		}
	}
}
