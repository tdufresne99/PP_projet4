using System.Collections;
using UnityEngine;

namespace RangeEnemy
{
    public class MeteorAbilityState : RangeEnemyState
    {
        private GameObject _meteorOverlayGO;
        private GameObject _meteorGO;
        private RangeEnemyStateManager _manager;
        private Vector3 _meteorHitLocation;
        private Coroutine _meteorCoroutine;

        public MeteorAbilityState(RangeEnemyStateManager manager)
        {
            this._manager = manager;
        }

        public override void Enter()
        {
            // ---- Set state animations ------------------------------
            _manager.meshRenderer.material = _manager.idleMat;

            _meteorCoroutine = _manager.StartCoroutine(CoroutineMeteorCast());
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }

        private IEnumerator CoroutineMeteorCast()
        {
            _meteorOverlayGO = _manager.InstantiateMeteorOverlay();
            var meteorHitLocation = _meteorOverlayGO.transform.position;

            yield return new WaitForSecondsRealtime(2f);

            GameObject.Destroy(_meteorOverlayGO);
            _manager.InstantiateMeteor(meteorHitLocation);
            _manager.TransitionToState(_manager.chaseState);
        }
    }
}