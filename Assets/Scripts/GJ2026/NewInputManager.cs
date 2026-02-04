using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using System.Collections;

[System.Serializable]
public class MaskConfig
{
    public char keyChar;
    public MaskItem maskPrefab;
}

public class NewInputManager : MonoBehaviour
{
    [Header("Cấu hình")]
    [SerializeField] private List<MaskConfig> maskConfigs;
    [SerializeField] private Transform parentPanel;
    [SerializeField] private List<GameObject> panels;

    [Header("Settings")]
    [SerializeField] private float inputCooldown = 0.05f;

    [Header("Runtime State")]
    private Queue<MaskItem> _activeMasks = new Queue<MaskItem>();
    private float _lastInputTime;
    
    // Cờ chặn input khi đang reset màn chơi
    private bool _isSpawning = false; 

    private void OnEnable()
    {
        // Đăng ký sự kiện
        GameEvents.OnEndGame += EndGame;
        if (Keyboard.current != null)
            Keyboard.current.onTextInput += OnTextInput;
    }

    private void OnDisable()
    {
        GameEvents.OnEndGame -= EndGame;
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= OnTextInput;
    }

    // --- HÀM UPDATE BẠN YÊU CẦU ---
    private void Update()
    {
        // Kiểm tra null an toàn trước khi truy cập
        if (GameManager.Instance != null && GameManager.Instance.combatManager != null)
        {
            // Chỉ hiện Panel khi đang đánh nhau (Fighting)
            bool isFighting = GameManager.Instance.combatManager.CurrentState == CombatState.Fighting;
            SetPanelActive(isFighting);
        }
    }
    // -----------------------------

    private void OnTextInput(char inputChar)
    {
        // ... (Các đoạn check Panel active, _isSpawning, char.IsControl giữ nguyên) ...
        if (panels.Count > 0 && !panels[0].activeSelf) return;
        if (_isSpawning) return;
        if (char.IsControl(inputChar) || char.IsWhiteSpace(inputChar)) return;
        if (Time.time - _lastInputTime < inputCooldown) return;
        _lastInputTime = Time.time;
        if (_activeMasks.Count == 0) return;

        MaskItem currentTarget = _activeMasks.Peek();
        if (currentTarget == null) { _activeMasks.Dequeue(); return; }

        char inputLower = char.ToLower(inputChar);
        char targetLower = char.ToLower(currentTarget.TargetChar);

        // --- LOG DEBUG CHI TIẾT ---
        // In ra tên object và Instance ID để xem có phải con trên màn hình không
        Debug.Log($"Input: '{inputLower}' | Target: '{targetLower}' | Obj Name: {currentTarget.name} (ID: {currentTarget.GetInstanceID()})");

        if (inputLower == targetLower)
        {
            HandleCorrectInput();
        }
        else
        {
            Debug.LogWarning($"❌ SAI! Logic đang đợi '{targetLower}' (ID: {currentTarget.GetInstanceID()}) nhưng nhận '{inputLower}'");
            HandleWrongInput();
        }
    }


    // Đổi từ void sang IEnumerator
    public void RandomSpawnMask(int count)
    {
        // Gọi Coroutine
        StartCoroutine(SpawnMaskRoutine(count));
    }

    private IEnumerator SpawnMaskRoutine(int count)
    {
        // 1. KHÓA INPUT
        _isSpawning = true;

        // 2. DỌN DẸP VISUAL (HÌNH ẢNH)
        // Dùng vòng lặp while để chắc chắn xóa không còn một mống nào
        while (parentPanel.childCount > 0)
        {
            DestroyImmediate(parentPanel.GetChild(0).gameObject);
        }

        // 3. DỌN DẸP LOGIC (DỮ LIỆU)
        _activeMasks.Clear();
        _lastInputTime = 0;

        // --- ĐIỂM KHÁC BIỆT QUAN TRỌNG NHẤT ---
        // Đợi 1 frame để Unity cập nhật lại Hierarchy và xóa sạch bộ đệm Input
        yield return null; 
        // ---------------------------------------

        // 4. SPAWN MỚI
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, maskConfigs.Count);
            MaskConfig config = maskConfigs[randomIndex];

            if (config.maskPrefab == null) continue;

            MaskItem newMask = Instantiate(config.maskPrefab, parentPanel);
            newMask.Setup(config.keyChar);

            _activeMasks.Enqueue(newMask);
        }
        
        // Debug để kiểm tra xem con đầu tiên thực sự là con gì
        if (_activeMasks.Count > 0)
        {
            Debug.Log($"<color=cyan>Hàng đợi mới: Đầu tiên là '{_activeMasks.Peek().TargetChar}'</color>");
        }

        // 5. MỞ INPUT
        _isSpawning = false;
    }
    private void HandleCorrectInput()
    {
        if (_activeMasks.Count == 0) return;

        MaskItem removedMask = _activeMasks.Dequeue();
        
        // Gọi hiệu ứng vỡ
        if(removedMask != null) removedMask.Break(); 

        if (_activeMasks.Count == 0)
        {
            Debug.Log("Finished Word!");
            GameEvents.OnCharCorrect?.Invoke();
            GameEvents.OnSubmitAnswer?.Invoke();
        }
        else
        {
            GameEvents.OnCharCorrect?.Invoke();
        }
    }

    private void HandleWrongInput()
    {
        GameEvents.OnCharWrong?.Invoke();
    }

    public void SetPanelActive(bool isActive)
    {
        foreach (var p in panels)
        {
            // Tối ưu: Chỉ gọi SetActive khi trạng thái thực sự thay đổi
            if (p != null && p.activeSelf != isActive) 
                p.SetActive(isActive);
        }
    }

    private void EndGame()
    {
        SetPanelActive(false);
    }
}