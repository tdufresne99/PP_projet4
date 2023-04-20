using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class ShieldAbilityState : TankEnemyState
    {
        private Coroutine _coroutineShieldUp;
        private TankEnemyStateManager _manager;

        public ShieldAbilityState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.shieldAbilityMat;

            _manager.transform.LookAt(_manager.targetTransform, Vector3.up);
            _manager.navMeshAgentManagerCS.ChangeAgentSpeed(0);

            _manager.enemyDamageReceiverCS.damageMultiplier = _manager.shieldDamageReduction;

            _coroutineShieldUp = _manager.StartCoroutine(CoroutineShieldUp());
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.enemyDamageReceiverCS.damageMultiplier = 1f;
            _manager.shieldActive = false;
            _manager.coroutineShieldCooldown = _manager.StartCoroutine(_manager.CoroutineShieldCooldown());
        }

        private IEnumerator CoroutineShieldUp()
        {
            _manager.shieldActive = true;
            yield return new WaitForSecondsRealtime(_manager.shieldUpTime);
            _manager.TransitionToState(_manager.chaseState);
        }
    }
}