using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GeneralManagers
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		private Bus _master, _sfx, _music, _ambience;

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultMasterVolume = 1f;

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultSFXVolume = 1f;

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultMusicVolume = 1f;

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultAmbienceVolume = 1f;

		[SerializeField, BoxGroup("UI")]
		private Slider masterVolumeSlider; // Reference to the Master Volume Slider

		[SerializeField, BoxGroup("UI")]
		private Slider sfxVolumeSlider; // Reference to the SFX Volume Slider

		[SerializeField, BoxGroup("UI")]
		private Slider musicVolumeSlider; // Reference to the Music Volume Slider

		[SerializeField, BoxGroup("UI")]
		private Slider ambienceVolumeSlider; // Reference to the Ambience Volume Slider

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			_master = RuntimeManager.GetBus("bus:/");
			_sfx = RuntimeManager.GetBus("bus:/SFX");
			_music = RuntimeManager.GetBus("bus:/Music");
			_ambience = RuntimeManager.GetBus("bus:/Ambience");

			// Set initial volumes based on slider values
			if (masterVolumeSlider != null)
				SetMasterVolume(masterVolumeSlider.value);

			if (sfxVolumeSlider != null)
				SetSFXVolume(sfxVolumeSlider.value);

			if (musicVolumeSlider != null)
				SetMusicVolume(musicVolumeSlider.value);

			if (ambienceVolumeSlider != null)
				SetAmbienceVolume(ambienceVolumeSlider.value);

			// Subscribe to the slider value change events
			if (masterVolumeSlider != null)
				masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

			if (sfxVolumeSlider != null)
				sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

			if (musicVolumeSlider != null)
				musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

			if (ambienceVolumeSlider != null)
				ambienceVolumeSlider.onValueChanged.AddListener(SetAmbienceVolume);
		}

		private void SetMasterVolume(float sliderValue)
		{
			_master.setVolume(ConvertToLinear(sliderValue));
		}

		private void SetSFXVolume(float sliderValue)
		{
			_sfx.setVolume(ConvertToLinear(sliderValue));
		}

		private void SetMusicVolume(float sliderValue)
		{
			_music.setVolume(ConvertToLinear(sliderValue));
		}

		private void SetAmbienceVolume(float sliderValue)
		{
			_ambience.setVolume(ConvertToLinear(sliderValue));
		}

		private float ConvertToLinear(float value)
		{
			// TODO: Implement conversion logic if needed.
			return value;
		}
	}
}

