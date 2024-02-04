using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Game.GeneralManagers;

namespace Game.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        // Reference to the AudioManager
        private AudioManager audioManager;

        // Singleton instance of the OptionsMenu
        public static OptionsMenu Instance { get; private set; }

        [Header("UI Sliders")]
        public Slider masterVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider ambienceVolumeSlider;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Make the OptionsMenu persist across scenes
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // Get reference to the AudioManager
            audioManager = AudioManager.Instance;

            // Initialize AudioManager if reference is valid
            if (audioManager != null)
            {
                audioManager.Initialize(); // Initialize audio settings
                LoadVolumeSettings(); // Load volume settings on scene start
            }

            // Add listeners to the sliders
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (ambienceVolumeSlider != null)
            {
                ambienceVolumeSlider.onValueChanged.AddListener(OnAmbienceVolumeChanged);
            }

        }

        // Call this method when UI opens (e.g. Options Menu is activated)
        public void OnUIOpened()
        {
            // Notify AudioManager that UI is open
            if (audioManager != null)
            {
                audioManager.SetUIState(true);
            }
        }

        // Call this method when UI closes (e.g. Options Menu is deactivated)
        public void OnUIClosed()
        {
            // Notify AudioManager that UI is closed
            if (audioManager != null)
            {
                audioManager.SetUIState(false);
            }
        }

        // Add methods to handle UI interactions related to audio settings
        public void OnMasterVolumeChanged(float sliderValue)
        {
            if (audioManager != null)
            {
                audioManager.SetMasterVolume(sliderValue);
                SaveVolumeSetting("MasterVolume", sliderValue);
            }
        }

        public void OnSFXVolumeChanged(float sliderValue)
        {
            if (audioManager != null)
            {
                audioManager.SetSFXVolume(sliderValue);
                SaveVolumeSetting("SFXVolume", sliderValue);
            }
        }

        public void OnMusicVolumeChanged(float sliderValue)
        {
            if (audioManager != null)
            {
                audioManager.SetMusicVolume(sliderValue);
                SaveVolumeSetting("MusicVolume", sliderValue);
            }
        }

        public void OnAmbienceVolumeChanged(float sliderValue)
        {
            if (audioManager != null)
            {
                audioManager.SetAmbienceVolume(sliderValue);
                SaveVolumeSetting("AmbienceVolume", sliderValue);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Initialize AudioManager if reference is valid
            if (audioManager != null)
            {
                audioManager.Initialize(); // Initialize audio settings
                LoadVolumeSettings(); // Load volume settings on scene change
            }
        }

        private void LoadVolumeSettings()
        {
            // Load volume settings from PlayerPrefs or your save system
            // and set the sliders accordingly.
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
                OnMasterVolumeChanged(masterVolumeSlider.value);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
                OnSFXVolumeChanged(sfxVolumeSlider.value);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
                OnMusicVolumeChanged(musicVolumeSlider.value);
            }

            if (ambienceVolumeSlider != null)
            {
                ambienceVolumeSlider.value = PlayerPrefs.GetFloat("AmbienceVolume", 1.0f);
                OnAmbienceVolumeChanged(ambienceVolumeSlider.value);
            }
        }

        private void SaveVolumeSetting(string key, float value)
        {
            // Save volume settings to PlayerPrefs or your save system
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save(); // Remember to save changes
        }
    }
}


