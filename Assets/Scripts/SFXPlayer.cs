using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class SFXPlayer : MonoBehaviour
	{
		public void PlaySound(string name)
		{
			RuntimeManager.PlayOneShot($"event:/{name}", transform.position);
		}
	}
}
