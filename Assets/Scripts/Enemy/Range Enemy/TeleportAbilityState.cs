using System.Collections;
using UnityEngine;

namespace Enemy.Range
{
    public class TeleportAbilityState : RangeEnemyState
    {
        private RangeEnemyStateManager _manager;

        public TeleportAbilityState(RangeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.teleportMat;

            TeleportRangeEnemy(_manager.teleportLocationFinderCS.GetRandomPosition(_manager.transform));
            _manager.TransitionToState(_manager.chaseState);
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.teleportOnCooldown = true;
        }

         public void TeleportRangeEnemy(Vector3 teleportPosition)
        {
            _manager.transform.position = teleportPosition;
            _manager.transform.LookAt(_manager.targetTransform);
        }
    }
}