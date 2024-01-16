using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Game
{
	public class FootstepVFXPlayer : MonoBehaviour
	{
		[SerializeField, Required]
		private VisualEffect FootstepVFX;

		private bool _isRightFoot;

		public void PlayFootstepVFX()
		{
			FootstepVFX.SendEvent(_isRightFoot ? "OnRightFootstep" : "OnLeftFootstep");
			_isRightFoot = !_isRightFoot;
		}
	}
}
