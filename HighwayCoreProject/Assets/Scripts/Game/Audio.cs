using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioClip clip;
    public AudioType audioType;
    public float pitch = 1f;
    public bool loop, autoPlay;

    protected AudioPlayer player;

    protected virtual void OnEnable()
    {
        player = AudioPool.GetObject((int)audioType);
        player.Setup(transform, loop, pitch, clip);
        if(autoPlay)
            Play();
    }

    public virtual void Play()
    {
        player.PlayClip(clip);
    }
    
    public virtual void Stop()
    {
        player.Stop();
    }
}

public enum AudioType{player, enemyGunshot, enemyFootsteps}