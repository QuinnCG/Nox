using UnityEngine;
using UnityEngine.UI;
using Game.GeneralManagers;  // Assuming AudioManager is in this namespace

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

        // Call this method when UI opens (e.g., Options Menu is activated)
        public void OnUIOpened()
        {
            // Notify AudioManager that UI is open
            if (audioManager != null)
            {
                audioManager.SetUIState(true);
            }
        }

        // Call this method when UI closes (e.g., Options Menu is deactivated)
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
            }
        }

        public void OnSFXVolumeChanged(float sliderValue)
        {
            if (audioManager != null)
            {
                audioManager.SetSFXVolume(sliderValue);
            }
        }

        public void OnMusicVolumeChanged(float sliderValue)
        {
            if (audioManager != null)
            {
                audioManager.SetMusicVolume(sliderValue);
            }
        }

        public void OnAmbienceVolumeChanged(float sliderValue)
        {
            if (audioManager != null)
            {
                audioManager.SetAmbienceVolume(sliderValue);
            }
        }
    }
}
