using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(Image))]
public class UIFrameAnimationV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] breakFrames;

    private Image _targetImage;
    private Coroutine _currentRoutine;

    private void Awake() => _targetImage = GetComponent<Image>();

    private void OnEnable() => PlayIdle();

    public void TriggerBreak(Action onComplete)
    {
        // 1. Kiểm tra dữ liệu
        if (breakFrames == null || breakFrames.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 2. [QUAN TRỌNG] Kiểm tra xem Object có đang Active không?
        // Nếu nó đang tắt (do Panel tắt hoặc Prefab tắt), StartCoroutine sẽ gây Crash game.
        if (!gameObject.activeInHierarchy)
        {
            // Nếu object đang ẩn -> Không cần diễn hoạt làm gì, gọi luôn onComplete
            // Để logic game (xóa mask, cộng điểm) vẫn chạy tiếp bình thường
            onComplete?.Invoke();
            return;
        }

        // 3. Nếu mọi thứ ổn -> Chạy Coroutine
        if (_currentRoutine != null) StopCoroutine(_currentRoutine);
        _currentRoutine = StartCoroutine(PlayOneShot(breakFrames, onComplete));
    }

    public void PlayIdle()
    {
        if (idleFrames == null || idleFrames.Length == 0) return;
        if (_currentRoutine != null) StopCoroutine(_currentRoutine);
        _currentRoutine = StartCoroutine(PlayLoop(idleFrames));
    }

    private IEnumerator PlayOneShot(Sprite[] clips, Action onFinished)
    {
        float waitTime = 1f / frameRate;
        
        // Chạy từ frame 0 đến frame cuối
        for (int i = 0; i < clips.Length; i++)
        {
            _targetImage.sprite = clips[i];
            yield return new WaitForSecondsRealtime(waitTime);
        }

        // --- ĐIỂM QUAN TRỌNG ---
        // Vòng lặp kết thúc, _targetImage.sprite đang giữ cái ảnh cuối cùng của mảng breakFrames.
        // Chúng ta KHÔNG gọi PlayIdle() lại.
        // Chúng ta KHÔNG tắt GameObject.
        // -> Nó sẽ đứng im ở trạng thái vỡ.

        onFinished?.Invoke();
    }

    private IEnumerator PlayLoop(Sprite[] clips)
    {
        float waitTime = 1f / frameRate;
        int index = 0;
        while (true)
        {
            _targetImage.sprite = clips[index];
            index = (index + 1) % clips.Length;
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
}