using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class FunctionUtils
    {
        public static bool MakeChance(Vector2 range, int chance)
        {
            int clampedChance = Mathf.Clamp(chance, (int)range.x, (int)range.y);
            return Random.Range(range.x, range.y) < clampedChance ? true : false;
        }

        public static bool MakeChance(Vector2 range, float chance)
        {
            float clampedChance = Mathf.Clamp(chance, range.x, range.y);
            return Random.Range(range.x, range.y) < clampedChance ? true : false;
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
}
