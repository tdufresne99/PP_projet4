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

            _manager.abilityLocked = true;

            _manager.transform.LookAt(_manager.targetTransform, Vector3.up);

            PerformCleaveAttack();
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            _manager.abilityLocked = false;
            _manager.cleaveOnCooldown = true;
        }

        private void PerformCleaveAttack()
        {
            _manager.tankAnimator.SetTrigger(_cleaveTrigger);
        }
    }
}