using FMODUnity;
using UnityEngine;

namespace Game.AnimationSystem
{
	public class SFXPlayer : MonoBehaviour
	{
		public void PlaySound(string name)
		{
			RuntimeManager.PlayOneShot($"event:/SFX/{name}", transform.position);
		}
	}
}
