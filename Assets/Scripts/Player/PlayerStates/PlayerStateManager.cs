
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

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
        public float spreadFireRemainingCooldownTime;

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
        public float lightningRainRemainingCooldownTime;

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
        public float iceShieldRemainingCooldownTime;

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
        public float naturesMelodyRemainingCooldownTime;
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
        [HideInInspector] public PlayerDamageDealer playerDamageDealerCS;
        [HideInInspector] public PlayerDamageReceiver playerDamageReceiverCS;
        [HideInInspector] public PlayerHealingDealer playerHealingDealerCS;
        [HideInInspector] public PlayerHealingReceiver playerHealingReceiverCS;
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
        public TextMeshProUGUI spreadFireCDText;
        public TextMeshProUGUI lightningRainCDText;
        public TextMeshProUGUI iceShieldCDText;
        public TextMeshProUGUI naturesMelodyCDText;
        public Image spreadFireCDIcon;
        public Image lightningRainCDIcon;
        public Image iceShieldCDIcon;
        public Image naturesMelodyCDIcon;
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
        public float baseAttackDamage = 30f;
        public float baseLeech = 0;
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

        #region Global Ability Settings
        public float desaturatedIconAlphaValue = 0.1f;
        #endregion

        #region SpreadFire Ability Settings
        [Header("SpreadFire Ability Settings")]
        public float spreadFireRange = 12f;
        [SerializeField] private float _spreadFireDamageMultiplier = 6f;
        public float spreadFireDamage => currentAttackDamage * _spreadFireDamageMultiplier;
        public int spreadFireTicks = 5;
        public float spreadFireDuration = 8f;
        public float spreadFireCooldownTime = 45f;
        public string spreadFireKey = "E";
        #endregion

        #region LightningRain Ability Settings
        [Header("LightningRain Ability Settings")]
        public float lightningRainRadius = 8f;
        [SerializeField] private float lightningRainDamagePerChargeMultiplier = 3f;
        public int lightningRainMaxCharges = 3;
        public float lightningRainDamagePerCharge => currentAttackDamage * lightningRainDamagePerChargeMultiplier;
        public float lightningRainCooldownTime = 100f;
        public float lightningRainActivationDelay = 3f;
        public float lightningRainStunDuration = 3f;
        [SerializeField] private float _lightningRainMoveSpeedMultiplier = 0.25f;
        public float lightningRainMoveSpeed => currentMovementSpeed * _lightningRainMoveSpeedMultiplier;
        public string lightningRainKey = "Q";
        #endregion

        #region IceShield Ability Settings
        [Header("IceShield Ability Settings")]
        public int iceShieldMaxStacks = 4;
        public float iceShieldHealthPerStack = 50f;
        public float iceShieldCooldownTime = 100f;
        public float iceShieldRange = 12f;
        public string iceShieldKey = "Shift";
        #endregion

        #region NaturesMelody Ability Settings
        [Header("NaturesMelody Ability Settings")]
        public float naturesMelodyCooldownTime = 100f;
        public float naturesMelodyTickTime = 0.25f;
        [SerializeField] private float _naturesMelodyMoveSpeedMultiplier = 0f;
        public float naturesMelodyMoveSpeed => currentMovementSpeed * _naturesMelodyMoveSpeedMultiplier;
        public int naturesMelodyMaxTicks = 10;
        public string naturesMelodyKey = "R";
        #endregion
        // << ========================
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Current Values
        [Header("Current values")]
        public float currentAttackDamage;
        public float currentLeech;
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

            if (abilityLocked == false)
            {
                // Basic Attack
                if (Input.GetButtonDown("Fire1"))
                {
                    TransitionToState(basicAttackState);
                }

                // Spread Fire
                if (Input.GetKeyDown(KeyCode.E) && !spreadFireOnCooldown)
                {
                    ChangeIconAlpha(spreadFireCDIcon, true);
                }
                if (Input.GetKeyUp(KeyCode.E) && !spreadFireOnCooldown)
                {
                    TransitionToState(spreadFireState);
                }

                // Lightning Rain
                if (Input.GetKeyDown(KeyCode.Q) && !lightningRainOnCooldown) 
                {
                    ChangeIconAlpha(lightningRainCDIcon, true);
                    TransitionToState(lightningRainState);
                }

                // Ice Shield
                if (Input.GetKeyDown(KeyCode.LeftShift) && !iceShieldOnCooldown) 
                {
                    ChangeIconAlpha(iceShieldCDIcon, true);
                }
                if (Input.GetKeyUp(KeyCode.LeftShift) && !iceShieldOnCooldown) 
                {
                    TransitionToState(iceShieldState);
                }

                // Nature's Melody
                if (Input.GetKeyDown(KeyCode.R) && !naturesMelodyOnCooldown) 
                {
                    ChangeIconAlpha(naturesMelodyCDIcon, true);
                    TransitionToState(naturesMelodyState);
                }
            }

            horizontalIntput = Input.GetAxis("Horizontal");

            float vertical = Input.GetAxis("Vertical");

            playerMovement = transform.forward * vertical;
            playerMovement = playerMovement.normalized * currentMovementSpeed;

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

            if (TryGetComponent(out PlayerDamageDealer playerDamageDealerTemp)) playerDamageDealerCS = playerDamageDealerTemp;
            else Debug.LogError("The component 'PlayerDamageDealer' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            if (TryGetComponent(out PlayerDamageReceiver playerDamageReceiverTemp)) playerDamageReceiverCS = playerDamageReceiverTemp;
            else Debug.LogError("The component 'PlayerDamageReceiver' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            if (TryGetComponent(out PlayerHealingReceiver playerHealingReceiverTemp)) playerHealingReceiverCS = playerHealingReceiverTemp;
            else Debug.LogError("The component 'PlayerHealingReceiver' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            if (TryGetComponent(out PlayerHealingDealer playerHealingDealerTemp)) playerHealingDealerCS = playerHealingDealerTemp;
            else Debug.LogError("The component 'PlayerHealingDealer' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");
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
            currentLeech = baseLeech;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
            currentJumpForce = baseJumpForce;

            shieldManagerCS.SetShieldPointsValues(iceShieldHealthPerStack * iceShieldMaxStacks);
        }

        private void SubscribeToRequiredEvents()
        {
            playerDamageDealerCS.OnDamageDealt += OnDamageDealt;
            playerDamageReceiverCS.OnDamageReceived += OnDamageReceived;
            playerHealingDealerCS.OnHealingDealt += OnHealingDealt;
            playerHealingReceiverCS.OnHealingReceived += OnHealingReceived;
        }

        private void OnDamageDealt(float damageDealt)
        {

        }

        private void OnDamageReceived(float damageReceived)
        {
            Debug.Log("-" + damageReceived);
        }

        private void OnHealingDealt(float healingDealt)
        {

        }

        private void OnHealingReceived(float healingreceived)
        {

        }

        private void ChangeIconAlpha(Image icon, bool desaturate)
        {
            if (desaturate) icon.color = new Color(1f, 1f, 1f, desaturatedIconAlphaValue);
            else new Color(1f, 1f, 1f, 1f);
        }

        // ===================================================================================================
        #region Spread Fire Cooldown
        public IEnumerator CoroutineSpreadFireCooldown()
        {
            spreadFireCDText.text = spreadFireRemainingCooldownTime + "";
            while (spreadFireRemainingCooldownTime > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                spreadFireRemainingCooldownTime -= 1f;
                spreadFireCDText.text = spreadFireRemainingCooldownTime + "";
            }
            spreadFireOnCooldown = false;
        }

        private void OnSpreadFireCooldownStart()
        {
            spreadFireRemainingCooldownTime = spreadFireCooldownTime;
            ChangeIconAlpha(spreadFireCDIcon, true);
            _coroutineSpreadFireCooldown = StartCoroutine(CoroutineSpreadFireCooldown());
        }

        private void OnSpreadFireCooldownEnd()
        {
            if (_coroutineSpreadFireCooldown != null) StopCoroutine(_coroutineSpreadFireCooldown);
            ChangeIconAlpha(spreadFireCDIcon, false);
            spreadFireCDText.text = spreadFireKey;
        }
        #endregion

        // ===================================================================================================
        #region Lightning Rain Cooldown
        public IEnumerator CoroutineLightningRainCooldown()
        {
            lightningRainCDText.text = lightningRainRemainingCooldownTime + "";
            while (lightningRainRemainingCooldownTime > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                lightningRainRemainingCooldownTime -= 1f;
                lightningRainCDText.text = lightningRainRemainingCooldownTime + "";
            }
            lightningRainOnCooldown = false;
        }

        private void OnLightningRainCooldownStart()
        {
            lightningRainRemainingCooldownTime = lightningRainCooldownTime;
            ChangeIconAlpha(lightningRainCDIcon, true);
            _coroutineLightningRainCooldown = StartCoroutine(CoroutineLightningRainCooldown());
        }

        private void OnLightningRainCooldownEnd()
        {
            if (_coroutineLightningRainCooldown != null) StopCoroutine(_coroutineLightningRainCooldown);
            ChangeIconAlpha(lightningRainCDIcon, false);
            lightningRainCDText.text = lightningRainKey;
        }
        #endregion

        // ===================================================================================================
        #region Ice Shield Cooldown
        public IEnumerator CoroutineIceShieldCooldown()
        {
            iceShieldCDText.text = iceShieldRemainingCooldownTime + "";
            while (iceShieldRemainingCooldownTime > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                iceShieldRemainingCooldownTime -= 1f;
                iceShieldCDText.text = iceShieldRemainingCooldownTime + "";
            }
            iceShieldOnCooldown = false;
        }

        private void OnIceShieldCooldownStart()
        {
            iceShieldRemainingCooldownTime = iceShieldCooldownTime;
            ChangeIconAlpha(iceShieldCDIcon, true);
            _coroutineIceShieldCooldown = StartCoroutine(CoroutineIceShieldCooldown());
        }

        private void OnIceShieldCooldownEnd()
        {
            if (_coroutineIceShieldCooldown != null) StopCoroutine(_coroutineIceShieldCooldown);
            ChangeIconAlpha(iceShieldCDIcon, false);
            iceShieldCDText.text = iceShieldKey;
        }
        #endregion

        // ===================================================================================================
        #region Nature's Melody Cooldown
        public IEnumerator CoroutineNaturesMelodyCooldown()
        {
            naturesMelodyCDText.text = naturesMelodyRemainingCooldownTime + "";
            while (naturesMelodyRemainingCooldownTime > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
                naturesMelodyRemainingCooldownTime -= 1f;
                naturesMelodyCDText.text = naturesMelodyRemainingCooldownTime + "";
            }
            naturesMelodyOnCooldown = false;
        }

        private void OnNaturesMelodyCooldownStart()
        {
            naturesMelodyRemainingCooldownTime = naturesMelodyCooldownTime;
            ChangeIconAlpha(naturesMelodyCDIcon, true);
            _coroutineNaturesMelodyCooldown = StartCoroutine(CoroutineNaturesMelodyCooldown());
        }

        private void OnNaturesMelodyCooldownEnd()
        {
            if (_coroutineNaturesMelodyCooldown != null) StopCoroutine(_coroutineNaturesMelodyCooldown);
            ChangeIconAlpha(naturesMelodyCDIcon, false);
            naturesMelodyCDText.text = naturesMelodyKey;
        }
        #endregion
    }
}