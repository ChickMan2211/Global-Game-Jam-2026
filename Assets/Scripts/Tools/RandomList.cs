using UnityEngine;
using System.Collections.Generic;


public static class RandomList
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            
            // Modern C# (Tuple Swap): Đổi chỗ 2 phần tử cực gọn
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
