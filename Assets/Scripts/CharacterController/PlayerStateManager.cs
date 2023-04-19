
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
        // ---------------------------------------------------------


        // ---- Calculated Values ----------------------------------
        [Header("Current values")]
        public float currentAttackDamage;
        public float currentAttackSpeed;
        public float currentMovementSpeed;
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

            // if (Input.GetKeyDown(KeyCode.Alpha1)) TransitionToState(chaseState);
            // if (Input.GetKeyDown(KeyCode.Alpha2)) TransitionToState(basicAttackState);
            // if (Input.GetKeyDown(KeyCode.Alpha3)) TransitionToState(resetState);
            // if (Input.GetKeyDown(KeyCode.Alpha4)) TransitionToState(idleState);
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
            else Debug.LogError("The component 'MeshRenderer' does not exist object " + gameObject.name + " at MeleeEnemyStateManager.cs");
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
        }

    }
}