using System.Collections;
using UnityEngine;

namespace MeleeEnemy
{
    public class DyingState : MeleeEnemyState
    {
        private MeleeEnemyStateManager _manager;

        public DyingState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            _manager.SelfDestruct();
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}