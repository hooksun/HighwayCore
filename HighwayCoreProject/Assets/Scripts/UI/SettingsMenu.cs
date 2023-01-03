using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public PlayerSettings Default, Settings;
    public SliderSetting sensitivity, fov;

    void OnEnable()
    {
        Settings.Load();
        sensitivity.ValueChange(Settings.settings.sensitivity);
        fov.ValueChange(Settings.settings.fov);
        sensitivity.Reset.SetActive(Default.settings.sensitivity != Settings.settings.sensitivity);
        fov.Reset.SetActive(Default.settings.fov != Settings.settings.fov);
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
            slider.value = amount;
            field.text = value.ToString("0.00");
        }
        public void SliderChange()
        {
            field.text = value.ToString("0.00");
        }
        public void FieldChange()
        {
            float val;
            if(float.TryParse(field.text, out val))
            {
                slider.value = Mathf.Clamp(val, slider.minValue, slider.maxValue);
            }
            field.text = value.ToString("0.00");
        }
    }
}