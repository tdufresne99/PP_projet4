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

            _manager.playerAnimator.SetBool("meleeAttack", true);
        }

        public override void Execute()
        {
            if(Input.GetButtonUp("Fire1") || !Input.GetButton("Fire1")) _manager.TransitionToState(_manager.idleState);
        }

        public override void Exit()
        {
            _manager.playerAnimator.SetBool("meleeAttack", false);
        }
    }
}