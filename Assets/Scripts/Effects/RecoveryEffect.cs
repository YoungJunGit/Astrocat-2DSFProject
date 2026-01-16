using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class RecoveryEffect : BaseEffect
{
    [SerializeField] RecoveryEffectController controller;
    [SerializeField] RecoveryFactor recoveryFactor;
    [SerializeField] Transform parent;
    [SerializeField] BoxCollider2D spawnBox;

    [SerializeField] float delay = 0.2f;
    [SerializeField] int spawnPointCount = 6;
    [SerializeField] float randomJitter = 0.05f;

    List<Vector2> _spawnPoints = new();
    List<int> _spawnIndexBag = new();

    private int _runningFactorCount = 0;
    private bool _isSpawningFinished = false;

    public override async UniTask<BaseEffect> PlayEffect()
    {
        controller.onStart += () => CreateFactor().Forget();
        controller.onEnd += () => _isSpawningFinished = true;

        await UniTask.WaitUntil(() => _isSpawningFinished && _runningFactorCount == 0);

        return this;
    }

    private async UniTask CreateFactor()
    {
        BuildSpawnPoints();
        RefillSpawnBag();

        while (!_isSpawningFinished)
        {
            if (_spawnIndexBag.Count == 0)
                RefillSpawnBag();

            int index = _spawnIndexBag[^1];
            _spawnIndexBag.RemoveAt(_spawnIndexBag.Count - 1);

            Vector2 pos = _spawnPoints[index];
            pos += Random.insideUnitCircle * randomJitter;

            var factor = Instantiate(recoveryFactor, parent);
            factor.transform.position = pos;

            Interlocked.Increment(ref _runningFactorCount);
            factor.Init().ContinueWith(() => Interlocked.Decrement(ref _runningFactorCount)).Forget();

            await UniTask.WaitForSeconds(delay);
        }
    }

    void BuildSpawnPoints()
    {
        _spawnPoints.Clear();

        var bounds = spawnBox.bounds;
        float step = bounds.size.x / spawnPointCount;

        for (int i = 0; i < spawnPointCount; i++)
        {
            float x = bounds.min.x + step * (i + 0.5f);
            float y = bounds.min.y;

            _spawnPoints.Add(new Vector2(x, y));
        }
    }

    void RefillSpawnBag()
    {
        _spawnIndexBag.Clear();

        for (int i = 0; i < _spawnPoints.Count; i++)
            _spawnIndexBag.Add(i);

        // Fisher?Yates Shuffle
        for (int i = 0; i < _spawnIndexBag.Count; i++)
        {
            int rand = Random.Range(i, _spawnIndexBag.Count);
            (_spawnIndexBag[i], _spawnIndexBag[rand]) = (_spawnIndexBag[rand], _spawnIndexBag[i]);
        }
    }
}
