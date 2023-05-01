using System.Collections;
using UnityEngine;

namespace Enemy.Melee
{
    public class ResetState : MeleeEnemyState
    {
        private MeleeEnemyStateManager _manager;

        public ResetState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            
        }

        public override void Execute()
        {
            if(Vector3.Distance(_manager.transform.position, _manager.resetTransform.position) < 2f) _manager.TransitionToState(_manager.idleState);
        }

        public override void Exit()
        {
            
        }
    }
}