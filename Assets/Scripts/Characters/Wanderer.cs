using UnityEngine;

namespace Game.Characters
{
	public class Wanderer : Character
	{
		protected override void Update()
		{
			base.Update();
			Movement.Move(Vector2.zero);
		}

		public override void Attack(Vector2 target)
		{
			// TODO: Add attack.
		}
	}
}
