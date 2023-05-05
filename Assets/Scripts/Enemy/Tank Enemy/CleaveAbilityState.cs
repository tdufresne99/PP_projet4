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
            Debug.Log("cleaveCast");
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
            _manager.tankAudioSource.volume = 1f;
            _manager.cleaveHitboxGO.SetActive(false);
            _manager.abilityLocked = false;
            _manager.cleaveOnCooldown = true;
        }

        private IEnumerator PerformCleaveAttack()
        {
            _manager.tankAudioSource.volume = 0.4f;
            _manager.tankAudioSource.PlayOneShot(_manager.cleaveSound);
            yield return new WaitForSecondsRealtime(3f);
            _manager.cleaveHitboxGO.SetActive(true);
            Debug.Log("cleave");
            _manager.enemyAnimator.SetTrigger("cleave");
            _manager.tankAudioSource.volume = 1f;
            yield return new WaitForSecondsRealtime(1f);
            _manager.TransitionToState(_manager.chaseState);
        }
    }
}