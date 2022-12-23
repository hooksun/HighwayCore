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
        if(!loop || !playing)
            return;
        
        if(time <= 0f)
        {
            Play();
            return;
        }
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
        player.PlayClip(clips.GetRandomVar());
        if(loop)
        {
            time = (time < 0f? time + clipTime : clipTime);
            playing = true;
        }
    }

    public override void Stop()
    {
        playing = false;
        time = 0f;
    }
}
