using System.Collections;
using UnityEngine;

namespace Enemy.Tank
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
            _manager.enemyAnimator.SetBool("iceShield", true);
            
            _manager.abilityLocked = true;

            _manager.transform.LookAt(_manager.targetTransform, Vector3.up);
            _manager.currentMovementSpeed = 0;

            _manager.enemyDamageReceiverCS.damageMultiplier *= _manager.shieldDamageReduction;

            _coroutineShieldUp = _manager.StartCoroutine(CoroutineShieldUp());
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.enemyAnimator.SetBool("iceShield", false);
            _manager.enemyDamageReceiverCS.damageMultiplier /= _manager.shieldDamageReduction;
            _manager.abilityLocked = false;
            _manager.shieldOnCooldown = true;
        }

        private IEnumerator CoroutineShieldUp()
        {
            yield return new WaitForSecondsRealtime(_manager.shieldUpTime);
            _manager.TransitionToState(_manager.chaseState);
        }
    }
}