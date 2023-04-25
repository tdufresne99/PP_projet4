using System.Collections;
using UnityEngine;

namespace Player
{
    public class BasicAttackState : PlayerState
    {
        private PlayerStateManager _manager;

        public BasicAttackState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.basicAttackMat;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}