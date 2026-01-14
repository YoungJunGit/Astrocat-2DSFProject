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

    CancellationTokenSource _cts;

    List<Vector2> _spawnPoints = new();
    List<int> _spawnIndexBag = new();

    List<UniTask> factorTasks = new();

    public override async UniTask PlayEffect()
    {
        _cts = new();
        controller.onStart += () => CreateFactor().Forget();
        controller.onEnd += () => _cts.Cancel();
        
        await UniTask.WaitUntil(() => factorTasks.Count != 0);

        await UniTask.WhenAll(factorTasks);
    }

    private async UniTask CreateFactor()
    {
        BuildSpawnPoints();
        RefillSpawnBag();

        while (!_cts.IsCancellationRequested)
        {
            if (_spawnIndexBag.Count == 0)
                RefillSpawnBag();

            int index = _spawnIndexBag[^1];
            _spawnIndexBag.RemoveAt(_spawnIndexBag.Count - 1);

            Vector2 pos = _spawnPoints[index];
            pos += Random.insideUnitCircle * randomJitter;

            var factor = Instantiate(recoveryFactor, parent);
            factor.transform.position = pos;
            factorTasks.Add(factor.Init());

            await UniTask.WaitForSeconds(delay, cancellationToken: _cts.Token).SuppressCancellationThrow();
        }

        await UniTask.WaitForSeconds(5f);

        Destroy(gameObject);
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

    private void OnDestroy()
    {
        _cts.Cancel();
    }
}
