using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Game.GeneralManagers
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		private Bus _master, _sfx, _music, _ambience;

		[SerializeField]
		private float DefaultMasterVolume = 1f;

		[SerializeField]
		private float DefaultSFXVolume = 1f;

		[SerializeField]
		private float DefaultMusicVolume = 1f;

		[SerializeField]
		private float DefaultAmbienceVolume = 1f;

		private bool isUIOpen = false;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}
		}

		/// <summary>
		/// Returns true if the event reference was valid and false if it was null.
		/// </summary>
		public static bool PlayOneShot(EventReference reference, Vector2 position = default)
		{
			if (reference.IsNull) return false;
			RuntimeManager.PlayOneShot(reference, position);
			return true;
		}

		// Initialization method to be called by the Options Menu script
		public void Initialize()
		{
			_master = RuntimeManager.GetBus("bus:/");
			_sfx = RuntimeManager.GetBus("bus:/SFX");
			_music = RuntimeManager.GetBus("bus:/Music");
			_ambience = RuntimeManager.GetBus("bus:/Ambience");

			// Set initial volumes based on default values
			SetMasterVolume(DefaultMasterVolume);
			SetSFXVolume(DefaultSFXVolume);
			SetMusicVolume(DefaultMusicVolume);
			SetAmbienceVolume(DefaultAmbienceVolume);
			Debug.Log("AudioManager initialized");
		}

		public void SetMasterVolume(float sliderValue)
		{
			_master.setVolume(ConvertToLinear(sliderValue));
		}

		public void SetSFXVolume(float sliderValue)
		{
			_sfx.setVolume(ConvertToLinear(sliderValue));
		}

		public void SetMusicVolume(float sliderValue)
		{
			_music.setVolume(ConvertToLinear(sliderValue));
		}

		public void SetAmbienceVolume(float sliderValue)
		{
			_ambience.setVolume(ConvertToLinear(sliderValue));
		}

		public float ConvertToLinear(float value)
		{
			// TODO: Implement conversion logic if needed.
			return value;
		}

		// Notify AudioManager when UI opens/closes
		public void SetUIState(bool isOpen)
		{
			isUIOpen = isOpen;
			// If UI is open, update audio settings immediately
			if (isUIOpen)
			{
				UpdateAudioSettings();
			}
		}

		// Update audio settings only if UI is open
		private void UpdateAudioSettings()
		{
			if (isUIOpen)
			{
				SetMasterVolume(DefaultMasterVolume);
				SetSFXVolume(DefaultSFXVolume);
				SetMusicVolume(DefaultMusicVolume);
				SetAmbienceVolume(DefaultAmbienceVolume);
			}
		}
	}
}



