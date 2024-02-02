using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.RoomSystem
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]
	public class Door : MonoBehaviour
	{
		[SerializeField, Required]
		private Sprite OpenSprite, ClosedSprite;

		[SerializeField]
		private EventReference OpenSound, CloseSound;

		[SerializeField]
		private bool StartOpen;

		public bool IsOpen { get; private set; }

		private SpriteRenderer _renderer;
		private Collider2D _collider;

		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_collider = GetComponent<Collider2D>();

			if (StartOpen)
			{
				Open(true);
			}
		}

		public void Open(bool supressFX = false)
		{
			if (!IsOpen)
			{
				IsOpen = true;
				_renderer.sprite = OpenSprite;
				_collider.enabled = false;

				if (!supressFX && !OpenSound.IsNull)
				{
					RuntimeManager.PlayOneShot(OpenSound, transform.position);
				}
			}
		}

		public void Close(bool supressFX = false)
		{
			if (IsOpen)
			{
				IsOpen = false;
				_renderer.sprite = ClosedSprite;
				_collider.enabled = true;

				if (!supressFX && !CloseSound.IsNull)
				{
					RuntimeManager.PlayOneShot(CloseSound, transform.position);
				}
			}
		}
	}
}
