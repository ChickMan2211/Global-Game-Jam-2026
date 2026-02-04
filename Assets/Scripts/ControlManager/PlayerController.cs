using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace ControlManager
{
    public class PlayerController : Actor
    {
        [Header("Player Stats")]
        [SerializeField] private int maxHealth=3;
        [SerializeField] private int maxLife = 2;

        [Header("setting")]
        [SerializeField] private float delayTakeDamage;
        
        [Header("VFX References")]
        [SerializeField] private ParticleSystem attackParticle;
        [SerializeField] private ParticleSystem focusParticle;

        [SerializeField] private ParticleSystem runParticle;
        [SerializeField] private ParticleSystem finisherParticle;

        [Header("Runtime Stats (Debug)")]
        [SerializeField] private int currentHealth;
        [SerializeField] private int currentLife;

        [SerializeField] private TextMeshProUGUI textHealth;
        [SerializeField] private GameObject iconDead;
        
        private CinemachineImpulseSource _myImpulse;

        public int CurrentHealth 
        { 
            get => currentHealth; 
            private set => currentHealth = value; 
        }
    
        public int CurrentLife 
        { 
            get => currentLife; 
            private set => currentLife = value; 
        }
        
        
    
        protected override void Start()
        {
            _myImpulse = GetComponent<CinemachineImpulseSource>();
            base.Start();
            ResetLife();
            ResetHealth();
        }

        private void OnEnable()
        {
            GameEvents.OnLevelStart += OnLevelStartHandler;

        }

        private void OnDisable()
        {
            GameEvents.OnLevelStart -= OnLevelStartHandler;
        
        }

        private void Update()
        {
            Attack();
            UpdateHealthUI();
        }

        private void OnLevelStartHandler( )
        {
            StartLevel();
        }

        public void StartLevel()
        {
            Running();
        }

        #region Action
        public override void Running()
        {
            base.Running();
            ToggleFootStepEffect(true);
        }

        public override void Stopping()
        {
            base.Stopping();
            ToggleFootStepEffect(false);
        }

        public void ReadyToFight(float timeDelayReady)
        {
            Stopping();
            this.DelayAction(timeDelayReady, () => FightStand());
        }

        public void DoParry()
        {
            base.NextAttack();
            PlayerEffectSword();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            GameManager.Instance.audioManager.PlayFocusSound();
            focusParticle.Play();
        }

        #endregion
    
        #region Sound Effect And Particle

        public void PlayerEffectSword()
        {
            if (attackParticle) 
                attackParticle.Play();
          
            PlaySwordSound();
        }
        private void PlaySwordSound()
        {
            GameManager.Instance.PlayerSwordEffect();
        }

        private void ToggleFootStepEffect(bool isPlay)
        {
            GameManager.Instance.PlayerFootStepEffect(isPlay);
            if(runParticle && isPlay) runParticle.Play();
        }


        #endregion
    
        #region Health and life
        public void DecreaseHealth()
        {
            
            LockAttack(delayTakeDamage);
            this.DelayAction( delayTakeDamage, TakeDamage);
            CurrentHealth--;
            this.DelayAction(0.1f,() => _myImpulse.GenerateImpulse(Vector3.one * 0.1f));
                    GameManager.Instance.audioManager.PlayHurtSound();
            
            if (CurrentHealth <= 0)
            {
              
                DecreaseLife();
                ResetHealth();
                if(CurrentLife >0)
                GameEvents.OnPlayerBroken?.Invoke(true);
            }
        }
        public void ResetHealth()
        {
            CurrentHealth = maxHealth;
        }

        public void ResetLife()
        {
            CurrentLife = maxLife;
            ResetHealth(); 
        }

        public void DecreaseLife()
        {
            CurrentLife--;
            if (CurrentLife <= 0)  this.DelayAction( delayTakeDamage,()=>
            {
                CancelNextAttack();
                Die();
                GameEvents.OnEndGame?.Invoke();
                
            });
      
        }

        public void LockAttack(float time)
        {
            CancelNextAttack();
            if (CurrentLife <= 0)  this.DelayAction( time+0.1f,()=>
            {
                if(currentState == ActorState.BrokenStand || currentState == ActorState.Dead) return;
                NextAttack();
            });
        }

        public void PlayerDefeated()
        {
            currentHealth = 2;
            currentLife = 1;
        }

        private void UpdateHealthUI()
        {
            if (currentState == ActorState.Dead)
            {
                textHealth.text = ""; 
                return;
            }
            switch (currentHealth)
            {
                case 1: textHealth.text = "I"; break;
                case 2: textHealth.text = "II"; break;
                default: textHealth.text = "II"; break;
            }
            if(currentLife <= 1) iconDead.SetActive(true);
            else iconDead.SetActive(false);
        }
        
        public void DoCinematicShake()
        {
            if (_myImpulse != null)
            {
                _myImpulse.GenerateImpulse(Vector3.one * 0.1f);
            }
        }
        #endregion
    
    
  
    }
}
