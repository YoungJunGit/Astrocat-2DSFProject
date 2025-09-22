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
    }
}
