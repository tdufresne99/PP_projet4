
using UnityEngine;
using System.Collections;

namespace Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        // ---- State Test materials ------------------------------
        [Header("State Test Materials")]
        public Material idleMat;
        public Material basicAttackMat;
        public Material spreadFireMat;
        public Material lightningRainMat;
        public Material iceShieldMat;
        public Material naturesMelodyMat;
        public Material dyingMat;
        // ---------------------------------------------------------


        // ---- Cooldown states ------------------------------------
        [Header("Cooldown States")]
        [SerializeField] private bool _spreadFireOnCooldown = false;
        public bool spreadFireOnCooldown
        {
            get => _spreadFireOnCooldown;
            set
            {
                if (_spreadFireOnCooldown == value) return;

                if (value == true) OnSpreadFireCooldownStart();
                else OnSpreadFireCooldownEnd();

                _spreadFireOnCooldown = value;
            }
        }

        public bool lightningRainOnCooldown = false;

        public bool iceShieldOnCooldown = false;

        public bool naturesMelodyOnCooldown = false;
        // ---------------------------------------------------------


        // ---- Player States --------------------------------------
        public PlayerState currentState;

        public IdleState idleState;
        public LightningRainState lightningRainState;
        public SpreadFireState spreadFireState;
        public IceShieldState iceShieldState;
        public NaturesMelodyState naturesMelodyState;
        // ---------------------------------------------------------


        // ---- Player Components ----------------------------------
        [Header("Internal Components")]
        public MeshRenderer meshRenderer;
        public Rigidbody playerRigidbody;
        public HealthManager healthManagerCS;
        public ShieldManager shieldManagerCS;

        // ---------------------------------------------------------


        // ---- External References --------------------------------
        [Header("External references")]
        public Transform spawnTransform;
        public Transform targetTransform;
        public LayerMask targetLayerMask;
        // ---------------------------------------------------------


        // ---- Coroutines -----------------------------------------
        private Coroutine _coroutineSpreadFireCooldown;
        // ---------------------------------------------------------


        // ---- Ajustable Values -----------------------------------
        [Header("Base Attack Settings")]
        public float baseAttackRange = 2.2f;
        public float baseAttackDamage = 20f;
        public float baseAttackSpeed = 1.5f;


        [Header("Base Movement Settings")]
        public float baseMovementSpeed = 20f;
        public float baseJumpForce = 20f;

        [Header("SpreadFire Ability Settings")]
        public float spreadFireRange = 12f;
        public float spreadFireDamage = 120f;
        public float spreadFireDuration = 8f;
        public float spreadFireCooldownTime = 45f;
        public int spreadFireTicks = 5;


        [Header("LightningRain Ability Settings")]
        public float lightningRainRadius = 8f;
        public float lightningRainDamagePerCharge = 40f;
        public float lightningRainCooldownTime = 100f;
        public float lightningRainActivationDelay = 3f;
        public float lightningRainStunDuration = 3f;
        public int lightningRainMaxCharges = 3;


        [Header("IceShield Ability Settings")]
        public int iceShieldMaxStacks = 4;
        public float iceShieldHealthPerStack = 50f;

        [Header("NaturesMelody Ability Settings")]

        // ---------------------------------------------------------


        // ---- Calculated Values ----------------------------------
        [Header("Current values")]
        public float currentAttackDamage;
        public float currentAttackSpeed;
        public float currentMovementSpeed;
        public float currentJumpForce;
        public bool abilityLocked = false;
        // ---------------------------------------------------------

        void Awake()
        {
            TryGetRequiredComponents();
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

            if (abilityLocked == false)
            {
                // Spread Fire
                if (Input.GetKeyDown(KeyCode.E) && !spreadFireOnCooldown) TransitionToState(spreadFireState);

                // Lightning Rain
                else if (Input.GetKeyDown(KeyCode.Q) && !lightningRainOnCooldown) TransitionToState(lightningRainState);

                // Ice Shield
                else if (Input.GetKeyDown(KeyCode.LeftShift) && !iceShieldOnCooldown) TransitionToState(iceShieldState);

                // Nature's Melody
                else if (Input.GetKeyDown(KeyCode.R) && !naturesMelodyOnCooldown) TransitionToState(naturesMelodyState);
            }

            currentState.Execute();
        }

        public void TransitionToState(PlayerState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;
            currentState.Enter();
        }

        private void TryGetRequiredComponents()
        {
            if (TryGetComponent(out MeshRenderer meshRendererTemp)) meshRenderer = meshRendererTemp;
            else Debug.LogError("The component 'MeshRenderer' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            if (TryGetComponent(out Rigidbody playerRigidbodyTemp)) playerRigidbody = playerRigidbodyTemp;
            else Debug.LogError("The component 'Rigidbody' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");
            
            if (TryGetComponent(out HealthManager healthManagerTemp)) healthManagerCS = healthManagerTemp;
            else Debug.LogError("The component 'HealthManager' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            if (TryGetComponent(out ShieldManager shieldManagerTemp)) shieldManagerCS = shieldManagerTemp;
            else Debug.LogError("The component 'ShieldManager' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");
        }
        private void CreateStateInstances()
        {
            idleState = new IdleState(this);
            lightningRainState = new LightningRainState(this);
            spreadFireState = new SpreadFireState(this);
            iceShieldState = new IceShieldState(this);
            naturesMelodyState = new NaturesMelodyState(this);
        }

        private void SetBaseValues()
        {
            currentAttackDamage = baseAttackDamage;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
            currentJumpForce = baseJumpForce;

            shieldManagerCS.SetShieldPointsValues(iceShieldHealthPerStack * iceShieldMaxStacks);
        }


        public IEnumerator CoroutineSpreadFireCooldown()
        {
            yield return new WaitForSecondsRealtime(spreadFireCooldownTime);
            spreadFireOnCooldown = false;
        }

        private void OnSpreadFireCooldownStart()
        {
            _coroutineSpreadFireCooldown = StartCoroutine(CoroutineSpreadFireCooldown());
        }

        private void OnSpreadFireCooldownEnd()
        {
            if (_coroutineSpreadFireCooldown != null) StopCoroutine(_coroutineSpreadFireCooldown);
        }
    }
}