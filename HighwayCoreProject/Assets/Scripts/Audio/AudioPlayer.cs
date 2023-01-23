using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    static List<AudioPlayer> ActivePlayers = new List<AudioPlayer>();

    public AudioSource Source;
    public float volume;

    Transform requester;
    protected bool paused;

    protected virtual bool playing {get => Source.isPlaying;}
    
    void Start()
    {
        SetVolume();
    }
    
    void OnEnable()
    {
        ActivePlayers.Add(this);
        paused = false;
        if(Player.ActivePlayer != null)
            SetVolume();
    }
    void OnDisable()
    {
        ActivePlayers.Remove(this);
    }

    protected virtual void Update()
    {
        if(requester == null || !requester.gameObject.activeInHierarchy)
        {
            Stop();
            if(!playing)
                gameObject.SetActive(false);
            return;
        }
        transform.position = requester.position;
    }

    public virtual void Setup(Transform req, bool loop, float pitch, AudioClip mainClip = null)
    {
        requester = req;
        Source.loop = loop;
        Source.pitch = pitch;
        Source.clip = mainClip;
    }

    public virtual void Play(float pitch = -1f)
    {
        if(pitch > 0f)
            Source.pitch = pitch;
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

    public virtual void Pause(bool pause)
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
        SetVolume();
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