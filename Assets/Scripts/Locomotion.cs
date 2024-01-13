using UnityEngine;

namespace Game
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Locomotion : MonoBehaviour
	{
		public Vector2 Velocity => _rb.velocity;

		private Rigidbody2D _rb;
		private Vector2 _addedVel, _setVel;

		protected virtual void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		protected virtual void LateUpdate()
		{
			var vel = _setVel != Vector2.zero ? _setVel : _addedVel;
			_rb.velocity = vel;

			_addedVel = Vector2.zero;
			_setVel = Vector2.zero;
		}

		public void AddVelocity(Vector2 velocity)
		{
			_addedVel += velocity;
		}

		public void SetVelocity(Vector2 velocity)
		{
			_setVel = velocity;
		}
	}
}
