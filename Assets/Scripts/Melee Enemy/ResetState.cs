using System.Collections;
using UnityEngine;

namespace MeleeEnemy
{
    public class ResetState : MeleeEnemyState
    {
        private MeleeEnemyStateManager _manager;

        public ResetState(MeleeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            Debug.Log(_manager.gameObject.name + " is now resetting");

            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.resetMat;

            _manager.navMeshAgentManagerCS.ChangeDestination(_manager.resetTransform.position);
        }

        public override void Execute()
        {
            Debug.Log(Vector3.Distance(_manager.transform.position, _manager.resetTransform.position));
            if(Vector3.Distance(_manager.transform.position, _manager.resetTransform.position) < 2f) _manager.TransitionToState(_manager.idleState);
        }

        public override void Exit()
        {
            
        }
    }
}