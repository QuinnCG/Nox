using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Player
{
	public class CrosshairManager : MonoBehaviour
	{
		public static CrosshairManager Instance { get; private set; }

		[SerializeField, Required]
		private GameObject CrosshairPrefab;

		public Vector2 CurrentPosition => _crosshair != null ? _crosshair.position : Vector2.zero;

		private Camera _cam;
		private Transform _crosshair;

		private void Awake()
		{
			_cam = Camera.main;
			Instance = this;
		}

		private void OnEnable()
		{
			_crosshair = Instantiate(CrosshairPrefab).transform;
			Cursor.visible = false;
		}

		private void OnDisable()
		{
			if (_crosshair)
			{
				Destroy(_crosshair.gameObject);
				_crosshair = null;
			}

			Cursor.visible = true;
		}

		private void Update()
		{
			Vector2 mousePos = Input.mousePosition;
			Vector2 worldPos = _cam.ScreenToWorldPoint(mousePos);

			_crosshair.position = worldPos;
		}

		public Vector2 GetDirectionToCrosshair()
		{
			Vector2 pos = PossessionManager.Instance.PossessedCharacter.transform.position;
			return (CurrentPosition - pos).normalized;
		}
	}
}
