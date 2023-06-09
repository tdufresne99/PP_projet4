using UnityEngine;
using System.Collections;

namespace Enemy.Melee
{
    public class MeleeEnemyStateManager : MonoBehaviour
    {
        // ----------------------------------------------------------
        #region State Test Materials
        [Header("-- State Test Materials --")]
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material stunMat;
        public Material resetMat;
        public Material dyingMat;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Melee Enemy States
        [Header("-- Range Enemy States --")]
        public MeleeEnemyState currentState;
        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
        public StunState stunState;
        public ResetState resetState;
        public DyingState dyingState;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Cooldown States
        [Header("-- Cooldown States --")]
        
        [SerializeField] private bool _enrageOnCooldown = false;
        public bool enrageOnCooldown
        {
            get => _enrageOnCooldown;
            set
            {
                if (_enrageOnCooldown == value) return;

                if (value == true) OnEnrageCooldownStart();
                else OnEnrageCooldownEnd();

                _enrageOnCooldown = value;
            }
        }
        public bool enrageActive = false;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Internal Components
        [Header("-- Internal Components --")]
        [HideInInspector] public Animator enemyAnimator;
        [HideInInspector] public NavMeshAgentManager navMeshAgentManagerCS;
        [HideInInspector] public HealthManager healthManagerCS;
        [HideInInspector] public EnemyDamageDealer enemyDamageDealerCS;
        [HideInInspector] public EnemyDamageReceiver enemyDamageReceiverCS;
        [HideInInspector] public AudioSource meleeAudioSource;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region External References
        [Header("-- External References --")]
        public Player.PlayerStateManager playerStateManagerCS;
        public Transform resetTransform;
        public Transform targetTransform;
        public LayerMask targetLayerMask;
        public AudioClip deathSound;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Coroutines
        public Coroutine coroutineEnrageCooldown;
        public Coroutine coroutineStopEnrage;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Ajustable Values
        [Header("-- Ajustable Values --")]
        // ------------------------------------------------->
        [Header("-- Level Settings --")]
        #region Level Settings
        [SerializeField] private int _level = 0;
        public int level { get => _level; set => _level = value - 1; }
        public float statsBuffPerLevel = 0.2f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        [Header("-- Base Attack Values --")]
        #region Base Attack Values
        public float baseAttackRange = 5f;
        public float baseAttackDamage = 25f;
        public float baseLeech = 0f;
        public float baseAttackSpeed = 1.5f;
        public float detectionRange = 12f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Health Settings
        [Header("-- Base Health Settings --")]
        public float baseHealthPoints = 225f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Enrage Settings
        [Header("-- Enrage Settings --")]
        public float enrageCooldownTime = 45f;
        public float enrageDuration = 10f;
        public float enrageSizeMultiplier = 1.5f;
        [SerializeField] private float _enrageAttackDamageMultiplier = 1.25f;
        public float _enrageLeechMultiplier = 0.5f;
        [SerializeField] private float _enrageAttackSpeedMultiplier = 1.5f;
        [SerializeField] private float _enrageMovementSpeedMultiplier = 1.5f;
        [SerializeField] private ParticleSystem _particuleSystem;
        public int maxEnrageStacks = 3;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Attack Values
        [Header("-- Base Movement Settings --")]
        public float baseMovementSpeed = 20f;
        #endregion
        // ---------------------------------------------------------
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Calculated Values
        [Header("-- Current Values --")]
        [SerializeField] private bool _inCombat = false;
        public bool inCombat
        {
            get => _inCombat;
            set
            {
                if (_inCombat == value) return;
                if (value == true) OnCombatStart();
                else OnCombatEnd();
                _inCombat = value;
            }
        }
        public float currentMaxHealthPoints;
        public float currentAttackDamage;
        [SerializeField] private float _currentAttackRange;
        public float currentAttackRange
        {
            get => _currentAttackRange;
            set
            {
                if (_currentAttackRange == value) return;
                _currentAttackRange = value;
                navMeshAgentManagerCS.ChangeStopDistance(_currentAttackRange);
            }
        }
        [SerializeField] private float _currentLeech;
        public float currentLeech
        {
            get => _currentLeech;
            set
            {
                if (value == _currentLeech) return;
                Mathf.Clamp(value, 0, 1f);
                _currentLeech = value;
                enemyDamageDealerCS.leech = _currentLeech;
            }
        }
        public float currentAttackSpeed;
        public float stunDuration;
        public bool stunned = false;
        public bool abilityLocked = false;
        [SerializeField] private float _currentMovementSpeed;
        public float currentMovementSpeed
        {
            get => _currentMovementSpeed;
            set
            {
                _currentMovementSpeed = value;
                navMeshAgentManagerCS.ChangeAgentSpeed(value);
            }
        }
        [SerializeField] private float _successiveBasicAttacks = 0;
        public float successiveBasicAttacks
        {
            get => _successiveBasicAttacks;
            set
            {
                if (enrageActive || enrageOnCooldown) return;
                _successiveBasicAttacks = value;
                if (_successiveBasicAttacks >= maxEnrageStacks)
                {
                    _successiveBasicAttacks = 0;
                    OnEnrageEnter();
                }
            }
        }
        #endregion
        // ---------------------------------------------------------

