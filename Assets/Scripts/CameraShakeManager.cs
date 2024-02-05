using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShakeManager : MonoBehaviour
{
	private CinemachineVirtualCamera virtualCam;
	private CinemachineBasicMultiChannelPerlin perlinNoise;

	private void Awake()
	{
		virtualCam = GetComponent<CinemachineVirtualCamera>();
		perlinNoise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		ResetIntensity();
	}

	public void ShakeCamera(float intensity, float shakeTime)
	{
		perlinNoise.m_AmplitudeGain = intensity;
		StartCoroutine(WaitTime(shakeTime));
	}

	IEnumerator WaitTime(float shakeTime)
	{
		yield return new WaitForSeconds(shakeTime);
		ResetIntensity();
	}

	void ResetIntensity()
	{
		perlinNoise.m_AmplitudeGain = 0;
	}
}
