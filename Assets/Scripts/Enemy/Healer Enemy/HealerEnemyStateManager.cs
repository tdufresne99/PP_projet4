using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HealerEnemy
{
    public class HealerEnemyStateManager : MonoBehaviour
    {
        // ---- State Test materials ------------------------------
        [Header("-- State Test Materials --")]
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material teleportMat;
        public Material healStartMat;
        public Material healEndMat;
        public Material resetMat;
        public Material dyingMat;
        // ---------------------------------------------------------


        // ---- Cooldown states ------------------------------------
        [Header("-- Cooldown States --")]
        public bool teleportOnCooldown = false;
        public bool healOnCooldown = false;
        // ---------------------------------------------------------


        // ---- Range Enemy States ---------------------------------
        public HealerEnemyState currentState;

        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
        public TeleportAbilityState teleportAbilityState;
        public HealingAbilityState healingAbilityState;
        public ResetState resetState;
        public DyingState dyingState;
        // ---------------------------------------------------------


        // ---- Range Enemy Components -----------------------------
        [Header("-- Internal Components --")]
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public NavMeshAgentManager navMeshAgentManagerCS;
        [HideInInspector] public HealthManager healthManagerCS;
        [HideInInspector] public EnemyDamageDealer enemyDamageDealerCS;
        [HideInInspector] public EnemyDamageReceiver enemyDamageReceiverCS;
        [HideInInspector] public TeleportLocationFinder teleportLocationFinderCS;
        // ---------------------------------------------------------


        // ---- External References --------------------------------
        [Header("-- External References --")]
        public Transform resetTransform;
        public Transform targetTransform;
        public Transform projectileSpawnTransform;
        public GameObject projectileGO;
        public GameObject healingHitboxGO;
        public LayerMask targetLayerMask;
        // ---------------------------------------------------------


        // ---- Coroutines -----------------------------------------
        public Coroutine coroutineTeleportCooldown;
        public Coroutine coroutineHealing;
        // ---------------------------------------------------------


        // ---- Ajustable Values -----------------------------------
        [Header("-- Base Attack Settings --")]
        public float baseAttackRange = 5f;
        public float baseAttackDamage = 20f;
        public float baseLeech = 0f;
        public float baseAttackSpeed = 2.2f;
        public float detectionRange = 5f;


        [Header("-- Base Defense Settings --")]
        public float baseHealthPoints = 100f;


        [Header("-- Base Movement Settings --")]
        public float baseMovementSpeed = 5f;


        [Header("-- Teleport Ability Settings --")]
        public float teleportCooldownTime = 12f;
        public float teleportMaxRange = 20f;
        public float teleportMinRange = 15f;

        [Header("-- Heal Ability Settings --")]
        public float healValue = 100f;
        public float healCastTime = 3f;
        public float healCooldownTime = 15f;
        public float healRange = 8f;
        // ---------------------------------------------------------


        // ---- Calculated Values ----------------------------------
        [Header("-- Current Values --")]
        public float currentAttackRange;
        public float currentAttackDamage;
        public float currentLeech;
        public float currentAttackSpeed;
        private bool _inCombat = false;
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

            navMeshAgentManagerCS.ChangeStopDistance(currentAttackRange);

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

        private void OnDamageReceived()
        {
            if (!teleportOnCooldown)
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

        public void InstantiateProjectile()
        {
            var instanciatedProjectile = Instantiate(projectileGO, projectileSpawnTransform.position, Quaternion.identity);
            var rangeEnemyProjectileCS = instanciatedProjectile.GetComponent<HealerEnemyProjectile>();
            rangeEnemyProjectileCS.targetTransform = targetTransform;
            rangeEnemyProjectileCS.damage = currentAttackDamage;
        }

        public void TeleportRangeEnemy(Vector3 teleportPosition)
        {
            transform.position = teleportPosition;
            transform.LookAt(targetTransform);
        }

        public IEnumerator CoroutineTeleportCooldown()
        {
            teleportOnCooldown = true;
            yield return new WaitForSecondsRealtime(teleportCooldownTime);
            teleportOnCooldown = false;
        }

        public IEnumerator CoroutineHealing()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(healCooldownTime);
                healOnCooldown = false;
                TransitionToState(healingAbilityState);
            }
        }
    }
}