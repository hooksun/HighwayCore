using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioContinuous : AudioPlayer
{
    public float fadeSpeed;
    float fade;
    bool fading;

    protected override bool playing {get => base.playing && fade > 0f;}

    public override void Setup(Transform req, bool loop, float pitch, AudioClip mainClip = null)
    {
        fade = 1f;
        base.Setup(req, loop, pitch, mainClip);
    }

    public override void Play(float pitch = -1f)
    {
        fade = 1f;
        fading = false;
        if(!playing)
            base.Play(pitch);
    }

    public override void Stop()
    {
        fading = true;
    }

    protected override void Update()
    {
        base.Update();
        if(paused)
            return;
        if(fading && fade > 0f)
        {
            fade -= fadeSpeed * Time.deltaTime;
            if(fade <= 0f)
            {
                fading = false;
                fade = 1f;
                Source.Stop();
                return;
            }
            Source.Pause();
            Source.volume = volume * fade * Player.ActivePlayer.Settings.settings.volume;
            Source.UnPause();
            return;
        }
        Source.Pause();
        Source.volume = volume * Player.ActivePlayer.Settings.settings.volume;
        Source.UnPause();
    }
}
