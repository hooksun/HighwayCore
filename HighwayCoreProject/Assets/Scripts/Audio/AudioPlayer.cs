using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    static List<AudioPlayer> ActivePlayers = new List<AudioPlayer>();

    public AudioSource Source;
    public float volume;

    Transform requester;
    bool paused;

    protected virtual bool playing {get => Source.isPlaying;}
    
    void OnEnable()
    {
        ActivePlayers.Add(this);
        paused = false;
    }
    void OnDisable()
    {
        ActivePlayers.Remove(this);
    }

    void Update()
    {
        SetVolume();
        if(requester == null || !requester.gameObject.activeInHierarchy)
        {
            Stop();
            if(!playing)
                gameObject.SetActive(false);
            return;
        }
        transform.position = requester.position;
    }

    public void Setup(Transform req, bool loop, float pitch, AudioClip mainClip = null)
    {
        requester = req;
        Source.loop = loop;
        Source.pitch = pitch;
        Source.clip = mainClip;
    }

    public virtual void Play()
    {
        Source.Play();
    }

    public void PlayClip(AudioClip clip, float volume = 1f, float pitch = -1f)
    {
        if(pitch > 0f)
            Source.pitch = pitch;
        Source.PlayOneShot(clip, volume * Player.ActivePlayer.Settings.settings.volume);
    }

    public virtual void Stop()
    {
        Source.loop = false;
    }

    public static void PauseAll(bool pause)
    {
        foreach(AudioPlayer audio in ActivePlayers)
        {
            audio.Pause(pause);
        }
    }

    public void Pause(bool pause)
    {
        if(pause)
        {
            if(playing)
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

    protected virtual void SetVolume()
    {
        Source.volume = volume * Player.ActivePlayer.Settings.settings.volume;
    }
}