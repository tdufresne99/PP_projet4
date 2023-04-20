using UnityEngine;
using System.Collections;

namespace RangeEnemy
{
    public class RangeEnemyStateManager : MonoBehaviour
    {
        // ---- State Test materials ------------------------------
        [Header("-- State Test Materials --")]
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material resetMat;
        public Material dyingMat;
        // ---------------------------------------------------------


        // ---- Cooldown states ------------------------------------
        [Header("-- Cooldown States --")]
        // ---------------------------------------------------------


        // ---- Range Enemy States ---------------------------------
        public RangeEnemyState currentState;

        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
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
        // ---------------------------------------------------------


         // ---- External References --------------------------------
        [Header("-- External References --")]
        public Transform resetTransform;
        public Transform targetTransform;
        public Transform projectileSpawnTransform;
        public GameObject projectileGO;
        public LayerMask targetLayerMask;
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
        // ---------------------------------------------------------


        // ---- Calculated Values ----------------------------------
        [Header("-- Current Values --")]
        public float currentAttackRange;
        public float currentAttackDamage;
        public float currentLeech;
        public float currentAttackSpeed;
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

        public void TransitionToState(RangeEnemyState newState)
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
        }

        private void CreateStateInstances()
        {
            idleState = new IdleState(this);
            chaseState = new ChaseState(this);
            basicAttackState = new BasicAttackState(this);
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

        public void InstantiateProjectile()
        {
            var instanciatedProjectile = Instantiate(projectileGO, projectileSpawnTransform.position, Quaternion.identity);
            var rangeEnemyProjectileCS = instanciatedProjectile.GetComponent<RangeEnemyProjectile>();
            rangeEnemyProjectileCS.targetTransform = targetTransform;
            rangeEnemyProjectileCS.damage = currentAttackDamage;
        }
    }
}