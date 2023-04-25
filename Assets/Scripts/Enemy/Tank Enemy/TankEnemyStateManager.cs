using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class TankEnemyStateManager : MonoBehaviour
    {
        // ---- State Test materials ------------------------------
        [Header("-- State Test Materials --")]
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material shieldAbilityMat;
        public Material cleaveAbilityMat;
        public Material stunMat;
        public Material resetMat;
        public Material dyingMat;
        // --------------------------------------------------------


        // ---- Tank Enemy States ---------------------------------
        public TankEnemyState currentState;

        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
        public ShieldAbilityState shieldAbilityState;
        public CleaveAbilityState cleaveAbilityState;
        public StunState stunState;
        public ResetState resetState;
        public DyingState dyingState;
        // --------------------------------------------------------


        // ---- Cooldown states ------------------------------------
        [Header("-- Cooldown States --")]
        public bool shieldOnCooldown = false;
        public bool shieldActive = false;

        public bool cleaveOnCooldown = false;
        public bool cleaveActive = false;
        // ---------------------------------------------------------


        // ---- Tank Enemy Components ---------------------------------
        [Header("-- Internal Components --")]
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public NavMeshAgentManager navMeshAgentManagerCS;
        [HideInInspector] public HealthManager healthManagerCS;
        [HideInInspector] public EnemyDamageDealer enemyDamageDealerCS;
        [HideInInspector] public EnemyDamageReceiver enemyDamageReceiverCS;
        [HideInInspector] public Animator tankAnimator;
        // --------------------------------------------------------


        // ---- External References --------------------------------
        [Header("-- External References --")]
        public Transform resetTransform;
        public Transform targetTransform;
        public LayerMask targetLayerMask;
        // ---------------------------------------------------------


        // ---- Coroutines -----------------------------------------
        public Coroutine coroutineShieldCooldown;
        public Coroutine coroutineCleaveCooldown;
        // ---------------------------------------------------------


        // ---- Ajustable Values -----------------------------------
        [Header("-- Base Attack Settings --")]
        public float baseAttackRange = 2.2f;
        public float baseAttackDamage = 20f;
        [Range(0f, 1f)] public float baseLeech = 0f;
        public float baseAttackSpeed = 1.5f;
        public float detectionRange = 5f;


        [Header("-- Base Health Settings --")]
        public float baseHealthPoints = 200f;


        [Header("-- Base Movement Settings --")]
        public float baseMovementSpeed = 20f;


        [Header("-- Shield Ability Settings --")]
        [Range(0f, 1f)] public float shieldDamageReduction = 0.1f;
        [Range(0f, 1f)] public float shieldActivationRatio = 0.8f;
        public float shieldUpTime = 10f;
        public float shieldCooldown = 20f;


        [Header("-- Cleave Ability Settings --")]
        public float cleaveAttackDamage = 80f;
        [Range(0f, 1f)] public float cleaveActivationChance = 0.3f;
        public float cleaveCooldown = 20f;
        // ---------------------------------------------------------


        // ---- Calculated Values ----------------------------------
        [Header("-- Current Values --")]
        public float currentAttackDamage;
        public float currentLeech;
        public float currentAttackSpeed;
        public float stunDuration;
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

            if (healthRatio <= shieldActivationRatio && !shieldOnCooldown && !shieldActive && !cleaveActive)
            {
                TransitionToState(shieldAbilityState);
            }
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

        public IEnumerator CoroutineShieldCooldown()
        {
            shieldOnCooldown = true;
            yield return new WaitForSecondsRealtime(shieldCooldown);
            shieldOnCooldown = false;
        }

        public void EndCleave()
        {
            TransitionToState(chaseState);
        }

        public IEnumerator CoroutineCleaveCooldown()
        {
            cleaveOnCooldown = true;
            yield return new WaitForSecondsRealtime(cleaveCooldown);
            cleaveOnCooldown = false;
        }
    }
}