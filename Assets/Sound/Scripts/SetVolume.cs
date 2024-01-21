using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;



namespace Game
{
    public class SetVolume : MonoBehaviour
    {
        public AudioMixer mixer;

        public void musiclevel(float sliderValue)
        {

            mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }


        public void masterlevel(float sliderValue)
        {

            mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void sfxlevel(float sliderValue)
        {

            mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void ambiancelevel(float sliderValue)
        {

            mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        }


    }
}
