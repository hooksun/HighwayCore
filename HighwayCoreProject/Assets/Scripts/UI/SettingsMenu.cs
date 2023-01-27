using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public PlayerSettings Default, Settings;
    public SettingSlider sensitivity, fov, volume, music;

    void OnEnable()
    {
        Settings.Load();
        SaveSystem.settings = Settings;
        sensitivity.DefaultValue = Default.settings.sensitivity;
        sensitivity.ValueChange(Settings.settings.sensitivity);
        fov.DefaultValue = Default.settings.fov;
        fov.ValueChange(Settings.settings.fov);
        volume.DefaultValue = Default.settings.volume;
        volume.ValueChange(Settings.settings.volume);
        music.DefaultValue = Default.settings.music;
        music.ValueChange(Settings.settings.music);
    }
    
    public void SensitivityValueChange(float value)
    {
        Settings.settings.sensitivity = value;
    }

    public void FovValueChange(float value)
    {
        Settings.settings.fov = value;
    }

    public void VolumeValueChange(float value)
    {
        Settings.settings.volume = value;
    }

    public void MusicValueChange(float value)
    {
        Settings.settings.music = value;
    }

    void OnDisable()
    {
        Settings.Save();
    }
}