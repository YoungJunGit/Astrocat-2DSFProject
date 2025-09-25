using UnityEngine;
using System.Collections;

public interface ISoundService
{
    void PlayEffectSound(string clipName);
    
    void PlayBackGround(string clipName, bool loop = true);
    void StopBackGround();
    IEnumerator RestoreBackGround(float delay);
    void SetMasterVolume(float volume);
    void SetSFXVolume(float volume);
    void SetBGMVolume(float volume);

    void Clear();
}
