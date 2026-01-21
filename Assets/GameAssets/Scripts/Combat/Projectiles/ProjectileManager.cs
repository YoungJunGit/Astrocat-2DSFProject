using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System;

public interface IProjectileManager
{
    public T CreateProjectile<T>(string name, Vector3 position) where T : BaseProjectile;
}

[CreateAssetMenu(fileName = "ProjectileManager", menuName = "Projectile/ProjectileManager")]
public class ProjectileManager : ScriptableObject, IProjectileManager
{
    [SerializedDictionary("Projectile Name", "Projectile")]
    public SerializedDictionary<string, BaseProjectile> projectileDic;

    private Dictionary<Type, string> _suffixDic = new Dictionary<Type, string>()
    {
        { typeof(BaseBullet), "Bullet" },
        { typeof(BaseInjection), "Injection"}
    };

    public T CreateProjectile<T>(string name, Vector3 position) where T : BaseProjectile
    {
        string prefabName = name;
        if (_suffixDic.TryGetValue(typeof(T), out var suffix))
        {
            prefabName += $"_{suffix}";
        }
        else
        {
            throw new Exception($"Undefined Type of suffix: {typeof(T)}");
        }

        if (projectileDic.TryGetValue(prefabName, out var prefab))
        {
            var instance = Instantiate(prefab, position, Quaternion.identity);

            if (instance is T typed)
                return typed;

            Debug.LogError($"Projectile '{prefabName}' is not of type {typeof(T)}");
        }

        return null;
    }
}
