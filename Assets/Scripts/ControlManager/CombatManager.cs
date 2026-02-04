using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

namespace ControlManager
{
    /// <summary>
    /// Quản lý logic chiến đấu, thời gian tấn công và xử lý kết quả thắng/thua.
    /// Refactored theo GDD mới: 1 Enemy - N Vocabs - Wave System.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private float spawnDistanceX;
        [SerializeField] private float timeDelayReady = 1f;
        [SerializeField] private float globalSpeedMultiplier = 1f;

        
        [Header("Refs")] [SerializeField] private PlayerController playerController;
        [SerializeField] private TextMeshProUGUI scoreText;
        

        // Data Runtime
        private List<EnemyWaveData> _waves;
        public int currentCountMask;
   

        // State Runtime
        public EnemyController CurrentEnemy;
        public GameObject enemyPrefab;
        public CombatState CurrentState;
        public bool isFinisher;
        public bool canMove;
        


        // Timer
        private float _attackTimer;
        private float _attackDuration;
        private float _lastPunishTime; // Chống spam click quá nhanh (0.2s)

        // Flags
        private bool _isLastChancePhase = false; // Cờ đánh dấu đang trong giai đoạn "Cơ hội cuối cùng"

        private void Start()
        {
            UpdateScoreText();
            currentCountMask = 2;
            CurrentState = CombatState.Waiting;
            if (!playerController) playerController = FindAnyObjectByType<PlayerController>();
            GameManager.Instance.inputDisplayManager.LockButton(true);
        }

        private void Update()
        {
            UpdateScoreText();
        }

        private void OnEnable()
        {
            GameEvents.OnLevelStart += SetUpStartLevel;
            GameEvents.OnReadyToFight += OnReadyToFight;
            GameEvents.OnCharCorrect += OnCharCorrect;
            GameEvents.OnCharWrong += OnCharWrong;
            GameEvents.OnSubmitAnswer += OnVocabFinished;
            GameEvents.OnFinisherFail += ExecuteFailFinisher;
            GameEvents.OnPlayerBroken += HandlePlayerDefeated;

            GameEvents.OnEndGame += OnEndGame;

        }

        private void OnDisable()
        {
            GameEvents.OnLevelStart -= SetUpStartLevel;
            GameEvents.OnReadyToFight -= OnReadyToFight;
            GameEvents.OnCharCorrect -= OnCharCorrect;
            GameEvents.OnCharWrong -= OnCharWrong;
            GameEvents.OnSubmitAnswer -= OnVocabFinished;
            GameEvents.OnFinisherFail -= ExecuteFailFinisher;
            GameEvents.OnPlayerBroken -= HandlePlayerDefeated;

 
            GameEvents.OnEndGame -= OnEndGame;
        }
        // --- LEVEL FLOW ---

        private void SetUpStartLevel( )
        {
            _isLastChancePhase = false;
            StartWave();
        }

        private void StartWave()
        {
            canMove = true;
            SpawnEnemy();
        }

        private void SpawnEnemy( )
        {
            Vector2 pos = new Vector2(transform.position.x + spawnDistanceX, transform.position.y);
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            CurrentEnemy = enemy.GetComponent<EnemyController>();
            CurrentEnemy.IncreaseHealth(1);

        }

        private void OnReadyToFight()
        {
            
            CurrentState = CombatState.Readying;
            this.DelayAction(timeDelayReady, () =>
            {
                CurrentState = CombatState.Fighting;
                StartCombatRound();
            });
            playerController.ReadyToFight(timeDelayReady);
            Debug.Log("CombatManager: Ready to fight!");
            canMove = false;
        }

        private void StartCombatRound()
        {
            GameManager.Instance.NewInputManager.RandomSpawnMask(currentCountMask);
            CurrentEnemy.SetActiveFuelSlider(true);
            GameManager.Instance.inputDisplayManager.LockButton(false);
          
            isFinisher = CurrentEnemy.health <=1;
            if (isFinisher)
            {
                SetupFinisherPhase();
            }
            else
            {
                if(playerController.currentState != ActorState.BrokenStand && playerController.currentState != ActorState.Focusing) playerController.FightStand();
            }
        }

        private void SetupFinisherPhase()
        {
             playerController.OnFocusing();
            CurrentEnemy.SetFuelGauge(1.2f);
        }

        // --- INPUT HANDLERS ---

        private void OnCharCorrect()
        {
            if (playerController.CurrentState != ActorState.Focusing)
            {
                CurrentEnemy.NextAttack();
                playerController.DoParry();
                CurrentEnemy.ResetFuelGauge();
            }
            else
            {
                playerController.CancelNextAttack();
                playerController.OnFocus();
            }

        }

