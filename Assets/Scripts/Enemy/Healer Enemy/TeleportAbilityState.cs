using System.Collections;
using UnityEngine;

namespace HealerEnemy
{
    public class TeleportAbilityState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;

        public TeleportAbilityState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.teleportMat;

            _manager.TeleportRangeEnemy(_manager.teleportLocationFinderCS.GetRandomPosition(_manager.transform));
            _manager.TransitionToState(_manager.chaseState);
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.coroutineTeleportCooldown = _manager.StartCoroutine(_manager.CoroutineTeleportCooldown());
        }
    }
}