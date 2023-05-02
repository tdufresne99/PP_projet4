using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        // ---------------------------------------------------------
        #region State Test Animations
        [Header("State Test Animations")]
        public Material idleMat;
        public Material basicAttackMat;
        public Material spreadFireMat;
        public Material lightningRainMat;
        public Material iceShieldMat;
        public Material naturesMelodyMat;
        public Material dyingMat;
        #endregion
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
        public DyingState dyingState;
        public LightningRainState lightningRainState;
        public SpreadFireState spreadFireState;
        public IceShieldState iceShieldState;
        public NaturesMelodyState naturesMelodyState;
        #endregion
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        #region Internal Components
        [Header("Internal Components")]
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
        public GameObject meleeHitboxGO;
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

        #region Base Health Settings
        [Header("Base Health Settings")]
        public float maxHealthPoints = 400f;
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
        public int spreadFireLevel = 1;
        public float spreadFireRange = 12f;
        [SerializeField] private float _spreadFireDamageMultiplier = 6f;
        public float spreadFireDamage => currentAttackDamage * _spreadFireDamageMultiplier;
        public float spreadFireDamageIncreaseDebuff => (spreadFireLevel > 1) ? 0.15f : 0;
        public int spreadFireTicks => (spreadFireLevel > 2) ? 9 : 5;
        public float spreadFireDuration => (spreadFireLevel > 2) ? 15 : 8;
        public float spreadFireCooldownTime = 12f;
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
        public bool playerCompletedTrials = false;
        public int currentLifes = 3;
        public float currentAttackDamage;
        public float currentDamageMultiplier = 1f;
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
        public bool spreadFirePressed = false;
        public bool lightningRainPressed = false;
        public bool iceShieldPressed = false;
        public bool naturesMelodyPressed = false;
        public bool abilityPressed => (spreadFirePressed || lightningRainPressed || iceShieldPressed || naturesMelodyPressed);
        public bool isDead = false;
        #endregion
        // ---------------------------------------------------------

        void Awake()
        {
            TryGetRequiredComponents();
            SubscribeToRequiredEvents();
        }

        private void Start()
        {
            SetBaseValues();
            CreateStateInstances();
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
                if (Input.GetKeyDown(KeyCode.E) && !spreadFireOnCooldown && !abilityPressed)
                {
                    spreadFirePressed = true;
                    ChangeIconAlpha(spreadFireCDIcon, true);
                }
                if (Input.GetKeyUp(KeyCode.E) && !spreadFireOnCooldown && spreadFirePressed)
                {
                    spreadFirePressed = false;
                    TransitionToState(spreadFireState);
                }

                // Lightning Rain
                if (Input.GetKeyDown(KeyCode.Q) && !lightningRainOnCooldown && !abilityPressed)
                {
                    ChangeIconAlpha(lightningRainCDIcon, true);
                    TransitionToState(lightningRainState);
                }

                // Ice Shield
                if (Input.GetKeyDown(KeyCode.LeftShift) && !iceShieldOnCooldown && !abilityPressed)
                {
                    iceShieldPressed = true;
                    ChangeIconAlpha(iceShieldCDIcon, true);
                }
                if (Input.GetKeyUp(KeyCode.LeftShift) && !iceShieldOnCooldown && iceShieldPressed)
                {
                    iceShieldPressed = false;
                    TransitionToState(iceShieldState);
                }

                // Nature's Melody
                if (Input.GetKeyDown(KeyCode.R) && !naturesMelodyOnCooldown && !abilityPressed)
                {
                    ChangeIconAlpha(naturesMelodyCDIcon, true);
                    TransitionToState(naturesMelodyState);
                }
            }

            horizontalIntput = Input.GetAxis("Horizontal");

            float verticalIntput = Input.GetAxis("Vertical");

            playerMovement = transform.forward * verticalIntput;
            playerMovement = playerMovement.normalized * currentMovementSpeed;

            playerAnimator.SetFloat("horizontal", horizontalIntput);
            playerAnimator.SetFloat("vertical", verticalIntput);

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
            if (playerIsGrounded) 
            {
                playerRigidbody.velocity = playerMovement + (transform.up * playerRigidbody.velocity.y);
                bool isRunning = (Mathf.Abs(playerRigidbody.velocity.x) > 0.1f || Mathf.Abs(playerRigidbody.velocity.z) > 0.1f);
            }

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
            if (newState == null)
            {
                // TransitionToState(idleState);
                return;
            }
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;
            currentState.Enter();
        }

        private void TryGetRequiredComponents()
        {
            if (TryGetComponent(out Rigidbody playerRigidbodyTemp)) playerRigidbody = playerRigidbodyTemp;
            else Debug.LogError("The component 'Rigidbody' does not exist on object " + gameObject.name + " (PlayerStateManager.cs)");

            var animator = GetComponentInChildren<Animator>();
            if (animator != null) playerAnimator = animator;
            else Debug.LogError("The component 'Animator' does not exist on object " + gameObject.name + "'s children (PlayerStateManager.cs)");

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
            dyingState = new DyingState(this);

            if (playerCompletedTrials)
            {
                spreadFireState = new SpreadFireState(this);
                iceShieldState = new IceShieldState(this);
                naturesMelodyState = new NaturesMelodyState(this);
                lightningRainState = new LightningRainState(this);
            }
        }

        private void SetBaseValues()
        {
            currentAttackDamage = baseAttackDamage;
            currentLeech = baseLeech;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
            currentJumpForce = baseJumpForce;

            shieldManagerCS.SetShieldPointsValues(iceShieldHealthPerStack * iceShieldMaxStacks);
            shieldManagerCS.currentShieldPoints = 0;

            healthManagerCS.SetHealthPointsValues(maxHealthPoints);

            if(LevelManager.instance != null) playerCompletedTrials = true;
        }

        private void SubscribeToRequiredEvents()
        {
            healthManagerCS.OnHealthPointsEmpty += OnHealthPointsEmpty;

            playerDamageDealerCS.OnDamageDealt += OnDamageDealt;
            playerDamageReceiverCS.OnDamageReceived += OnDamageReceived;
            playerHealingDealerCS.OnHealingDealt += OnHealingDealt;
            playerHealingReceiverCS.OnHealingReceived += OnHealingReceived;
        }

        public void ResetPlayer(Transform resetPosition)
        {
            transform.position = resetPosition.position;
            transform.rotation = resetPosition.rotation;

            spreadFireRemainingCooldownTime = 0;
            lightningRainRemainingCooldownTime = 0;
            iceShieldRemainingCooldownTime = 0;
            naturesMelodyRemainingCooldownTime = 0;

            SetBaseValues();

            TransitionToState(idleState);
        }

        public void OnAbilityLearned(PlayerAbilityEnum ability)
        {
            switch (ability)
            {
                case PlayerAbilityEnum.SpreadFire:
                    if(spreadFireLevel < 1) 
                    {
                        spreadFireState = new SpreadFireState(this);
                        ChangeIconAlpha(spreadFireCDIcon, false);
                    }
                    spreadFireLevel++;
                    break;

                case PlayerAbilityEnum.IceShield:
                    iceShieldState = new IceShieldState(this);
                    ChangeIconAlpha(iceShieldCDIcon, false);
                    break;

                case PlayerAbilityEnum.NaturesMelody:
                    naturesMelodyState = new NaturesMelodyState(this);
                    ChangeIconAlpha( naturesMelodyCDIcon, false);
                    break;

                case PlayerAbilityEnum.LightningRain:
                    lightningRainState = new LightningRainState(this);
                    ChangeIconAlpha( lightningRainCDIcon, false);
                    break;

                default:
                    break;
            }
        }

        private void OnHealthPointsEmpty(HealthManager hm)
        {
            TransitionToState(dyingState);
            OnPlayerDeath?.Invoke();
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
            if (desaturate)
            {
                icon.color = new Color(1f, 1f, 1f, desaturatedIconAlphaValue);
            }
            else
            {
                icon.color = new Color(1f, 1f, 1f, 1f);
            }
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

        public event Action OnPlayerDeath;
    }
}