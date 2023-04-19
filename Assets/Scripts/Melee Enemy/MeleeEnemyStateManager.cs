using UnityEngine;
using System.Collections;

namespace MeleeEnemy
{
    // *** Idée pour moi du futur: faire des dictionnaires d'anims pour differencier enrage et non enrage anims.
    public class MeleeEnemyStateManager : MonoBehaviour
    {
        // ---- State Test materials ------------------------------
            [Header("State Test Materials")]
            public Material idleMat;
            public Material chaseMat;
            public Material basicAttackMat;
            public Material resetMat;
        // ---------------------------------------------------------


        // ---- Cooldown states ------------------------------------
            [Header("Cooldown States")]
            public bool enrageOnCooldown = false;
            public bool enrageActive = false; 
        // ---------------------------------------------------------


        // ---- Melee Enemy States ---------------------------------
            public MeleeEnemyState currentState;

            public IdleState idleState;
            public ChaseState chaseState;
            public BasicAttackState basicAttackState;
            public ResetState resetState;
            public DyingState dyingState;
        // ---------------------------------------------------------


        // ---- Melee Enemy Components -----------------------------
            [Header("Internal Components")]
            public MeshRenderer meshRenderer;
            public NavMeshAgentManager navMeshAgentManagerCS;
            public HealthManager healthManagerCS;
            public EnemyDamageDealer enemyDamageDealerCS;
        // ---------------------------------------------------------


        // ---- External References --------------------------------
            [Header("External references")]
            public Transform resetTransform;
            public Transform targetTransform;
            public LayerMask targetLayerMask;
        // ---------------------------------------------------------


        // ---- Coroutines -----------------------------------------
            private Coroutine _coroutineEnrageCooldown;
            private Coroutine _coroutineStopEnrage;
        // ---------------------------------------------------------


        // ---- Ajustable Values -----------------------------------
            [Header("Base Attack Settings")]
            public float baseAttackRange = 2.2f;
            public float baseAttackDamage = 20f;
            public float baseLeech = 0f;
            public float baseAttackSpeed = 1.5f;


            [Header("Enrage Settings")]
            public float enrageCoodldown = 20f;
            public float enrageDuration = 8f;
            public float enrageAttackDamageBonus = 5f;
            public float enrageLeechBonus = 0.3f;
            public float enrageAttackSpeedBonus = 0.5f;
            public float enrageMovementSpeedBonus = 20f;
            public int nbOfAttacksToTriggerEnrage = 3;


            [Header("Base Movement Settings")]
            public float baseMovementSpeed = 20f;
        // ---------------------------------------------------------


        // ---- Calculated Values ----------------------------------
            [Header("Current values")]
            public float currentAttackDamage;
            public float currentLeech;
            public float currentAttackSpeed;
            public float currentMovementSpeed;
            [SerializeField] private float _successiveBasicAttacks = 0;
            public float successiveBasicAttacks
            {
                get => _successiveBasicAttacks;
                set
                {
                    if(enrageActive || enrageOnCooldown) return;
                    _successiveBasicAttacks = value;
                    if(_successiveBasicAttacks >= nbOfAttacksToTriggerEnrage) 
                    {
                        _successiveBasicAttacks = 0;
                        OnEnrageEnter();
                    }
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

        public void TransitionToState(MeleeEnemyState newState)
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

            if (TryGetComponent(out EnemyDamageDealer enemyDamageDealerCSTemp)) enemyDamageDealerCS = enemyDamageDealerCSTemp;
            else Debug.LogError("The component 'EnemyDamageDealer' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out HealthManager healthManagerCSTemp)) healthManagerCS = healthManagerCSTemp;
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
            currentAttackDamage = baseAttackDamage;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
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

        public void OnEnrageEnter()
        {
            enrageActive = true;

            // Set enrage animations & model
            gameObject.transform.localScale = Vector3.one * 1.2f;
            
            // Set enrage values
            currentAttackDamage = baseAttackDamage + enrageAttackDamageBonus;
            currentLeech = baseLeech + enrageLeechBonus;
            currentAttackSpeed = baseAttackSpeed - enrageAttackSpeedBonus;
            currentMovementSpeed = baseMovementSpeed + enrageMovementSpeedBonus;

            _coroutineStopEnrage = StartCoroutine(CoroutineStopEnrage());
        }

        public void OnEnrageExit()
        {
            // Set base animations & model
            gameObject.transform.localScale = Vector3.one;
            
            // Set base values
            currentAttackDamage = baseAttackDamage;
            currentLeech = baseLeech;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
            
            enrageActive = false;
            _coroutineEnrageCooldown = StartCoroutine(CoroutineEnrageCooldown());
        }

        public IEnumerator CoroutineStopEnrage()
        {
            yield return new WaitForSecondsRealtime(enrageDuration);
            OnEnrageExit();
        }

        public IEnumerator CoroutineEnrageCooldown()
        {
            enrageOnCooldown = true;
            yield return new WaitForSecondsRealtime(enrageCoodldown);
            enrageOnCooldown = false;
        }
    }
}