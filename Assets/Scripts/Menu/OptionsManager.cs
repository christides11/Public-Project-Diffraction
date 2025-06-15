using TightStuff.Menu;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace TightStuff
{
    public class OptionsManager : MonoBehaviour
    {
        public Slider display;
        public Slider musicVolume;
        public Slider menuVolume;
        public Slider effectsVolume;
        public Slider voicesVolume;
        public Slider language;

        public AudioMixer audioMixer;

        // Start is called before the first frame update
        void Start()
        {
            ReflectChangesToPlayerPrefs();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void SaveChanges()
        {
            PlayerPrefs.SetInt("Display", (int)display.value);
            PlayerPrefs.SetInt("MusicVolume", (int)musicVolume.value);
            PlayerPrefs.SetInt("MenuVolume", (int)menuVolume.value);
            PlayerPrefs.SetInt("EffectsVolume", (int)effectsVolume.value);
            PlayerPrefs.SetInt("VoicesVolume", (int)voicesVolume.value);
            PlayerPrefs.SetInt("Language", (int)language.value);
        }
        public void ReflectChangesToPlayerPrefs()
        {
            SetMenuVol(PlayerPrefs.GetInt("MenuVolume", 100));
            SetEffectsVol(PlayerPrefs.GetInt("EffectsVolume", 100));
            SetMusicVol(PlayerPrefs.GetInt("MusicVolume", 100));
            SetVoicesVol(PlayerPrefs.GetInt("VoicesVolume", 100));

            // Base resolution (640x360)
            int baseWidth = 640;
            int baseHeight = 360;

            // Get the maximum screen resolution supported by the user's display
            int maxWidth = Screen.currentResolution.width;
            int maxHeight = Screen.currentResolution.height;

            // Find the largest power of 2 multiple of 640x360 that fits within the screen's resolution
            int width = baseWidth;
            int height = baseHeight;

            while (width * 2 <= maxWidth && height * 2 <= maxHeight)
            {
                width *= 2;
                height *= 2;
            }

            switch (PlayerPrefs.GetInt("Display", 0))
            {
                case 0:
                    Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case 1:
                    Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 2:
                    Screen.SetResolution(width, height, FullScreenMode.Windowed);
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("Language", 0)];
        }

        public void UpdateDisplay()
        {
            display.value = PlayerPrefs.GetInt("Display", 0);
            musicVolume.value = PlayerPrefs.GetInt("MusicVolume", 100);
            menuVolume.value = PlayerPrefs.GetInt("MenuVolume", 100);
            effectsVolume.value = PlayerPrefs.GetInt("EffectsVolume", 100);
            voicesVolume.value = PlayerPrefs.GetInt("VoicesVolume", 100);
            language.value = PlayerPrefs.GetInt("Language", 0);
        }

        public void SetVolume(string parameter, float sliderValue)
        {
            // Convert linear slider (0-1) to logarithmic (-80dB to 0dB)
            float volume = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
            audioMixer.SetFloat(parameter, volume);
        }

        public void SetMenuVol(float sliderValue)
        {
            SetVolume("Menu", sliderValue / 100);
        }
        public void SetEffectsVol(float sliderValue)
        {
            SetVolume("Effects", sliderValue / 100);
        }
        public void SetMusicVol(float sliderValue)
        {
            SetVolume("Music", sliderValue / 100);
        }
        public void SetVoicesVol(float sliderValue)
        {
            SetVolume("Voices", sliderValue / 100);
        }
    }
}
