using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVaried : Audio
{
    public VariablePool<AudioClip> clips;
    public float clipTime;

    float time;
    bool playing;

    void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        if(playing && time <= 0f)
        {
            Play();
            return;
        }
        if(time < 0f)
            time = 0f;
        if(time > 0f)
            time -= Time.deltaTime;
    }

    protected override void Activate()
    {
        time = 0f;
        playing = false;
        base.Activate();
    }

    public override void Play()
    {
        if(time > 0f)
            return;
        player.PlayClip(clips.GetRandomVar(), volume, pitch);
        time += clipTime;
        if(loop)
        {
            playing = true;
        }
    }

    public override void Stop()
    {
        playing = false;
    }
}
