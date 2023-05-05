using System.Collections;
using UnityEngine;

namespace Enemy.Healer
{
    public class TeleportAbilityState : HealerEnemyState
    {
        private HealerEnemyStateManager _manager;
        private Coroutine _coroutineTeleport;

        public TeleportAbilityState(HealerEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------

            _coroutineTeleport = _manager.StartCoroutine(CoroutineTeleportRangeEnemy(_manager.teleportLocationFinderCS.GetRandomPosition(_manager.transform)));
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.teleportOnCooldown = true;
        }

        public IEnumerator CoroutineTeleportRangeEnemy(Vector3 teleportPosition)
        {
            _manager.enemyAnimator.SetTrigger("teleport");
            yield return new WaitForSecondsRealtime(0.7f);
            _manager.healerAudioSource.PlayOneShot(_manager.teleportSound);
            _manager.transform.position = teleportPosition;
            _manager.transform.LookAt(_manager.targetTransform);
            _manager.TransitionToState(_manager.chaseState);
        }
    }
}