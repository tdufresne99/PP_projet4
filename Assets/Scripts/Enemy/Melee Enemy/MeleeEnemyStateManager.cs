using UnityEngine;
using System.Collections;

namespace Enemy.Melee
{
    // *** Idée pour moi du futur: faire des dictionnaires d'anims pour differencier enrage et non enrage anims.
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
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public NavMeshAgentManager navMeshAgentManagerCS;
        [HideInInspector] public HealthManager healthManagerCS;
        [HideInInspector] public EnemyDamageDealer enemyDamageDealerCS;
        [HideInInspector] public EnemyDamageReceiver enemyDamageReceiverCS;
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
        public Coroutine coroutineEnrageCooldown;
        public Coroutine coroutineStopEnrage;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Ajustable Values
        [Header("-- Ajustable Values --")]
        // ------------------------------------------------->
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
        [Header("-- Base Attack Settings --")]
        public float baseHealthPoints = 225f;
        #endregion
        // -------------------------------------------------<
        // ------------------------------------------------->
        #region Enrage Settings
        [Header("-- Enrage Settings --")]
        public float enrageCooldownTime = 45f;
        public float enrageDuration = 10f;
        public float enrageSize = 1.2f;
        [SerializeField] private float _enrageAttackDamageMultiplier = 1.25f;
        public float enrageAttackDamage => currentAttackDamage * _enrageAttackDamageMultiplier;
        [SerializeField] private float _enrageLeech = 0.3f;
        public float enrageLeech => currentLeech + _enrageLeech;
        [SerializeField] private float _enrageAttackSpeed = 0.5f;
        public float enrageAttackSpeed => currentAttackSpeed - _enrageAttackSpeed;
        [SerializeField] private float _enrageMovementSpeed = 5f;
        public float enrageMovementSpeed => currentMovementSpeed + _enrageMovementSpeed;
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
            currentAttackDamage = baseAttackDamage;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
            currentLeech = baseLeech;
            currentMovementSpeed = baseMovementSpeed;

            healthManagerCS.SetHealthPointsValues(baseHealthPoints);
        }

        private void OnCombatStart()
        {
            
        }

        private void OnCombatEnd()
        {

        }

        private void OnDamageReceived()
        {
            
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

        public void OnEnrageEnter()
        {
            // Set enrage animations & model
            gameObject.transform.localScale = Vector3.one * enrageSize;

            enrageActive = true;

            // Set enrage values
            currentAttackDamage = enrageAttackDamage;
            currentLeech = enrageLeech;
            currentAttackSpeed = enrageAttackSpeed;
            currentMovementSpeed = enrageMovementSpeed;

            coroutineStopEnrage = StartCoroutine(CoroutineStopEnrage());
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
    }
}