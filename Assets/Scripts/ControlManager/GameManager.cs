using ControlManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton quản lý vòng đời game (Game Loop), chuyển cảnh và khởi tạo.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState gameState;
    public bool isStartLevel;
    public bool isCompletedLevel;

    [Header("Data")]
    public LevelData currentLevelData;
    // Đã chuyển CurrentVocabIndex sang CombatManager quản lý

    [Header("Managers")]
    public BackGroundManager backGroundManager;
    public InputDisplayManager inputDisplayManager;
    public CombatManager combatManager; // Renamed to PascalCase
    public AudioManager audioManager;
    public CutScenesManager CutScenesManager;
    public NewInputManager NewInputManager;


    [Header("Button")]
    public GameObject PlayButton;
    public GameObject RestartButton;
    
    private void Awake()
    {
        // Singleton Implementation
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        gameState = GameState.Menu;
    }

    private void Start()
    {
        RestartButton.SetActive(false);
        // Khởi động vocab đầu tiên nếu cần thiết hoặc đợi lệnh từ UI Menu
        if (currentLevelData != null)
        {
            // CombatManager sẽ tự StartLevel qua sự kiện OnLevelStart
        }
    }



    private void Update()
    {
        if(gameState == GameState.Playing && !isStartLevel) 
        {
            StartLevel();
        }



        if (Input.GetKeyDown(KeyCode.Space) && gameState == GameState.Menu)
        {
            ButtonPlay();
        }
        if (Input.GetKeyDown(KeyCode.Space) && gameState == GameState.CompletedLevel)
        {
           
            ReloadCurrentScene();
        }

        if (gameState == GameState.CompletedLevel)
        {
            RestartButton.SetActive(true);
        }
    }
    
    public void StartLevel()
    {
        GameEvents.OnLevelStart?.Invoke();
        isStartLevel = true;
        // if (currentLevelData != null)
        // {
        //     GameEvents.OnLevelStart?.Invoke(currentLevelData);
        //     isStartLevel = true;
        //     }
        }
    public void LoadLevel(int levelID)
    {
        // Logic load scene hoặc load data level mới
    }

    public void CompletedLevel()
    {
        isCompletedLevel = true;
        GameEvents.OnLevelComplete?.Invoke(true);
    }

    // --- Audio Proxies ---
    public void PlayerSwordEffect()
    {
        if(audioManager) audioManager.PlaySwordSound();
    }
    
    public void PlayerFootStepEffect(bool isPlay)
    {
        if(audioManager) audioManager.PlayFootStep(isPlay);
    }

    public void ButtonPlay()
    {
            audioManager.PlayFightMusic();
        
        gameState = GameState.Playing;
        PlayButton.SetActive(false);
    }
    
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;

        // 1. Hủy instance hiện tại để nó không còn là Singleton sống dai nữa
        // (Lưu ý: Chỉ dùng cách này nếu GameManager của bạn được đặt sẵn trong Scene)
        if (Instance == this) 
        {
            Instance = null; // Xóa tham chiếu static
        }
    
        // 2. Nếu GameManager của bạn có DontDestroyOnLoad, bạn cần Destroy nó
        Destroy(gameObject); 

        // 3. Load lại scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
   
}