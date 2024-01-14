using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
	public class CrosshairManager : MonoBehaviour
	{
		public static CrosshairManager Instance { get; private set; }

		[SerializeField, Required]
		private GameObject CrosshairPrefab;

		public Vector2 CurrentPosition => _crosshair.position;

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
		}

		private void OnDisable()
		{
			if (_crosshair)
			{
				Destroy(_crosshair.gameObject);
				_crosshair = null;
			}
		}

		private void Update()
		{
			Vector2 mousePos = Input.mousePosition;
			Vector2 worldPos = _cam.ScreenToWorldPoint(mousePos);

			_crosshair.position = worldPos;
		}
	}
}
