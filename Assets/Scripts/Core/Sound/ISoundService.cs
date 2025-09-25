using UnityEngine;
using System.Collections;

public interface ISoundService
{
    void PlayEffectSound(string clipName);
    
    void PlayBackGround(string clipName, bool loop = true);
    void StopBackGround();
    void SetMasterVolume(float volume);
    void SetSFXVolume(float volume);
    void SetBGMVolume(float volume);

    void Clear();
}
