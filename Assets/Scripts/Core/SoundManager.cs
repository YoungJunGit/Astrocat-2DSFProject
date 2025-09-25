using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : Singleton<SoundManager>, ISoundService
{
    private AudioSource m_AudioSource;
    private AudioSource m_EffectSource;

    private Dictionary<string, AudioClip> _clips = new();

    protected SoundManager() { }

    public void Init()
    {
        var sound = new GameObject("SoundManager_Audio");
        //DontDestroyOnLoad(sound);
        m_AudioSource = sound.AddComponent<AudioSource>();
        m_EffectSource = sound.AddComponent<AudioSource>();

        AudioClip[] clips = Resources.LoadAll<AudioClip>("EffectSounds");
        foreach (var clip in clips)
        {
            if (!_clips.ContainsKey(clip.name))
            {
                _clips.Add(clip.name, clip);
            }
        }
        PlayBackGround("Background", true);
    }
    public void PlayEffectSound(string clipName)
    {
        if (_clips.TryGetValue(clipName, out var clip))
        {
            if (m_EffectSource != null)
                m_EffectSource.PlayOneShot(clip);
            m_AudioSource.volume = 0.7f;
            StartCoroutine(RestoreBackGround(clip.length));
        }
        else return;
    }

    public void PlayBackGround(string clipName, bool loop = true) {
        if (_clips.TryGetValue(clipName, out var clip))
        {
            if (m_AudioSource != null)
            {
                m_AudioSource.clip = clip;
                m_AudioSource.loop = loop;
                m_AudioSource.Play();
            }

        }
        else return;
    }
    public IEnumerator RestoreBackGround(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m_AudioSource != null)
            m_AudioSource.volume = 1.0f;
    }

    public void StopBackGround() {
        if (m_AudioSource == null) return;
        m_AudioSource.Stop();
    }

    public void SetMasterVolume(float volume) {
        AudioListener.volume = volume;
    }
    public void SetSFXVolume(float volume) {
        if (m_EffectSource == null) return;
        m_EffectSource.volume = Mathf.Clamp01(volume);
    }
    public void SetBGMVolume(float volume) {
        if (m_AudioSource == null) return;
        m_AudioSource.volume = Mathf.Clamp01(volume);
    }

    public void Clear() {
        _clips.Clear();
    }

}
