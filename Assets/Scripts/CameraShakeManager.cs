using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
	public static CameraShakeManager Instance { get; private set; }

	[SerializeField] private float globalShakeForce = 1f;

	private void Awake()
	{
		Instance = this;
	}

	public void CameraShake(CinemachineImpulseSource impulseSource)
	{
		impulseSource.GenerateImpulseWithForce(globalShakeForce);
	}
}
