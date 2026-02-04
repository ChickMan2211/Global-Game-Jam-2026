using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class CoroutineExtensions
{
    // 1. Tạo kho chứa (Dictionary) để lưu các mốc thời gian đã tạo
    // Static readonly đảm bảo list này tồn tại suốt đời game
    private static readonly Dictionary<float, WaitForSeconds> _waitDictionary = new Dictionary<float, WaitForSeconds>();

    /// <summary>
    /// Hàm lấy WaitForSeconds từ trong kho ra (Không tạo rác mới)
    /// </summary>
    public static WaitForSeconds GetWait(float time)
    {
        if (_waitDictionary.TryGetValue(time, out var wait)) return wait;

        // Nếu chưa có thì mới tạo mới và lưu vào kho
        _waitDictionary[time] = new WaitForSeconds(time);
        return _waitDictionary[time];
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static Coroutine DelayAction(this MonoBehaviour mono, float delayTime, Action action)
    {
        // Check an toàn đầu vào
        if (!mono || !mono.gameObject.activeInHierarchy) return null;
        
        // Truyền 'mono' vào để check lại sau khi delay xong
        return mono.StartCoroutine(ExecuteDelay(mono, delayTime, action));
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator ExecuteDelay(MonoBehaviour mono, float time, Action action)
    {
        // 2. Dùng hàm GetWait thay vì 'new WaitForSeconds' -> HẾT EXPENSIVE
        yield return GetWait(time);
        
        // 3. Check an toàn: Sau khi ngủ dậy, liệu object còn sống không?
        if (mono && mono.gameObject.activeInHierarchy)
        {
            action?.Invoke();
        }
    }
}