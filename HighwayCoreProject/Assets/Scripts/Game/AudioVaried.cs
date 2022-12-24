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
        if(loop && playing && time <= 0f)
        {
            Play();
            return;
        }
        if(time < 0f)
            time = 0f;
        if(time > 0f)
            time -= Time.deltaTime;
    }

    protected override void OnEnable()
    {
        time = 0f;
        playing = false;
        base.OnEnable();
    }

    public override void Play()
    {
        if(time > 0f)
            return;
        player.PlayClip(clips.GetRandomVar(), pitch);
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
