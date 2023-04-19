
using UnityEngine;
using System.Collections;

namespace Player
{
    public class PlayerStateManager : MonoBehaviour
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


        // ---- Player States ---------------------------------
        public PlayerState currentState;

        public IdleState idleState;
        public RunState runState;
        // ---------------------------------------------------------


        // ---- Player Components -----------------------------
        [Header("Internal Components")]
        public MeshRenderer meshRenderer;
        public Rigidbody playerRigidbody;
        // ---------------------------------------------------------


        // ---- External References --------------------------------
        [Header("External references")]
        public Transform spawnTransform;
        public Transform targetTransform;
        public LayerMask targetLayerMask;
        // ---------------------------------------------------------


        // ---- Coroutines -----------------------------------------

        // ---------------------------------------------------------


        // ---- Ajustable Values -----------------------------------
        [Header("Base Attack Settings")]
        public float baseAttackRange = 2.2f;
        public float baseAttackDamage = 20f;
        public float baseAttackSpeed = 1.5f;


        [Header("Base Movement Settings")]
        public float baseMovementSpeed = 20f;
        public float baseJumpForce = 20f;
        // ---------------------------------------------------------


        // ---- Calculated Values ----------------------------------
        [Header("Current values")]
        public float currentAttackDamage;
        public float currentAttackSpeed;
        public float currentMovementSpeed;
        public float currentJumpForce;
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
            else Debug.LogError("The component 'MeshRenderer' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");

            if (TryGetComponent(out Rigidbody playerRigidbodyTemp)) playerRigidbody = playerRigidbodyTemp;
            else Debug.LogError("The component 'Rigidbody' does not exist on object " + gameObject.name + " (MeleeEnemyStateManager.cs)");
        }
        private void CreateStateInstances()
        {
            idleState = new IdleState(this);
            runState = new RunState(this);
        }

        private void SetBaseValues()
        {
            currentAttackDamage = baseAttackDamage;
            currentAttackSpeed = baseAttackSpeed;
            currentMovementSpeed = baseMovementSpeed;
            currentJumpForce = baseJumpForce;
        }
    }
}