using UnityEngine;
using System.Collections;

namespace Enemy.Range
{
    public class RangeEnemyStateManager : MonoBehaviour
    {
        // ----------------------------------------------------------
        #region State Test Materials
        [Header("-- State Test Materials --")]
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material teleportMat;
        public Material stunMat;
        public Material resetMat;
        public Material dyingMat;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Range Enemy States
        [Header("-- Range Enemy States --")]
        public RangeEnemyState currentState;
        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
        public TeleportAbilityState teleportAbilityState;
        public MeteorAbilityState meteorAbilityState;
        public StunState stunState;
        public ResetState resetState;
        public DyingState dyingState;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Cooldown States
        [Header("-- Cooldown States --")]
        [SerializeField] private bool _teleportOnCooldown = false;
        public bool teleportOnCooldown
        {
            get => _teleportOnCooldown;
            set
            {
                if (_teleportOnCooldown == value) return;

                if (value == true) OnTeleportCooldownStart();
                else OnTeleportCooldownEnd();

                _teleportOnCooldown = value;
            }
        }
        [SerializeField] private bool _meteorOnCooldown = false;
        public bool meteorOnCooldown
        {
            get => _meteorOnCooldown;
            set
            {
                if (_meteorOnCooldown == value) return;

                if (value == true) OnMeteorCooldownStart();
                else OnMeteorCooldownEnd();

                _meteorOnCooldown = value;
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
        [HideInInspector] public TeleportLocationFinder teleportLocationFinderCS;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region External References
        [Header("-- External References --")]
        public Player.PlayerStateManager playerStateManagerCS;
        public Transform resetTransform;
        public Transform targetTransform;
        public Transform projectileSpawnTransform;
        public GameObject projectileGO;
        public GameObject meteorOverlayGO;
        public GameObject meteorGO;
        public LayerMask targetLayerMask;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Coroutines
        public Coroutine coroutineTeleportCooldown;
        public Coroutine coroutineMeteorCooldown;
        public Coroutine coroutineMeteor;
        public Coroutine coroutineDamageReduction;
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
        public float baseAttackRange = 20f;
        public float baseAttackDamage = 30f;
        public float baseLeech = 0f;
        public float baseAttackSpeed = 2.2f;
        public float detectionRange = 12f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Health Settings
        [Header("-- Base Health Settings --")]
        public float baseHealthPoints = 100f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Movement Settings
        [Header("-- Base Movement Settings --")]
        public float baseMovementSpeed = 20f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Teleport Ability Settings
        [Header("-- Teleport Ability Settings --")]
        [SerializeField] private float _teleportCooldownTime = 15f;
        public float teleportCooldownTime => _teleportCooldownTime * cooldownReduction;
        public float teleportMaxRange = 20f;
        public float teleportMinRange = 15f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Meteor Ability Settings
        [Header("-- Meteor Ability Settings --")]
        [SerializeField] private float _meteorCooldownTime = 30f;
        public float meteorCooldownTime => _meteorCooldownTime * cooldownReduction;
        public float firstMeteorWaitTime = 3f;
        [SerializeField] private float _meteorDamageMultiplier = 8f;
        public float meteorDamage => currentAttackDamage * _meteorDamageMultiplier;
        public float meteorSpeed = 22f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Cooldown Reduction Buff 
        [Header("-- Cooldown Reduction Buff --")]
        public float cooldownReductionBuffDuration = 5f;
        public float cooldownReductionBuffValue = 0.1f;
        public float baseCooldownReductionValue = 1;
        #endregion
        // -------------------------------------------------<
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
                Mathf.Clamp(value, 0, 10);
                _currentLeech = value;
                enemyDamageDealerCS.leech = _currentLeech;
            }
        }
        public float currentAttackSpeed;
        public float stunDuration;
        public float cooldownReduction = 1f;
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

        public void TransitionToState(RangeEnemyState newState)
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

            if (TryGetComponent(out TeleportLocationFinder teleportLocationFinderTemp)) teleportLocationFinderCS = teleportLocationFinderTemp;
            else Debug.LogError("The component 'TeleportLocationFinder' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");
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
            teleportAbilityState = new TeleportAbilityState(this);
            meteorAbilityState = new MeteorAbilityState(this);
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
            currentMovementSpeed = baseMovementSpeed;

            healthManagerCS.SetHealthPointsValues(currentMaxHealthPoints);

            teleportLocationFinderCS.radius = teleportMaxRange;
            teleportLocationFinderCS.minRadius = teleportMinRange;
        }
        
        private void OnCombatStart()
        {
            coroutineMeteor = StartCoroutine(CoroutineMeteor());
        }

        private void OnCombatEnd()
        {
            if (coroutineMeteor != null) StopCoroutine(coroutineMeteor);
            TransitionToState(resetState);
        }

        private void OnPlayerDeath()
        {
            inCombat = false;
        }

        private void OnDamageReceived(float damageReceived)
        {
            if (!teleportOnCooldown && !abilityLocked)
            {
                TransitionToState(teleportAbilityState);
            }
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

        // ===================================================================================================
        #region Teleport Cooldown
        public IEnumerator CoroutineTeleportCooldown()
        {
            yield return new WaitForSecondsRealtime(teleportCooldownTime);
            teleportOnCooldown = false;
        }

        private void OnTeleportCooldownStart()
        {
            coroutineTeleportCooldown = StartCoroutine(CoroutineTeleportCooldown());
        }

        private void OnTeleportCooldownEnd()
        {
            if (coroutineTeleportCooldown != null) StopCoroutine(coroutineTeleportCooldown);
        }
        #endregion

        // ===================================================================================================
        #region Meteor Cooldown
        public IEnumerator CoroutineMeteorCooldown()
        {
            yield return new WaitForSecondsRealtime(meteorCooldownTime);
            meteorOnCooldown = false;
        }

        private void OnMeteorCooldownStart()
        {
            coroutineMeteorCooldown = StartCoroutine(CoroutineMeteorCooldown());
        }

        private void OnMeteorCooldownEnd()
        {
            if (coroutineMeteorCooldown != null) StopCoroutine(coroutineMeteorCooldown);
        }
        #endregion

        // ===================================================================================================

        public IEnumerator CoroutineStartCooldownReduction()
        {
            cooldownReduction = cooldownReductionBuffValue;
            yield return new WaitForSecondsRealtime(cooldownReductionBuffDuration);
            cooldownReduction = baseCooldownReductionValue;
        }

        public IEnumerator CoroutineMeteor()
        {
            var waitTime = Random.Range(firstMeteorWaitTime - 2f, firstMeteorWaitTime + 2f);
            yield return new WaitForSecondsRealtime(waitTime);

            while (true)
            {
                TransitionToState(meteorAbilityState);
                yield return new WaitForSecondsRealtime(meteorCooldownTime);
            }
        }
    
        void OnDestroy()
        {
            if(playerStateManagerCS != null) playerStateManagerCS.OnPlayerDeath -= OnPlayerDeath;

            if(healthManagerCS != null) healthManagerCS.OnHealthPointsEmpty -= OnHealthPointsEmpty;
            if(enemyDamageReceiverCS != null) enemyDamageReceiverCS.OnDamageReceived -= OnDamageReceived;
        }
    }
}