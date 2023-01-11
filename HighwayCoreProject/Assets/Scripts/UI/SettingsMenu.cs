using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public PlayerSettings Default, Settings;
    public SliderSetting sensitivity, fov, volume;

    void OnEnable()
    {
        Settings.Load();
        sensitivity.ValueChange(Settings.settings.sensitivity);
        fov.ValueChange(Settings.settings.fov);
        volume.ValueChange(Settings.settings.volume);
        sensitivity.Reset.SetActive(Default.settings.sensitivity != Settings.settings.sensitivity);
        fov.Reset.SetActive(Default.settings.fov != Settings.settings.fov);
        volume.Reset.SetActive(Default.settings.volume != Settings.settings.volume);
    }
    
    public void SensitivitySliderChange()
    {
        sensitivity.SliderChange();
        Settings.settings.sensitivity = sensitivity.value;
        sensitivity.Reset.SetActive(Default.settings.sensitivity != Settings.settings.sensitivity);
    }
    public void SensitivityFieldChange()
    {
        sensitivity.FieldChange();
        Settings.settings.sensitivity = sensitivity.value;
        sensitivity.Reset.SetActive(Default.settings.sensitivity != Settings.settings.sensitivity);
    }
    public void SensitivityReset()
    {
        sensitivity.ValueChange(Default.settings.sensitivity);
        Settings.settings.sensitivity = sensitivity.value;
        sensitivity.Reset.SetActive(Default.settings.sensitivity != Settings.settings.sensitivity);
    }

    public void FovSliderChange()
    {
        fov.SliderChange();
        Settings.settings.fov = fov.value;
        fov.Reset.SetActive(Default.settings.fov != Settings.settings.fov);
    }
    public void FovFieldChange()
    {
        fov.FieldChange();
        Settings.settings.fov = fov.value;
        fov.Reset.SetActive(Default.settings.fov != Settings.settings.fov);
    }
    public void FovReset()
    {
        fov.ValueChange(Default.settings.fov);
        Settings.settings.fov = fov.value;
        fov.Reset.SetActive(Default.settings.fov != Settings.settings.fov);
    }

    public void VolumeSliderChange()
    {
        volume.SliderChange();
        Settings.settings.volume = volume.value;
        volume.Reset.SetActive(Default.settings.volume != Settings.settings.volume);
    }
    public void VolumeFieldChange()
    {
        volume.FieldChange();
        Settings.settings.volume = volume.value;
        volume.Reset.SetActive(Default.settings.volume != Settings.settings.volume);
    }
    public void VolumeReset()
    {
        volume.ValueChange(Default.settings.volume);
        Settings.settings.volume = volume.value;
        volume.Reset.SetActive(Default.settings.volume != Settings.settings.volume);
    }

    void OnDisable()
    {
        Settings.Save();
    }

    [System.Serializable]
    public class SliderSetting
    {
        public Slider slider;
        public TMP_InputField field;
        public GameObject Reset;
        public float value{get => slider.value;}

        public void ValueChange(float amount)
        {
            amount -= amount % 0.01f;
            slider.value = amount;
            field.text = value.ToString();
        }
        public void SliderChange()
        {
            slider.value -= value % 0.01f;
            field.text = value.ToString();
        }
        public void FieldChange()
        {
            float val;
            if(float.TryParse(field.text, out val))
            {
                val -= val % 0.01f;
                slider.value = Mathf.Clamp(val, slider.minValue, slider.maxValue);
            }
            field.text = value.ToString();
        }
    }
}