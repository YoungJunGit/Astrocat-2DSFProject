using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public interface ISoundService
{
    public UniTask PreloadByLabel(string label);
    void PlayEffectSound(string clipName);
    void PlayEffectSound(string clipName, float delaySeconds);
    void PlayBackGround(string clipName, bool loop = true);
    void StopBackGround();
    void SetMasterVolume(float volume);
    void SetSFXVolume(float volume);
    void SetBGMVolume(float volume);

    void Clear();
}
