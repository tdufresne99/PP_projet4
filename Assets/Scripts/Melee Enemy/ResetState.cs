using System.Collections;
using UnityEngine;

namespace MeleeEnemy
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
            Debug.Log(_manager.gameObject.name + " is now resetting");
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.resetMat;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}