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
        [HideInInspector] public MeshRenderer meshRenderer;
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

            if (TryGetComponent(out TeleportLocationFinder teleportLocationFinderTemp)) teleportLocationFinderCS = teleportLocationFinderTemp;
            else Debug.LogError("The component 'TeleportLocationFinder' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");
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
            currentAttackRange = baseAttackRange;
            currentAttackDamage = baseAttackDamage;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;

            healthManagerCS.SetHealthPointsValues(baseHealthPoints);

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
        }

        private void OnDamageReceived()
        {
            if (!teleportOnCooldown && !abilityLocked)
            {
                TransitionToState(teleportAbilityState);
            }
        }

        private void OnHealthPointsEmpty()
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
    }
}