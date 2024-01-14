using FMODUnity;
using UnityEngine;

namespace Game
{
	public class SFXPlayer : MonoBehaviour
	{
		public void PlaySound(string name)
		{
			RuntimeManager.PlayOneShot($"event:/{name}", transform.position);
		}
	}
}
