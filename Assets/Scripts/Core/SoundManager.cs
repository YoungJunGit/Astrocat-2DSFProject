using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private AudioSource m_AudioSource;
    private AudioSource m_EffectSource;

    protected SoundManager() { }

}
