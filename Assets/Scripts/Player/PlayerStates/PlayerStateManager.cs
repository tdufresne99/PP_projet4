
using UnityEngine;
using System.Collections;

namespace Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        // ---------------------------------------------------------
        #region State Test Materials
        [Header("State Test Materials")]
        public Material idleMat;
        public Material basicAttackMat;
        public Material spreadFireMat;
        public Material lightningRainMat;
        public Material iceShieldMat;
        public Material naturesMelodyMat;
        public Material dyingMat;
        #endregion State Test Materials
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Cooldown States
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


        [SerializeField] private bool _lightningRainOnCooldown = false;
        public bool lightningRainOnCooldown
        {
            get => _lightningRainOnCooldown;
            set
            {
                if (_lightningRainOnCooldown == value) return;

                if (value == true) OnLightningRainCooldownStart();
                else OnLightningRainCooldownEnd();

                _lightningRainOnCooldown = value;
            }
        }


        [SerializeField] private bool _iceShieldOnCooldown = false;
        public bool iceShieldOnCooldown
        {
            get => _iceShieldOnCooldown;
            set
            {
                if (_iceShieldOnCooldown == value) return;

                if (value == true) OnIceShieldCooldownStart();
                else OnIceShieldCooldownEnd();

                _iceShieldOnCooldown = value;
            }
        }


        [SerializeField] private bool _naturesMelodyOnCooldown = false;
        public bool naturesMelodyOnCooldown
        {
            get => _naturesMelodyOnCooldown;
            set
            {
                if (_naturesMelodyOnCooldown == value) return;

                if (value == true) OnNaturesMelodyCooldownStart();
                else OnNaturesMelodyCooldownEnd();

                _naturesMelodyOnCooldown = value;
            }
        }
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Player States
        public PlayerState currentState;

        public IdleState idleState;
        public BasicAttackState basicAttackState;
        public LightningRainState lightningRainState;
        public SpreadFireState spreadFireState;
        public IceShieldState iceShieldState;
        public NaturesMelodyState naturesMelodyState;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Internal Components
        [Header("Internal Components")]
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public Rigidbody playerRigidbody;
        [HideInInspector] public Animator playerAnimator;
        [HideInInspector] public HealthManager healthManagerCS;
        [HideInInspector] public ShieldManager shieldManagerCS;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region External References
        [Header("External references")]
        public Transform spawnTransform;
        public Transform targetTransform;
        public Transform groundCheckTransform;
        public LayerMask targetLayerMask;
        public LayerMask groundLayerMask;
        public Camera mainCamera;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Coroutines
        private Coroutine _coroutineSpreadFireCooldown;
        private Coroutine _coroutineLightningRainCooldown;
        private Coroutine _coroutineIceShieldCooldown;
        private Coroutine _coroutineNaturesMelodyCooldown;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Ajustable Values
        // ======================== >>
        #region Base Attack Settings
        [Header("Base Attack Settings")]
        public float baseAttackRange = 2.2f;
        public float baseAttackDamage = 20f;
        public float baseAttackSpeed = 1.5f;
        #endregion

        #region Base Movement Settings
        [Header("Base Movement Settings")]
        public float baseMovementSpeed = 20f;
        public float baseJumpForce = 20f;
        public float gravityMultiplier = 20f;
        public float rotateSpeed = 5f;
        public float mouseSensitivity = 100f;

        #endregion

        #region SpreadFire Ability Settings
        [Header("SpreadFire Ability Settings")]
        public float spreadFireRange = 12f;
        public float spreadFireDamage = 120f;
        public float spreadFireDuration = 8f;
        public float spreadFireCooldownTime = 45f;
        public int spreadFireTicks = 5;
        #endregion

        #region LightningRain Ability Settings
        [Header("LightningRain Ability Settings")]
        public float lightningRainRadius = 8f;
        public float lightningRainDamagePerCharge = 40f;
        public float lightningRainCooldownTime = 100f;
        public float lightningRainActivationDelay = 3f;
        public float lightningRainStunDuration = 3f;
        public int lightningRainMaxCharges = 3;
        #endregion

        #region IceShield Ability Settings
        [Header("IceShield Ability Settings")]
        public int iceShieldMaxStacks = 4;
        public float iceShieldHealthPerStack = 50f;
        public float iceShieldCooldownTime = 100f;
        #endregion

        #region NaturesMelody Ability Settings
        [Header("NaturesMelody Ability Settings")]
        public float naturesMelodyCooldownTime = 100f;
        public float naturesMelodyTickTime = 0.25f;
        public int naturesMelodyMaxTicks = 10;
        #endregion
        // << ========================
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Current Values
        [Header("Current values")]
        public float currentAttackDamage;
        public float currentAttackSpeed;
        public float currentMovementSpeed;
        public Vector3 playerMovement;
        public float horizontalIntput;
        public float currentJumpForce;
        public bool playerHoldingJump = false;
        public bool playerIsFastFalling = false;
        public bool playerIsGrounded;
        public bool abilityLocked = false;
        #endregion
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
                // Basic Attack
                if (Input.GetButtonDown("Fire1")) TransitionToState(basicAttackState);

                // Spread Fire
                if (Input.GetKeyDown(KeyCode.E) && !spreadFireOnCooldown) TransitionToState(spreadFireState);

                // Lightning Rain
                else if (Input.GetKeyDown(KeyCode.Q) && !lightningRainOnCooldown) TransitionToState(lightningRainState);

                // Ice Shield
                else if (Input.GetKeyDown(KeyCode.LeftShift) && !iceShieldOnCooldown) TransitionToState(iceShieldState);

                // Nature's Melody
                else if (Input.GetKeyDown(KeyCode.R) && !naturesMelodyOnCooldown) TransitionToState(naturesMelodyState);
            }

            horizontalIntput = Input.GetAxis("Horizontal");

            float vertical = Input.GetAxis("Vertical");

            playerMovement = transform.forward * vertical;
            playerMovement = playerMovement.normalized * currentMovementSpeed * Time.deltaTime;

            playerIsGrounded = Physics.CheckSphere(groundCheckTransform.position, 0.2f, groundLayerMask);
            Color debugColor = playerIsGrounded ? Color.green : Color.red;
            Debug.DrawRay(groundCheckTransform.position, Vector3.down * 0.2f, debugColor);

            if (Input.GetKeyDown(KeyCode.Space)) playerHoldingJump = true;
            if (Input.GetKeyUp(KeyCode.Space)) playerHoldingJump = false;

            playerIsFastFalling = (!playerIsGrounded && playerRigidbody.velocity.y < 0);

            currentState.Execute();
        }

        void FixedUpdate()
        {
            if (playerIsGrounded) playerRigidbody.velocity = playerMovement + (transform.up * playerRigidbody.velocity.y);

            transform.Rotate(Vector3.up, horizontalIntput * rotateSpeed);

            if (playerHoldingJump && playerIsGrounded) 
            {
                playerRigidbody.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
                playerHoldingJump = false;
            }

            if (playerIsFastFalling) playerRigidbody.AddForce((Vector3.down * gravityMultiplier), ForceMode.Acceleration);
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

            if (TryGetComponent(out Animator playerAnimatorTemp)) playerAnimator = playerAnimatorTemp;
            else Debug.LogError("The component 'Animator' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            if (TryGetComponent(out HealthManager healthManagerTemp)) healthManagerCS = healthManagerTemp;
            else Debug.LogError("The component 'HealthManager' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            if (TryGetComponent(out ShieldManager shieldManagerTemp)) shieldManagerCS = shieldManagerTemp;
            else Debug.LogError("The component 'ShieldManager' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");
        }

        private void CreateStateInstances()
        {
            idleState = new IdleState(this);
            basicAttackState = new BasicAttackState(this);
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

        // ===================================================================================================
        #region Spread Fire Cooldown
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
        #endregion

        // ===================================================================================================
        #region Lightning Rain Cooldown
        public IEnumerator CoroutineLightningRainCooldown()
        {
            yield return new WaitForSecondsRealtime(lightningRainCooldownTime);
            lightningRainOnCooldown = false;
        }

        private void OnLightningRainCooldownStart()
        {
            _coroutineLightningRainCooldown = StartCoroutine(CoroutineLightningRainCooldown());
        }

        private void OnLightningRainCooldownEnd()
        {
            if (_coroutineLightningRainCooldown != null) StopCoroutine(_coroutineLightningRainCooldown);
        }
        #endregion

        // ===================================================================================================
        #region Ice Shield Cooldown
        public IEnumerator CoroutineIceShieldCooldown()
        {
            yield return new WaitForSecondsRealtime(iceShieldCooldownTime);
            iceShieldOnCooldown = false;
        }

        private void OnIceShieldCooldownStart()
        {
            _coroutineIceShieldCooldown = StartCoroutine(CoroutineIceShieldCooldown());
        }

        private void OnIceShieldCooldownEnd()
        {
            if (_coroutineIceShieldCooldown != null) StopCoroutine(_coroutineIceShieldCooldown);
        }
        #endregion

        // ===================================================================================================
        #region Nature's Melody Cooldown
        public IEnumerator CoroutineNaturesMelodyCooldown()
        {
            yield return new WaitForSecondsRealtime(naturesMelodyCooldownTime);
            naturesMelodyOnCooldown = false;
        }

        private void OnNaturesMelodyCooldownStart()
        {
            _coroutineNaturesMelodyCooldown = StartCoroutine(CoroutineNaturesMelodyCooldown());
        }

        private void OnNaturesMelodyCooldownEnd()
        {
            if (_coroutineNaturesMelodyCooldown != null) StopCoroutine(_coroutineNaturesMelodyCooldown);
        }
        #endregion
    }
}