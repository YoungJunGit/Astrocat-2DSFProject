using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Utils
{
    public class FunctionUtils
    {
        public static bool MakeChance(Vector2 range, int chance)
        {
            int clampedChance = Mathf.Clamp(chance, (int)range.x, (int)range.y);
            return UnityEngine.Random.Range(range.x, range.y) < clampedChance ? true : false;
        }

        public static bool MakeChance(float chance)
        {
            return UnityEngine.Random.value < Mathf.Clamp01(chance);
        }

        public static T SafeGet<T>(T[] array, int index, T defaultValue = default)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                Debug.LogError("Error : Argument Out Of Range!");
                return defaultValue;
            }
            return array[index];
        }
    }

    public static class TargetExtensions
    {
        public static bool TryGetSingle<TUnit>(this ITarget<TUnit> t, out TUnit value)
        {
            value = default;
            var col = t?.Targets;
            if (col == null) return false;

            if (col is IReadOnlyList<TUnit> list)
            {
                if (list.Count == 1) { value = list[0]; return true; }
                return false;
            }

            using var e = col.GetEnumerator();
            if (!e.MoveNext()) return false;
            var first = e.Current;
            if (e.MoveNext()) return false;
            value = first;
            return true;
        }

        public static TUnit SingleOrDefaultFast<TUnit>(this ITarget<TUnit> t)
        {
            return t.TryGetSingle(out var v) ? v : default;
        }
    }

    public static class RaycastExtensions
    {
        public static bool RaycastMouse<T>(int layerMask, out T obj) where T : Component
        {
            obj = default;

            if (Pointer.current == null || Camera.main == null)
                return false;

            Vector2 screen = Pointer.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(screen);

            var hit = Physics2D.Raycast(mousePos, Vector2.zero, 100.0f, layerMask);
            if (hit.collider == null)
                return false;

            // 1?? 자기 자신
            if (hit.collider.TryGetComponent<T>(out obj))
                return true;

            // 2?? 부모에서 검색
            obj = hit.collider.GetComponentInParent<T>();
            if (obj != null)
                return true;

            // 3?? 자식에서 검색
            obj = hit.collider.GetComponentInChildren<T>();
            return obj != null;
        }

        public static bool RaycastMouse<T>(out T obj) where T : Component
        {
            return RaycastMouse(Physics2D.DefaultRaycastLayers, out obj);
        }
    }
}
