using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class ShieldAbilityState : TankEnemyState
    {
        private TankEnemyStateManager _manager;

        public ShieldAbilityState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            Debug.Log(_manager.gameObject.name + " is now using Shield ability");
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.shieldAbilityMat;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}