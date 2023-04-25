using System.Collections;
using UnityEngine;

namespace Player
{
    public class LightningRainState : PlayerState
    {
        private PlayerStateManager _manager;

        public LightningRainState(PlayerStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            _manager.abilityLocked = true;
            
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.abilityLocked = false;
            
        }
    }
}