using UnityEngine;

namespace TankEnemy
{
    public class TankEnemyStateManager : MonoBehaviour
    {
        // ---- State Test materials ------------------------------
        public Material idleMat;
        public Material chaseMat;
        public Material basicAttackMat;
        public Material shieldAbilityMat;
        public Material cleaveAbilityMat;
        public Material resetMat;
        // --------------------------------------------------------


        // ---- Tank Enemy States ---------------------------------
        public TankEnemyState currentState;
        
        public IdleState idleState;
        public ChaseState chaseState;
        public BasicAttackState basicAttackState;
        public ShieldAbilityState shieldAbilityState;
        public CleaveAbilityState cleaveAbilityState;
        public ResetState resetState;
        // --------------------------------------------------------


        // ---- Tank Enemy Components ---------------------------------
        public MeshRenderer meshRenderer;
        // --------------------------------------------------------

        private void Start()
        {
            CreateStateInstances();
            TransitionToState(idleState);
        }

        void Update()
        {
            if (currentState == null)
            {
                TransitionToState(idleState);
            }

            currentState.Execute();

            if (Input.GetKeyDown(KeyCode.Alpha1)) TransitionToState(chaseState);
            if (Input.GetKeyDown(KeyCode.Alpha2)) TransitionToState(basicAttackState);
            if (Input.GetKeyDown(KeyCode.Alpha3)) TransitionToState(shieldAbilityState);
            if (Input.GetKeyDown(KeyCode.Alpha4)) TransitionToState(cleaveAbilityState);
            if (Input.GetKeyDown(KeyCode.Alpha5)) TransitionToState(resetState);
            if (Input.GetKeyDown(KeyCode.Alpha6)) TransitionToState(idleState);
        }

        public void TransitionToState(TankEnemyState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;
            currentState.Enter();
        }

        private void CreateStateInstances()
        {
            idleState = new IdleState(this);
            chaseState = new ChaseState(this);
            basicAttackState = new BasicAttackState(this);
            shieldAbilityState = new ShieldAbilityState(this);
            cleaveAbilityState = new CleaveAbilityState(this);
            resetState = new ResetState(this);
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
    }
}