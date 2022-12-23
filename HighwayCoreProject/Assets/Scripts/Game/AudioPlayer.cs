using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    static List<AudioPlayer> ActivePlayers = new List<AudioPlayer>();

    public AudioClip clip;
    public AudioType audioType;

    AudioSource Source;
    bool paused;
    
    void OnEnable()
    {
        ActivePlayers.Add(this);
        paused = false;
        Source = AudioPool.GetObject((int)audioType);
        Source.transform.parent = transform;
        Source.transform.localPosition = Vector3.zero;
    }
    void OnDisable()
    {
        ActivePlayers.Remove(this);
        AudioPool.Return(Source);
    }

    public static void PauseAll(bool pause)
    {
        foreach(AudioPlayer audio in ActivePlayers)
        {
            audio.Pause(pause);
        }
    }

    public void Play()
    {
        Source.PlayOneShot(clip);
    }

    public void Stop()
    {

    }

    void Pause(bool pause)
    {
        if(pause)
        {
            if(Source.isPlaying)
            {
                Source.Pause();
                paused = true;
            }
            return;
        }
        if(paused)
        {
            paused = false;
            Source.UnPause();
        }
    }
}

public enum AudioType{player, enemyGunshot, enemyFootsteps}