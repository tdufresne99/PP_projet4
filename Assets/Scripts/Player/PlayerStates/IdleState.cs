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
            // ---- Set state animations ------------------------------
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}