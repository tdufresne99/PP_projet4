using System.Collections;
using UnityEngine;

namespace Enemy.Tank
{
    public class TankEnemyStateManager : MonoBehaviour
    {
        // ---------------------------------------------------------
        #region State Test Materials
        [Header("-- State Test Materials --")]
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material shieldAbilityMat;
        public Material cleaveAbilityMat;
        public Material stunMat;
        public Material resetMat;
        public Material dyingMat;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Tank Enemy States
        public TankEnemyState currentState;
        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
        public ShieldAbilityState shieldAbilityState;
        public CleaveAbilityState cleaveAbilityState;
        public StunState stunState;
        public ResetState resetState;
        public DyingState dyingState;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Cooldown States
        [Header("-- Cooldown States --")]
        [SerializeField] private bool _shieldOnCooldown = false;
        public bool shieldOnCooldown
        {
            get => _shieldOnCooldown;
            set
            {
                if (_shieldOnCooldown == value) return;

                if (value == true) OnShieldCooldownStart();
                else OnShieldCooldownEnd();

                _shieldOnCooldown = value;
            }
        }

        [SerializeField] private bool _cleaveOnCooldown = false;
        public bool cleaveOnCooldown
        {
            get => _cleaveOnCooldown;
            set
            {
                if (_cleaveOnCooldown == value) return;

                if (value == true) OnCleaveCooldownStart();
                else OnCleaveCooldownEnd();

                _cleaveOnCooldown = value;
            }
        }
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
        [HideInInspector] public Animator tankAnimator;
        [HideInInspector] public AudioSource tankAudioSource;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region External References
        [Header("-- External References --")]
        public Player.PlayerStateManager playerStateManagerCS;
        public GameObject cleaveHitboxGO;
        public Transform resetTransform;
        public Transform targetTransform;
        public LayerMask targetLayerMask;
        public AudioClip deathSound;
        public AudioClip cleaveSound;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Coroutines
        public Coroutine coroutineShieldCooldown;
        public Coroutine coroutineCleaveCooldown;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Ajustable Values
        [Header("-- Ajustable Values --")]
        // ------------------------------------------------->
        #region Level Settings
        [Header("-- Level Settings --")]
        [SerializeField] private int _level = 0;
        public int level { get => _level; set => _level = value - 1; }
        public float statsBuffPerLevel = 0.2f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Attack Settings
        [Header("-- Base Attack Settings --")]
        public float baseAttackRange = 5f;
        public float baseAttackDamage = 20f;
        public float baseLeech = 0f;
        public float baseAttackSpeed = 1.8f;
        public float detectionRange = 12f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Health Settings
        [Header("-- Base Health Settings --")]
        public float baseHealthPoints = 500f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Movement Settings
        [Header("-- Base Movement Settings --")]
        public float baseMovementSpeed = 20f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Shield Ability Settings
        [Header("-- Shield Ability Settings --")]
        [Range(0f, 1f)] public float shieldDamageReduction = 0.1f;
        [Range(0f, 1f)] public float shieldActivationRatio = 0.6f; // HP threshold for shield activation
        public float shieldUpTime = 8f;
        public float shieldCooldownTime = 60f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Cleave Ability Settings
        [Header("-- Cleave Ability Settings --")]
        [SerializeField] private float _cleaveAttackMultiplier = 8f;
        public float cleaveAttackDamage => currentAttackDamage * _cleaveAttackMultiplier;
        [Range(0f, 1f)] public float cleaveActivationChance = 0.333f; // % to activate cleave with each attacks
        public float cleaveCooldownTime = 40f;
        #endregion
        // -------------------------------------------------<
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Calculated Values
        [Header("-- Current Values --")]
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
                Mathf.Clamp(value, 0, 10);
                _currentLeech = value;
                enemyDamageDealerCS.leech = _currentLeech;
            }
        }
        public float currentAttackSpeed;
        public float currentStunDuration;
        public bool stunned = false;
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
        public bool abilityLocked = false;
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

        public void TransitionToState(TankEnemyState newState)
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

            if (TryGetComponent(out AudioSource audioSourceTemp)) tankAudioSource = audioSourceTemp;
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
            shieldAbilityState = new ShieldAbilityState(this);
            cleaveAbilityState = new CleaveAbilityState(this);
            stunState = new StunState(this);
            resetState = new ResetState(this);
            dyingState = new DyingState(this);
        }

        private void SetBaseValues()
        {
            if(LevelManager.instance != null) level = LevelManager.instance.currentLevel;

            currentMaxHealthPoints = baseHealthPoints + (baseHealthPoints * level * statsBuffPerLevel);
            currentAttackDamage = baseAttackDamage + (baseAttackDamage * level * statsBuffPerLevel);

            currentAttackRange = baseAttackRange;
            currentAttackSpeed = baseAttackSpeed;
            currentLeech = baseLeech;
            currentMovementSpeed = baseMovementSpeed;

            healthManagerCS.SetHealthPointsValues(currentMaxHealthPoints);
        }

        private void OnCombatStart()
        {

        }

        private void OnCombatEnd()
        {
            TransitionToState(resetState);
        }

        private void OnPlayerDeath()
        {
            inCombat = false;
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

        private void OnDamageReceived(float damageReceived)
        {
            var healthRatio = healthManagerCS.currentHealthPoints / healthManagerCS.maxHealthPoints;

            if (healthRatio <= shieldActivationRatio && !shieldOnCooldown && !abilityLocked)
            {
                TransitionToState(shieldAbilityState);
            }
        }

        public void EndCleave()
        {
            TransitionToState(chaseState);
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


        #region Cleave Cooldown
        public IEnumerator CoroutineCleaveCooldown()
        {
            yield return new WaitForSecondsRealtime(cleaveCooldownTime);
            cleaveOnCooldown = false;
        }

        private void OnCleaveCooldownStart()
        {
            coroutineCleaveCooldown = StartCoroutine(CoroutineCleaveCooldown());
        }

        private void OnCleaveCooldownEnd()
        {
            if (coroutineCleaveCooldown != null) StopCoroutine(coroutineCleaveCooldown);
        }
        #endregion

        #region Shield Cooldown
        public IEnumerator CoroutineShieldCooldown()
        {
            yield return new WaitForSecondsRealtime(shieldCooldownTime);
            shieldOnCooldown = false;
        }

        private void OnShieldCooldownStart()
        {
            coroutineShieldCooldown = StartCoroutine(CoroutineShieldCooldown());
        }

        private void OnShieldCooldownEnd()
        {
            if (coroutineShieldCooldown != null) StopCoroutine(coroutineShieldCooldown);
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