using UnityEngine;
using UnityEngine.UI;

namespace ControlManager
{
    public class EnemyController : Actor
    {
        [Header("Enemy Config")] 
        public int maxHealth;
        public int health;
        public float FuelGauge;
        public int speed;
    
        [Header("Detection Settings")]
        [SerializeField] private float rayDistance = 5f; 
        [SerializeField] private LayerMask detectionLayer; 
        [SerializeField] private Vector2 rayOffset = new Vector2(0, 0.5f);
        [SerializeField] private float brokenDuration;
        [SerializeField] private bool IsFuelGausePause;

        [Header("UI References")]
        [SerializeField] private Slider _fuelGaugeSlider;
        
        [Header("Debug")]
        [SerializeField] private float currentFuelGauge;
   
        
    
        // State
        public bool HasPlayerDetected;

        // Components & Cache
        public Rigidbody2D _rb2d;
        private BackGroundManager _backgroundManager;
        private CombatManager _combatManager;
        
        private readonly RaycastHit2D[] _rayResults = new RaycastHit2D[1];

        protected override void Awake()
        {
            base.Awake();
            _rb2d = GetComponent<Rigidbody2D>();
        }

        protected override void Start()
        {
            health =maxHealth;
            base.Start();
            currentFuelGauge = FuelGauge;
            if (_fuelGaugeSlider)
            {
                _fuelGaugeSlider.maxValue = FuelGauge;
                _fuelGaugeSlider.value = currentFuelGauge;
            }

            SetActiveFuelSlider(false);

            // Cache reference ngay từ đầu, tránh gọi FindObject mỗi frame
            if (GameManager.Instance != null)
            {
                _backgroundManager = GameManager.Instance.backGroundManager;
                _combatManager = GameManager.Instance.combatManager;
            }
        }
    
        // Knockback Physics variables
        private float _knockbackVelocity;
        private float _currentKnockbackForce; // Lực gốc ban đầu
        private float _knockbackTimer;
        private bool _isKnockingBack;

        [Header("Knockback Settings")]
        [Tooltip("Biểu đồ vận tốc theo thời gian. Trục Y: 1->0. Trục X: Time.")]
        [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); 
        [SerializeField] private float knockbackDuration = 0.3f; // Thời gian hiệu ứng đẩy lùi


        private void Update()
        {
            CheckForPlayer();
            FuelGaugeDercease();
            // Xử lý hiệu ứng đẩy lùi bằng Animation Curve
            if (_isKnockingBack)
            {
                _knockbackTimer += Time.deltaTime;
                float progress = _knockbackTimer / knockbackDuration;

                if (progress >= 1f)
                {
                    // Kết thúc Knockback
                    _isKnockingBack = false;
                    _knockbackVelocity = 0f;
                }
                else
                {
                    // Tính vận tốc dựa trên Curve: Lực gốc * Giá trị Curve tại thời điểm t
                    float curveValue = knockbackCurve.Evaluate(progress);
                    _knockbackVelocity = _currentKnockbackForce * curveValue;

                    // Di chuyển
                    transform.position += Vector3.right * (_knockbackVelocity * Time.deltaTime);

                    // Kiểm tra player qua xa 
                    if (!HasPlayerDetected)
                    {
                        _isKnockingBack = false;
                        _knockbackVelocity = 0f;
                    }
                }
            }

            if (!HasPlayerDetected && GameManager.Instance.combatManager.canMove)
            {
                Moving(true);
            }
            else
            {
                Moving(false);
                Attack();
            }
            
        }

        public void TriggerKnockback(float force)
        {
            // Khởi động quá trình Knockback theo Curve
            _currentKnockbackForce = force;
            _knockbackTimer = 0f;
            _isKnockingBack = true;
        }

        public void SetupEnemyData(EnemyProfile data)
        {
            // Khi setup enemy mới, tạm dừng background cho đến khi nó bắt đầu chạy
            if (_backgroundManager) _backgroundManager.SetRunState(false);

        }

        public void Moving(bool isMoving,float speed =0)
        {
            if (isMoving)
            {
                speed = speed==0? this.speed : speed;
                _rb2d.linearVelocity = new Vector2(speed* -1f, _rb2d.linearVelocity.y);
                Running();
                if (_backgroundManager)  _backgroundManager.SetRunState(true);
            }
            else
            {
                if (_backgroundManager) _backgroundManager.SetRunState(false);
            }
        }

       

        // ReSharper disable Unity.PerformanceAnalysis
        public void FuelGaugeDercease()
        {
            if(IsFuelGausePause) return;
            currentFuelGauge -= Time.deltaTime;
            if(_fuelGaugeSlider){ _fuelGaugeSlider.value = currentFuelGauge;} 
            if (currentFuelGauge <= 0)
            {
                if(currentState != ActorState.BrokenStand)GameEvents.OnCharWrong?.Invoke();
                else GameEvents.OnFinisherFail?.Invoke();
                currentFuelGauge = FuelGauge;
            }
        }

        public void SetFuelGauge(float multi)
        {
            float a = FuelGauge * multi;
            _fuelGaugeSlider.maxValue = a;
            currentFuelGauge= a;
        }
        

        public void SetActiveFuelSlider(bool isActive)
        {
            IsFuelGausePause = !isActive;
            if (_fuelGaugeSlider) _fuelGaugeSlider.gameObject.SetActive(isActive);
            ResetFuelGauge();
        }
        
     

        public void ResetFuelGauge()
        {
            _fuelGaugeSlider.maxValue = FuelGauge;
            currentFuelGauge = FuelGauge;
        }
        
        /// <summary>
        /// Kiểm tra player
        /// </summary>
        private void CheckForPlayer()
        {

            Vector2 origin = (Vector2)transform.position + rayOffset;
            Vector2 direction = Vector2.left; 
        
            int hitCount = Physics2D.RaycastNonAlloc(origin, direction,_rayResults , rayDistance, detectionLayer);
            if (hitCount > 0)
            {
                var hit = _rayResults[0];

                if (hit.collider.CompareTag("Player"))
                {
                    if (!HasPlayerDetected)
                    {
                        HasPlayerDetected = true;
                    
                        if (_combatManager  && _combatManager.CurrentState != CombatState.Fighting)
                        {
                            
                            GameEvents.OnReadyToFight?.Invoke();
                        }
                    
                        FightStand();
                    }
                    return; 
                }
            }

            HasPlayerDetected = false;
        }

        public void HealthDecrease(int damage)
        {
            health -= damage;
        }

        public void ResetHealth()
        {
            health = maxHealth;
        }

        public void IncreaseHealth(int amount)
        {
            maxHealth += amount;
            health = maxHealth;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector2 origin = (Vector2)transform.position + rayOffset;
            Vector2 direction = Vector2.left; 
            Gizmos.DrawLine(origin, origin + direction * rayDistance);
        }
    }
}