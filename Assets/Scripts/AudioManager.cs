using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultMasterVolume = 1f;

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultSFXVolume = 1f;

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultMusicVolume = 1f;

		[SerializeField, BoxGroup("Defaults")]
		private float DefaultAmbienceVolume = 1f;

		public float MasterVolume
		{
			get
			{
				_master.getVolume(out float vol);
				return vol;
			}
		}
		public float SFXVolume
		{
			get
			{
				_sfx.getVolume(out float vol);
				return vol;
			}
		}
		public float MusicVolume
		{
			get
			{
				_music.getVolume(out float vol);
				return vol;
			}
		}
		public float AmbienceVolume
		{
			get
			{
				_ambience.getVolume(out float vol);
				return vol;
			}
		}

		private Bus _master, _sfx, _music, _ambience;

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

			SetMasterVolume(DefaultMasterVolume);
			SetSFXVolume(DefaultSFXVolume);
			SetMusicVolume(DefaultMusicVolume);
			SetAmbienceVolume(DefaultAmbienceVolume);
		}

		/// <summary>
		/// Set's the master volume for the audio engine.
		/// </summary>
		/// <param name="percent">0-1 range.</param>
		public void SetMasterVolume(float percent)
		{
			_master.setVolume(ConvertToLinear(percent));
		}

		/// <summary>
		/// Set's the SFX volume for the audio engine.
		/// </summary>
		/// <param name="percent">0-1 range.</param>
		public void SetSFXVolume(float percent)
		{
			_sfx.setVolume(ConvertToLinear(percent));
		}

		/// <summary>
		/// Set's the music volume for the audio engine.
		/// </summary>
		/// <param name="percent">0-1 range.</param>
		public void SetMusicVolume(float percent)
		{
			_music.setVolume(ConvertToLinear(percent));
		}

		/// <summary>
		/// Set's the ambience volume for the audio engine.
		/// </summary>
		/// <param name="percent">0-1 range.</param>
		public void SetAmbienceVolume(float percent)
		{
			_ambience.setVolume(ConvertToLinear(percent));
		}

		private float ConvertToLinear(float value)
		{
			// TODO: Convert to linear.
			return value;
		}
	}
}
