using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Enemy.Healer
{
    public class HealerEnemyStateManager : MonoBehaviour
    {
        // ----------------------------------------------------------
        #region State Test Materials
        [Header("-- State Test Materials --")]
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material teleportMat;
        public Material healStartMat;
        public Material healEndMat;
        public Material stunMat;
        public Material resetMat;
        public Material dyingMat;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Healer Enemy States
        [Header("-- Healer Enemy States --")]
        public HealerEnemyState currentState;
        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
        public TeleportAbilityState teleportAbilityState;
        public HealingAbilityState healingAbilityState;
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
        [SerializeField] private bool _healOnCooldown = false;
        public bool healOnCooldown
        {
            get => _healOnCooldown;
            set
            {
                if (_healOnCooldown == value) return;

                // if (value == true) OnHealCooldownStart();
                // else OnHealCooldownEnd();

                _healOnCooldown = value;
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
        public GameObject healingHitboxGO;
        public LayerMask targetLayerMask;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Coroutines
        public Coroutine coroutineTeleportCooldown;
        public Coroutine coroutineHealCooldown;
        public Coroutine coroutineHealing;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Ajustable Values
        [Header("-- Ajustable Values --")]
        // ------------------------------------------------->
        [Header("-- Level Settings --")]
        #region Level Settings
        [SerializeField] private int _level = 0;
        public int level => _level + 1;
        public float statsBuffPerLevel = 0.2f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        [Header("-- Base Attack Values --")]
        #region Base Attack Values
        public float baseAttackRange = 20f;
        public float baseAttackDamage = 20f;
        public float baseLeech = 0f;
        public float baseAttackSpeed = 2f;
        public float detectionRange = 12f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Base Health Settings
        [Header("-- Base Health Settings --")]
        public float baseHealthPoints = 175f;
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
        public float teleportCooldownTime = 20f;
        public float teleportMaxRange = 15f;
        public float teleportMinRange = 10f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Heal Ability Settings
        [Header("-- Heal Ability Settings --")]
        public float healValue = 100f;
        public float healCastTime = 3f;
        public float healCooldownTime = 25f;
        public float healRange = 20f;
        public float firstHealWaitTime = 8f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
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

        public void TransitionToState(HealerEnemyState newState)
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
            if (TryGetComponent(out MeshRenderer meshRendererTemp)) meshRenderer = meshRendererTemp;
            else Debug.LogError("The component 'MeshRenderer' does not exist on object " + gameObject.name + " (HealerEnemyStateManager.cs)");

            if (TryGetComponent(out NavMeshAgentManager navMeshAgentManagerTemp)) navMeshAgentManagerCS = navMeshAgentManagerTemp;
            else Debug.LogError("The component 'NavMeshAgentManager' does not exist on object " + gameObject.name + " (HealerEnemyStateManager.cs)");

            if (TryGetComponent(out EnemyDamageDealer enemyDamageDealerTemp)) enemyDamageDealerCS = enemyDamageDealerTemp;
            else Debug.LogError("The component 'EnemyDamageDealer' does not exist on object " + gameObject.name + " (HealerEnemyStateManager.cs)");

            if (TryGetComponent(out EnemyDamageReceiver enemyDamageReceiverTemp)) enemyDamageReceiverCS = enemyDamageReceiverTemp;
            else Debug.LogError("The component 'EnemyDamageReceiver' does not exist on object " + gameObject.name + " (HealerEnemyStateManager.cs)");

            if (TryGetComponent(out HealthManager healthManagerTemp)) healthManagerCS = healthManagerTemp;
            else Debug.LogError("The component 'HealthManager' does not exist on object " + gameObject.name + " (HealerEnemyStateManager.cs)");

            if (TryGetComponent(out TeleportLocationFinder teleportLocationFinderTemp)) teleportLocationFinderCS = teleportLocationFinderTemp;
            else Debug.LogError("The component 'TeleportLocationFinder' does not exist on object " + gameObject.name + " (HealerEnemyStateManager.cs)");
        }

        private void CreateStateInstances()
        {
            idleState = new IdleState(this);
            chaseState = new ChaseState(this);
            basicAttackState = new BasicAttackState(this);
            teleportAbilityState = new TeleportAbilityState(this);
            healingAbilityState = new HealingAbilityState(this);
            stunState = new StunState(this);
            resetState = new ResetState(this);
            dyingState = new DyingState(this);
        }

        private void SetBaseValues()
        {
            currentMaxHealthPoints = baseHealthPoints + (baseHealthPoints * _level * statsBuffPerLevel);
            currentAttackDamage = baseAttackDamage + (baseAttackDamage * _level * statsBuffPerLevel);

            currentAttackRange = baseAttackRange;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;

            healthManagerCS.SetHealthPointsValues(currentMaxHealthPoints);

            teleportLocationFinderCS.radius = teleportMaxRange;
            teleportLocationFinderCS.minRadius = teleportMinRange;
        }

        private void OnCombatStart()
        {
            coroutineHealing = StartCoroutine(CoroutineHealing());
        }

        private void OnCombatEnd()
        {
            StopCoroutine(coroutineHealing);
        }

        private void OnDamageReceived(float damageReceived)
        {
            if (!teleportOnCooldown)
            {
                TransitionToState(teleportAbilityState);
            }
        }

        private void OnHealthPointsEmpty()
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

        public IEnumerator CoroutineHealing()
        {
            var waitTime = Random.Range(firstHealWaitTime - 2f, firstHealWaitTime + 2f);
            yield return new WaitForSecondsRealtime(waitTime);

            while (true)
            {
                TransitionToState(healingAbilityState);
                yield return new WaitForSecondsRealtime(healCooldownTime);
                healOnCooldown = false;
            }
        }

        // ===================================================================================================
        #region Heal Cooldown
        public IEnumerator CoroutineHealCooldown()
        {
            yield return new WaitForSecondsRealtime(healCooldownTime);
            healOnCooldown = false;
        }

        private void OnHealCooldownStart()
        {
            coroutineHealCooldown = StartCoroutine(CoroutineHealCooldown());
        }

        private void OnHealCooldownEnd()
        {
            if (coroutineHealCooldown != null) StopCoroutine(coroutineHealCooldown);
        }
        #endregion

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
    }
}