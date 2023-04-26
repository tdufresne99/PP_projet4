using System.Collections;
using UnityEngine;

namespace TankEnemy
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
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public NavMeshAgentManager navMeshAgentManagerCS;
        [HideInInspector] public HealthManager healthManagerCS;
        [HideInInspector] public EnemyDamageDealer enemyDamageDealerCS;
        [HideInInspector] public EnemyDamageReceiver enemyDamageReceiverCS;
        [HideInInspector] public Animator tankAnimator;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region External References
        [Header("-- External References --")]
        public Transform resetTransform;
        public Transform targetTransform;
        public LayerMask targetLayerMask;
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
        #region Base Attack Settings
        [Header("-- Base Attack Settings --")]
        public float baseAttackRange = 4f;
        public float baseAttackDamage = 20f;
        public float baseLeech = 0f;
        public float baseAttackSpeed = 1.5f;
        public float detectionRange = 10f;
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
        #endregion
        // ---------------------------------------------------------

        void Awake()
        {
            TryGetRequiredComponents();
            SubscribeToRequiredEvents();
        }

        private void Start()
        {
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
            if (stunned) return;

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
            healthManagerCS.OnDamageReceived += OnDamageReceived;
        }

        private void TryGetRequiredComponents()
        {
            if (TryGetComponent(out MeshRenderer meshRendererTemp)) meshRenderer = meshRendererTemp;
            else Debug.LogError("The component 'MeshRenderer' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out NavMeshAgentManager navMeshAgentManagerTemp)) navMeshAgentManagerCS = navMeshAgentManagerTemp;
            else Debug.LogError("The component 'NavMeshAgentManager' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out EnemyDamageDealer enemyDamageDealerTemp)) enemyDamageDealerCS = enemyDamageDealerTemp;
            else Debug.LogError("The component 'EnemyDamageDealer' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out EnemyDamageReceiver enemyDamageReceiverTemp)) enemyDamageReceiverCS = enemyDamageReceiverTemp;
            else Debug.LogError("The component 'EnemyDamageReceiver' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out HealthManager healthManagerTemp)) healthManagerCS = healthManagerTemp;
            else Debug.LogError("The component 'HealthManager' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out Animator tankAnimatorTemp)) tankAnimator = tankAnimatorTemp;
            else Debug.LogError("The component 'Animator' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");
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
            currentAttackRange = baseAttackRange;
            currentAttackDamage = baseAttackDamage;
            currentAttackSpeed = baseAttackSpeed;
            currentLeech = baseLeech;
            currentMovementSpeed = baseMovementSpeed;

            healthManagerCS.SetHealthPointsValues(baseHealthPoints);
        }

        private void OnHealthPointsEmpty()
        {
            TransitionToState(dyingState);
        }

        private void OnDamageReceived()
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

        public void SelfDestruct()
        {
            Destroy(gameObject);
        }

        public bool DetectObject(Transform otherObjectTransform, float distanceThreshold, LayerMask layerMask)
        {
            // Get the position of the two GameObjects
            Vector3 object1Pos = transform.position;
            Vector3 object2Pos = otherObjectTransform.position;

            // Check if the two objects are within the maximum distance for the line of sight check
            if ((object1Pos - object2Pos).sqrMagnitude > distanceThreshold * distanceThreshold)
            {
                // The two objects are too far apart for a line of sight check, do not perform raycast
                return false;
            }

            // Find the direction from object1 to object2
            Vector3 direction = object2Pos - object1Pos;

            // Set up the raycast hit information
            RaycastHit hit;
            bool isHit = Physics.Raycast(object1Pos, direction, out hit, distanceThreshold, layerMask);

            // Check if the raycast hit anything
            if (!isHit || hit.collider.gameObject == otherObjectTransform.gameObject)
            {
                // There are no obstacles in the way, so the two objects have line of sight
                // Visualize the check by drawing a line between the two objects
                Debug.DrawLine(object1Pos, object2Pos, Color.green, 0.1f);
                return true;
            }
            else
            {
                // There is an obstacle in the way, so the two objects do not have line of sight
                // Visualize the check by drawing a line between the two objects up to the point of the hit
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

    }
}