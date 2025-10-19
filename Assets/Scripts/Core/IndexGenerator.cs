using Obvious.Soap.Example;
using System.Collections.Generic;
using UnityEngine;

public static class IndexGenerator
{
    /// <summary>
    /// 중심 인덱스를 기준으로 좌우 순회
    /// </summary>
    public static IEnumerable<int> Normal(int centerIndex, int leftCount, int rightCount, int n)
    {
        // center
        yield return centerIndex;

        // left
        for (int i = 1; i <= leftCount; i++)
        {
            int idx = centerIndex - i;
            if (idx < 0) break;
            yield return idx;
        }

        // right
        for (int i = 1; i <= rightCount; i++)
        {
            int idx = centerIndex + i;
            if (idx >= n) break;
            yield return idx;
        }
    }

    /// <summary>
    /// 교차 순회 (좌1→우1→좌2→우2)
    /// </summary>
    public static IEnumerable<int> Alternate(int centerIndex, int leftCount, int rightCount, int n)
    {
        yield return centerIndex;
        int max = Mathf.Max(leftCount, rightCount);

        for (int step = 1; step <= max; step++)
        {
            // left
            if (step <= leftCount)
            {
                int li = centerIndex - step;
                if (li >= 0) yield return li;
            }

            // right
            if (step <= rightCount)
            {
                int ri = centerIndex + step;
                if (ri < n) yield return ri;
            }
        }
    }

    /// <summary>
    /// 왼쪽부터 순회
    /// </summary>
    public static IEnumerable<int> LeftFirst(int centerIndex, int leftCount, int rightCount, int n)
    {
        // left
        for (int i = 1; i <= leftCount; i++)
        {
            int idx = centerIndex - i;
            if (idx < 0) break;
            yield return idx;
        }

        // center
        yield return centerIndex;

        // right
        for (int i = 1; i <= rightCount; i++)
        {
            int idx = centerIndex + i;
            if (idx >= n) break;
            yield return idx;
        }
    }

    /// <summary>
    /// 오른쪽부터 순회
    /// </summary>
    public static IEnumerable<int> RightFirst(int centerIndex, int leftCount, int rightCount, int n)
    {
        // right
        for (int i = 1; i <= rightCount; i++)
        {
            int idx = centerIndex + i;
            if (idx >= n) break;
            yield return idx;
        }

        // center
        yield return centerIndex;

        // left
        for (int i = 1; i <= leftCount; i++)
        {
            int idx = centerIndex - i;
            if (idx < 0) break;
            yield return idx;
        }
    }
}