        void Awake()
        {
            TryGetRequiredComponents();
            SubscribeToRequiredEvents();
        }

        private void Start()
        {
            var trialsEnemy = GetComponent<TrialsEnemy>();
            if (trialsEnemy != null && TrialsManager.instance != null) GetTrialsRequiredLinks();

            if (LevelManager.instance != null) GetLevelRequiredLinks();

            CreateStateInstances();
            SetBaseValues();
            TransitionToState(idleState);
        }

        void Update()
        {
            if (currentState == null)
            {
                TransitionToState(idleState);
            }

            currentState.Execute();
        }

        public void TransitionToState(MeleeEnemyState newState)
        {
            if (stunned && newState != dyingState) return;
            
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;
            currentState.Enter();
        }

        private void SubscribeToRequiredEvents()
        {
            healthManagerCS.OnHealthPointsEmpty += OnHealthPointsEmpty;
            enemyDamageReceiverCS.OnDamageReceived += OnDamageReceived;
        }

        private void TryGetRequiredComponents()
        {
            var animator = GetComponentInChildren<Animator>();
            if (animator != null) enemyAnimator = animator;
            else Debug.LogError("The component 'Animator' does not exist on object " + gameObject.name + " (HealerEnemyStateManager.cs)");

            if (TryGetComponent(out NavMeshAgentManager navMeshAgentManagerTemp)) navMeshAgentManagerCS = navMeshAgentManagerTemp;
            else Debug.LogError("The component 'NavMeshAgentManager' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out EnemyDamageDealer enemyDamageDealerTemp)) enemyDamageDealerCS = enemyDamageDealerTemp;
            else Debug.LogError("The component 'EnemyDamageDealer' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out EnemyDamageReceiver enemyDamageReceiverTemp)) enemyDamageReceiverCS = enemyDamageReceiverTemp;
            else Debug.LogError("The component 'EnemyDamageReceiver' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out HealthManager healthManagerTemp)) healthManagerCS = healthManagerTemp;
            else Debug.LogError("The component 'HealthManager' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out AudioSource audioSourceTemp)) meleeAudioSource = audioSourceTemp;
            else Debug.LogError("The component 'AudioSource' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");
        }

        private void GetTrialsRequiredLinks()
        {
            if(playerStateManagerCS == null) playerStateManagerCS = TrialsManager.instance.playerStateManagerCS;

            targetTransform = playerStateManagerCS.transform;
            enemyDamageDealerCS.playerDamageReceiver = targetTransform.GetComponent<Player.PlayerDamageReceiver>();

            playerStateManagerCS.OnPlayerDeath += OnPlayerDeath;
        }

        private void GetLevelRequiredLinks()
        {
            if(playerStateManagerCS == null) playerStateManagerCS = LevelManager.instance.playerStateManagerCS;

            targetTransform = playerStateManagerCS.transform;
            enemyDamageDealerCS.playerDamageReceiver = targetTransform.GetComponent<Player.PlayerDamageReceiver>();

            playerStateManagerCS.OnPlayerDeath += OnPlayerDeath;
        }

        private void CreateStateInstances()
        {
            idleState = new IdleState(this);
            chaseState = new ChaseState(this);
            basicAttackState = new BasicAttackState(this);
            stunState = new StunState(this);
            resetState = new ResetState(this);
            dyingState = new DyingState(this);
        }

        private void SetBaseValues()
        {
            if(LevelManager.instance != null) level = LevelManager.instance.currentLevel;

            currentMaxHealthPoints = baseHealthPoints + (baseHealthPoints * level * statsBuffPerLevel);
            currentAttackDamage = baseAttackDamage + (baseAttackDamage * level * statsBuffPerLevel);

            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
            currentLeech = baseLeech;
            currentMovementSpeed = baseMovementSpeed;

            healthManagerCS.SetHealthPointsValues(currentMaxHealthPoints);
        }

        private void OnCombatStart()
        {
            
        }

        private void OnCombatEnd()
        {

        }

        private void OnPlayerDeath()
        {
            inCombat = false;
        }

        private void OnDamageReceived(float damageReceived)
        {
            
        }

        private void OnHealthPointsEmpty(HealthManager hm)
        {
            // Do something...

            // Die
            Die();
        }

        private void Die()
        {
            TransitionToState(dyingState);
        }

        public void SelfDestruct()
        {
            Destroy(gameObject);
        }

        public bool DetectObject(Transform otherObjectTransform, float distanceThreshold, LayerMask layerMask)
        {
            Vector3 object1Pos = transform.position;
            Vector3 object2Pos = otherObjectTransform.position;

            if ((object1Pos - object2Pos).sqrMagnitude > distanceThreshold * distanceThreshold)
            {
                return false;
            }

            Vector3 direction = object2Pos - object1Pos;

            RaycastHit hit;
            bool isHit = Physics.Raycast(object1Pos, direction, out hit, distanceThreshold, layerMask);

            if (!isHit || hit.collider.gameObject == otherObjectTransform.gameObject)
            {
                Debug.DrawLine(object1Pos, object2Pos, Color.green, 0.1f);
                return true;
            }
            else
            {
                Debug.DrawLine(object1Pos, hit.point, Color.red, 0.1f);
                return false;
            }
        }

        public void OnEnrageEnter()
        {
            // Set enrage animations & model
            gameObject.transform.localScale *= enrageSizeMultiplier;
            _particuleSystem.Play();

            enrageActive = true;

            // Set enrage values
            currentAttackDamage *= _enrageAttackDamageMultiplier;
            currentLeech += _enrageLeechMultiplier;
            currentAttackSpeed *= _enrageAttackSpeedMultiplier;
            currentMovementSpeed *= _enrageMovementSpeedMultiplier;

            coroutineStopEnrage = StartCoroutine(CoroutineStopEnrage());
        }

        public void OnEnrageExit()
        {
            // Set base animations & model
            gameObject.transform.localScale /= enrageSizeMultiplier;
            _particuleSystem.Stop();

            // Set base values
            currentAttackDamage /= _enrageAttackDamageMultiplier;
            currentLeech -= _enrageLeechMultiplier;
            currentAttackSpeed /= baseAttackSpeed;
            currentMovementSpeed /= baseMovementSpeed;

            enrageActive = false;
            enrageOnCooldown = true;
        }

        public IEnumerator CoroutineStopEnrage()
        {
            yield return new WaitForSecondsRealtime(enrageDuration);
            OnEnrageExit();
        }

        #region Enrage Cooldown
        public IEnumerator CoroutineEnrageCooldown()
        {
            yield return new WaitForSecondsRealtime(enrageCooldownTime);
            enrageOnCooldown = false;
        }

        private void OnEnrageCooldownStart()
        {
            coroutineEnrageCooldown = StartCoroutine(CoroutineEnrageCooldown());
        }

        private void OnEnrageCooldownEnd()
        {
            if (coroutineEnrageCooldown != null) StopCoroutine(coroutineEnrageCooldown);
        }
        #endregion
    
        void OnDestroy()
        {
            if(playerStateManagerCS != null) playerStateManagerCS.OnPlayerDeath -= OnPlayerDeath;

            if(healthManagerCS != null) healthManagerCS.OnHealthPointsEmpty -= OnHealthPointsEmpty;
            if(enemyDamageReceiverCS != null) enemyDamageReceiverCS.OnDamageReceived -= OnDamageReceived;
        }
    }
}