using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSequence : Audio
{
    public float delay;
    public AudioTime[] sequence;

    float time;
    int index;
    bool playing;

    protected override void Activate()
    {
        time = 0f;
        playing = false;
        index = 0;
        base.Activate();
    }

    void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        if(playing && time <= 0f)
        {
            PlaySequence();
            return;
        }

        if(time < 0f)
            time = 0f;
        if(time > 0f)
            time -= Time.deltaTime;
    }

    public override void Play()
    {
        playing = true;
        index = 0;
        time = delay;
        PlaySequence();
    }
    void PlaySequence()
    {
        if(time > 0f)
            return;

        player.PlayClip(sequence[index].clip, volume, pitch);
        time += sequence[index].time;
        index++;
        if(index >= sequence.Length)
        {
            index = 0;
            playing = loop;
            time += delay;
        }
    }

    public override void Stop()
    {
        playing = false;
    }
}

[System.Serializable]
public struct AudioTime
{
    public AudioClip clip;
    public float time;
}