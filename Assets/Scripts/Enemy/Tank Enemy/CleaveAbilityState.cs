using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class CleaveAbilityState : TankEnemyState
    {
        private string _cleaveTrigger = "cleave";
        private TankEnemyStateManager _manager;

        public CleaveAbilityState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.cleaveAbilityMat;

            PerformCleaveAttack();
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            _manager.cleaveActive = false;
            _manager.coroutineCleaveCooldown = _manager.StartCoroutine(_manager.CoroutineCleaveCooldown());
        }

        private void PerformCleaveAttack()
        {
            _manager.cleaveActive = true;
            _manager.tankAnimator.SetTrigger(_cleaveTrigger);
        }
    }
}