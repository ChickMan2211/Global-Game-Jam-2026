using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("Game/Environment/Background Manager")]
public class BackGroundManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseRunSpeed = 5f;
    [SerializeField] private List<ParallaxLayer> layers = new();
    [SerializeField] private float acceleration = 5f; // Tốc độ tăng tốc/giảm tốc

    [SerializeField] private ParticleSystem leavesParticle;

    private float _currentGlobalSpeed = 0f; // Tốc độ chạy nền
    public bool _targetRunState = false;
    
    // Knockback variables
    private float _knockbackVelocity = 0f;
    private float _currentKnockbackForce = 0f;
    private float _knockbackTimer = 0f;
    private bool _isKnockingBack = false;

    [Header("Knockback Settings")]
    [Tooltip("Biểu đồ vận tốc đẩy lùi. Nên copy giống hệt Enemy để đồng bộ.")]
    [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); 
    [SerializeField] private float knockbackDuration = 0.3f;

    // THÊM DÒNG NÀY: Biến để test nhanh trong Inspector
    [Header("Debug")]
    [SerializeField] private bool testRunning = false;


    [System.Serializable]
    public struct ParallaxLayer
    {
        public Rigidbody2D body;
        [Range(0f, 1f)] public float parallaxFactor; 
    }

    [ContextMenu("Auto Calculate Parallax Factors")]
    private void AutoCalculateFactors()
    {
        if (layers.Count == 0) return;
        float step = 1f / layers.Count;
        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];
            layer.parallaxFactor = (i + 1) * step; 
            layers[i] = layer;
        }
    }

    private void Start()
    {
        foreach (var layer in layers)
        {
            if (layer.body is { } rb) 
            {
                rb.bodyType = RigidbodyType2D.Kinematic; 
                rb.useFullKinematicContacts = false;
            }
        }
    }

    // Dùng Update để check biến _testRunning liên tục
    private void Update()
    {
        // 1. Xử lý tốc độ chạy nền (Running)
        float targetSpeed = _targetRunState ? baseRunSpeed : 0f;
        _currentGlobalSpeed = Mathf.MoveTowards(_currentGlobalSpeed, targetSpeed, acceleration * Time.deltaTime);

        // 2. Xử lý Knockback theo Curve
        if (_isKnockingBack)
        {
            _knockbackTimer += Time.deltaTime;
            float progress = _knockbackTimer / knockbackDuration;

            if (progress >= 1f)
            {
                _isKnockingBack = false;
                _knockbackVelocity = 0f;
            }
            else
            {
                float curveValue = knockbackCurve.Evaluate(progress);
                _knockbackVelocity = _currentKnockbackForce * curveValue;
            }
        }

        // 3. Áp dụng tổng hợp vận tốc
        ApplyVelocityToLayers();

    
        if (testRunning) SetRunState(true); else SetRunState(false);
    }

    public void TriggerKnockback(float force)
    {
        // Khởi động Knockback (Lực âm để đẩy lùi về sau)
        _currentKnockbackForce = -force;
        _knockbackTimer = 0f;
        _isKnockingBack = true;
    }

    public void StopImmediate()
    {
        _currentGlobalSpeed = 0f;
        _knockbackVelocity = 0f;
        _isKnockingBack = false;
        ApplyVelocityToLayers();
    }

    public void SetRunState(bool isRunning)
    {
        _targetRunState = isRunning;
       // if (isRunning) LeavesForceOverLifetime(baseRunSpeed); else ResetLeavesForceOverLifetime();
    }
    private void ApplyVelocityToLayers()
    {
        Vector2 direction = Vector2.left;

        foreach (var layer in layers)
        {
            if (layer.body is null) continue;

            // Tổng hợp: Tốc độ chạy + Tốc độ Knockback
            float totalSpeed = _currentGlobalSpeed + _knockbackVelocity;
            float finalSpeed = totalSpeed * layer.parallaxFactor;
            
            layer.body.linearVelocity = direction * finalSpeed;
        }
        
    }

    // private void LeavesForceOverLifetime(float speed)
    // {
    //     var forceOverLifetime = leavesParticle.forceOverLifetime;
    //     forceOverLifetime.x = -speed *0.2f;
    // }
    //
    // private void ResetLeavesForceOverLifetime()
    // {
    //     var forceOverLifetime = leavesParticle.forceOverLifetime;
    //     forceOverLifetime.x = -0.1f;
    // }
}