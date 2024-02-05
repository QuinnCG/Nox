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

		[SerializeField]
		private bool NotBossRoom;

		[SerializeField, Required]
		private Door EntranceDoor;

		[SerializeField, Required]
		private Door ExitDoor;

		[SerializeField, Required, HideIf(nameof(NotBossRoom))]
		private GameObject BossInstance;

		[field: SerializeField, Required]
		public Transform PlayerSpawnPoint { get; private set; }

		[SerializeField, Required, HideIf(nameof(NotBossRoom))]
		private Collider2D ExitTrigger;

		[field: SerializeField, Required]
		public PolygonCollider2D CameraBounds { get; private set; }

		[SerializeField]
		private EventReference BossMusic;

		public event Action OnBossDeath;

		public BossBrain Boss { get; private set; }
		public bool HasStarted { get; private set; }

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

			ExitDoor.Close(true);

			if (!NotBossRoom)
			{
				InitializeBoss();
			}
		}

		private void FixedUpdate()
		{
			if (_exiting || ExitTrigger == null) return;
			if (PossessionManager.Instance.PossessedCharacter == null) return;

			var collisions = new List<Collider2D>();
			ExitTrigger.Overlap(collisions);

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

		public void PlayerEnterRoom()
		{
			if (!HasStarted)
			{
				HasStarted = true;

				EntranceDoor.Close();

				if (!NotBossRoom)
				{
					Boss.OnPlayerEnter();
				}
			}
		}

		public void StopBossMusic()
		{
			if (_bossMusic.isValid())
			{
				_bossMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}

		private void InitializeBoss()
		{
			Boss = BossInstance.GetComponent<BossBrain>();
			Boss.GetComponent<Health>().OnDeath += _ => BossDeath();
			Boss.Room = this;
		}

		private void BossDeath()
		{
			StopBossMusic();
			ExitDoor.Open();
			OnBossDeath?.Invoke();
		}
	}
}
