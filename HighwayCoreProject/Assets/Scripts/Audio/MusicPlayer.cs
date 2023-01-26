using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : AudioPlayer
{
    public override void Pause(bool pause)
    {

    }

    protected override void Update()
    {
        Source.Pause();
        SetVolume();
        Source.UnPause();
    }

    protected override void SetVolume()
    {
        Source.volume = volume * SaveSystem.settings.settings.music;
    }
}
