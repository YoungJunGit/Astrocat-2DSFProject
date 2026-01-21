using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SoundService : ISoundService
{
    private GameObject _root;
    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private readonly Dictionary<string, AsyncOperationHandle<AudioClip>> _clipHandles
        = new Dictionary<string, AsyncOperationHandle<AudioClip>>();

    private float _masterVolume = 1f;
    private float _sfxVolume = 1f;
    private float _bgmVolume = 1f;

    // BGM 로딩 경쟁 상태 방지용 버전 토큰
    private int _bgmLoadVersion = 0;

    // 오디오 딜레이 방지용 메소드 (필요한 오디오 캐싱)
    public async UniTask PreloadByLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            return;

        EnsureInitialized();

        // 1️⃣ Label에 포함된 모든 Location 확보
        var locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(AudioClip));
        var locations = await locationsHandle.Task;

        // 2️⃣ Addressables Address 기준으로 개별 로드
        foreach (var location in locations)
        {
            string address = location.PrimaryKey; // ⭐ Addressable Name

            if (_clipHandles.ContainsKey(address))
                continue;

            var clipHandle = Addressables.LoadAssetAsync<AudioClip>(location);
            var clip = await clipHandle.Task;

            if (clipHandle.Status != AsyncOperationStatus.Succeeded || clip == null)
            {
                Debug.LogWarning($"[SoundService] 프리로드 실패: {address}");
                if (clipHandle.IsValid())
                    Addressables.Release(clipHandle);
                continue;
            }

            // ✅ Addressable Name으로 캐싱
            _clipHandles[address] = clipHandle;
        }

        if (locationsHandle.IsValid())
            Addressables.Release(locationsHandle);
    }

    public void PlayEffectSound(string clipName)
    {
        // 인터페이스 준수: 기존 시그니처는 0초 지연으로 재생
        PlayEffectSound(clipName, 0f);
    }

    public void PlayEffectSound(string clipName, float delaySeconds)
    {
        // fire-and-forget
        UniTask.Void(async () =>
        {
            EnsureInitialized();

            var clip = await LoadClipAsync(clipName);
            if (clip == null)
            {
                Debug.LogWarning($"[SoundService] 효과음 로드 실패: {clipName}");
                return;
            }

            if (_root == null) EnsureInitialized();

            if (delaySeconds > 0f)
            {
                await UniTask.Delay(Mathf.Max(0, Mathf.RoundToInt(delaySeconds * 1000f)));
            }

            _sfxSource.PlayOneShot(clip);
            SetBGMVolume(0.7f);
            await RestoreBackground(clip.length);

        });
    }

    public void PlayBackGround(string clipName, bool loop = true)
    {
        // fire-and-forget
        UniTask.Void(async () =>
        {
            EnsureInitialized();

            // 경쟁 방지용 버전 증가
            int version = ++_bgmLoadVersion;

            var clip = await LoadClipAsync(clipName);
            // 다른 호출이 선행되었으면 무시
            if (version != _bgmLoadVersion)
                return;

            if (clip == null)
            {
                Debug.LogWarning($"[SoundService] 배경음 로드 실패: {clipName}");
                return;
            }

            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
            {
                _bgmSource.loop = loop;
                return;
            }

            _bgmSource.Stop();
            _bgmSource.clip = clip;

            // 🔥 반드시 한 프레임 대기
            await UniTask.NextFrame(PlayerLoopTiming.Update);

            _bgmSource.loop = loop;
            _bgmSource.Play();
        });
    }

    public void StopBackGround()
    {
        // 이후 로딩 결과가 적용되지 않도록 버전 증가
        _bgmLoadVersion++;

        if (_bgmSource == null) return;
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    private async UniTask RestoreBackground(float seconds)
    {
        await UniTask.Delay(Mathf.Max(0, Mathf.RoundToInt(seconds * 1000f)));

        if (_bgmSource != null) SetBGMVolume(1.0f);
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void Clear()
    {
        // 이후 비동기 결과가 반영되지 않게
        _bgmLoadVersion++;

        // 재생 중지
        if (_bgmSource != null)
        {
            _bgmSource.Stop();
            _bgmSource.clip = null;
        }

        if (_sfxSource != null)
        {
            _sfxSource.Stop();
        }

        // Addressables 핸들 해제
        foreach (var kv in _clipHandles)
        {
            var handle = kv.Value;
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        _clipHandles.Clear();

        // 루트 파괴
        if (_root != null)
        {
            Object.Destroy(_root);
            _root = null;
            _bgmSource = null;
            _sfxSource = null;
        }
    }

    // 내부 유틸리티
    private void EnsureInitialized()
    {
        if (_root == null)
        {
            _root = new GameObject("SoundService_AudioRoot");
            _root.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(_root);
        }

        if (_bgmSource == null)
        {
            _bgmSource = _root.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.volume = 1f;
        }

        if (_sfxSource == null)
        {
            _sfxSource = _root.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
            _sfxSource.volume = 1f;
        }

        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (_sfxSource != null) _sfxSource.volume = _masterVolume * _sfxVolume;
        if (_bgmSource != null) _bgmSource.volume = _masterVolume * _bgmVolume;
    }

    // Addressables 비동기 로드 (키 = clipName)
    
    private async UniTask<AudioClip> LoadClipAsync(string clipName)
    {
        if (string.IsNullOrWhiteSpace(clipName))
            return null;

        if (_clipHandles.TryGetValue(clipName, out var cached))
        {
            if (cached.IsValid() && cached.Status == AsyncOperationStatus.Succeeded && cached.Result != null)
            {
                return cached.Result;
            }

            // 유효하지 않거나 실패한 핸들은 정리
            if (cached.IsValid())
                Addressables.Release(cached);

            _clipHandles.Remove(clipName);
        }

        var handle = Addressables.LoadAssetAsync<AudioClip>(clipName);

        // handle.Task은 Task<T>이므로 await 가능
        var clip = await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || clip == null)
        {
            Debug.LogError($"[SoundService] Addressables 로드 실패: {clipName} ({handle.OperationException?.Message})");
            if (handle.IsValid())
                Addressables.Release(handle);
            return null;
        }

        _clipHandles[clipName] = handle;
        return clip;
    }
}