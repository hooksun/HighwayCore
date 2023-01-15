using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioContinuous : AudioPlayer
{
    public float fadeSpeed;
    float fade;
    bool fading;

    protected override bool playing {get => base.playing && fade > 0f;}

    public override void Play()
    {
        fade = 1f;
        fading = false;
        base.Play();
    }

    public override void Stop()
    {
        fading = true;
    }

    protected override void SetVolume()
    {
        if(fading && fade > 0f)
        {
            fade -= fadeSpeed * Time.deltaTime;
            if(fade <= 0f)
            {
                fade = 0f;
                Source.Stop();
            }
            Source.volume = volume * fade * Player.ActivePlayer.Settings.settings.volume;
        }
    }
}
