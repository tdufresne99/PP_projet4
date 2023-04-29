using System.Collections;
using UnityEngine;

namespace Player
{
    public class DyingState : PlayerState
    {
        private PlayerStateManager _manager;

        public DyingState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.dyingMat;

            OnDeath();
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }

        private void OnDeath()
        {
            _manager.currentMovementSpeed = 0;
        }
    }
}