using System.Collections;
using UnityEngine;

namespace Enemy.Melee
{
    public class IdleState : MeleeEnemyState
    {
        private MeleeEnemyStateManager _manager;

        public IdleState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetBool("isRunning", false);
        }

        public override void Execute()
        {
            if (_manager.DetectObject(_manager.targetTransform, _manager.detectionRange, _manager.targetLayerMask)) _manager.TransitionToState(_manager.chaseState);
        }

        public override void Exit()
        {
            
        }
    }
}