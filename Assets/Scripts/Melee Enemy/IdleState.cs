using System.Collections;
using UnityEngine;

namespace MeleeEnemy
{
    public class IdleState : MeleeEnemyState
    {
        private float detectionRange = 5f;

        private MeleeEnemyStateManager _manager;

        public IdleState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            Debug.Log(_manager.gameObject.name + " is now idle");

            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.idleMat;
        }

        public override void Execute()
        {
            if (_manager.DetectObject(_manager.targetTransform, detectionRange, _manager.targetLayerMask)) _manager.TransitionToState(_manager.chaseState);
        }

        public override void Exit()
        {
            
        }
    }
}