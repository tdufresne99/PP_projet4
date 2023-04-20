using System.Collections;
using UnityEngine;

namespace TankEnemy
{
    public class CleaveAbilityState : TankEnemyState
    {
        private TankEnemyStateManager _manager;

        public CleaveAbilityState(TankEnemyStateManager manager)
        {
            this._manager = manager;
        }  

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.cleaveAbilityMat;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}