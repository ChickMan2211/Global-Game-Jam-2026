using System;
using UnityEngine;

public class EnvironmentSpeedControl : MonoBehaviour
{
    // Singleton để các layer dễ dàng truy cập
    public static EnvironmentSpeedControl Instance { get; private set; }

    [Header("Timeline sẽ điều khiển số này")]
    public float GlobalMultiplier = 1f; // 1 = Bình thường, 0 = Dừng, 0.5 = Slow Motion

    public bool active;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   
    }

    private void Update()
    {
        if (active)
        {
        GameManager.Instance.backGroundManager.TriggerKnockback(GlobalMultiplier);
            active = false;
        }
    }

    public void triggerKockB()
    {
        active = true;
    }
}