using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MasterVolumeControl : MonoBehaviour
{
    public static Slider volumeSlider;
    public AudioMixer mixer;
    public Slider a;

    void Start()
    {
        volumeSlider = a;
        // Set initial volume
        float value;
        if (mixer.GetFloat("MasterVolume", out value))
        {
            volumeSlider.value = Mathf.Pow(10, value / 20); // convert dB back to 0–1
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        // Convert slider (0–1) to decibels
        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }
}
