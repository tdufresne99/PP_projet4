using System.Collections;
using UnityEngine;

namespace Enemy.Tank
{
    public class CleaveAbilityState : TankEnemyState
    {
        private string _cleaveTrigger = "cleave";
        private TankEnemyStateManager _manager;
        private Coroutine _coroutineCleave;

        public CleaveAbilityState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.enemyAnimator.SetTrigger("cleaveCast");

            _manager.abilityLocked = true;

            _manager.transform.LookAt(_manager.targetTransform, Vector3.up);

            _coroutineCleave = _manager.StartCoroutine(PerformCleaveAttack());
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            _manager.cleaveHitboxGO.SetActive(false);
            _manager.abilityLocked = false;
            _manager.cleaveOnCooldown = true;
        }

        private IEnumerator PerformCleaveAttack()
        {
            yield return new WaitForSecondsRealtime(2f);
            _manager.cleaveHitboxGO.SetActive(true);
            _manager.enemyAnimator.SetTrigger("cleaveCast");
            yield return new WaitForSecondsRealtime(0.5f);
            _manager.TransitionToState(_manager.idleState);
        }
    }
}