        private void OnCharWrong()
        {
            bool isFocusing = playerController.CurrentState == ActorState.Focusing;

            if (isFocusing)
            {
                if ( playerController.CurrentHealth <=1)
                {   
                    ExecuteFailFinisher();
                }
            }
            else
            {
                HandleInputWrong();
            }
        }

        private void HandleInputWrong()
        {
            if (Time.time - _lastPunishTime < 0.2f) return;
            _lastPunishTime = Time.time;

            Debug.Log("CombatManager: HandleInputWrong. Applying IMMEDIATE penalty.");

            CurrentEnemy.NextAttack();
            playerController.DecreaseHealth();
            UpdateMistakes();
        }

        private void UpdateMistakes()
        {
            // Logic cập nhật UI mistake nếu cần
        }

        private void ResetAttackTimer()
        {
            // Reset timer logic if needed (Currently using FuelGauge)
        }

        // --- RESOLUTION ---

        private void HandlePlayerDefeated(bool isNextVocab)
        {
            playerController.ResetAnimatorParameters();
            playerController.PlayerDefeated();
            Debug.Log("Player Broken! Entering Last Chance Phase...");
            playerController.BrokenStand(true);
            _isLastChancePhase = true; // Bật cờ đánh dấu đang trong giai đoạn hồi phục sinh tử
            CurrentEnemy.ResetFuelGauge();
            if(isNextVocab) StartCombatRound();
            
        }

        /// <summary>
        /// Hàm này dùng để hồi phục trạng thái Player khi sang từ vựng mới.
        /// Đã đổi tên từ NextVocab -> ResetPlayerState để tránh nhầm lẫn.
        /// </summary>
     

        private void OnVocabFinished()
        {
          
            
            // Kiểm tra cờ Last Chance
            if (_isLastChancePhase)
            {
                Debug.Log("Last Chance Success! Resetting Wave & Standing Up.");

                _isLastChancePhase = false; // Tắt cờ vì đã thành công
                playerController.BrokenStand(false);
                playerController.ResetHealth();
                playerController.ResetLife();
                StartCombatRound();
                return;
            }

            playerController.ResetHealth();
            CurrentEnemy.HealthDecrease(1);
            bool isBroken = CurrentEnemy.health <= 1;

            if (isFinisher)
            {
                ExecuteWinFinisher();
            }
            else
            {
                if (isBroken)  this.DelayAction(0.2f, () => { CurrentEnemy.BrokenStand(true); });
                StartCombatRound();
            }
        }

        private void ExecuteWinFinisher()
        {
            playerController.ResetAnimatorParameters();
            
            CurrentState = CombatState.CutScene;
            Debug.Log("PERFECT FINISHER!");
            GameManager.Instance.inputDisplayManager.LockButton(true);
            CurrentEnemy.SetActiveFuelSlider(false);
            GameManager.Instance.CutScenesManager.PlayFinisherSuccess(CurrentEnemy);
            playerController.Idle();
            CurrentEnemy.Die();
            canMove = false;
            
            playerController.ResetAnimatorParameters();
            this.DelayAction(2f, KillEnemyAndNextWave);
        }

        private void ExecuteFailFinisher()
        {
            
            CurrentState = CombatState.CutScene;
            Debug.Log("FAILED FINISHER -> RESET FIGHT");
            
            GameManager.Instance.inputDisplayManager.LockButton(true);
            CurrentEnemy.SetActiveFuelSlider(false);
            GameManager.Instance.CutScenesManager.PlayFinisherFail(CurrentEnemy,(() =>
            {
                this.DelayAction(0.2f, StartCombatRound);
                CurrentState = CombatState.Fighting;
                HandlePlayerDefeated(false); // Phuong an tam thoi SOS
                CurrentEnemy.ResetHealth();
            }));
                
                CurrentEnemy.BrokenStand(false);
            
        }

        private void OnEndGame()
        {
            CurrentState = CombatState.Waiting;
            GameManager.Instance.NewInputManager.SetPanelActive(false);
            
            GameManager.Instance.inputDisplayManager.LockButton(true);
            CurrentEnemy.SetActiveFuelSlider(false);
            GameManager.Instance.gameState = GameState.CompletedLevel;
        }
     
        private void KillEnemyAndNextWave()
        {
            if (CurrentEnemy) Destroy(CurrentEnemy.gameObject);
            currentCountMask++;
          playerController.Idle();
           this.DelayAction(2f,() =>
           {
               CurrentState = CombatState.Waiting;
               playerController.Running();
               SetUpStartLevel();
           })  ;
        }

        private void UpdateScoreText()
        {
            scoreText.text = ""+(currentCountMask - 2);
        }
        
        [ContextMenu("Kill enemy")]
        private void AutoCalculateFactors()
        {
            isFinisher = true;
            OnVocabFinished();
        }

    }
}
