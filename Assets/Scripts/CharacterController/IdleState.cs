using System.Collections;
using UnityEngine;

namespace Player
{
    public class IdleState : PlayerState
    {
        private PlayerStateManager _manager;

        public IdleState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }

        private bool VerifyMovementKeys()
        {
            if(Input.GetAxis("Horizontal") != 0) return true;
            if(Input.GetAxis("Vertical") != 0) return true;
            return false;
        }
    }
}