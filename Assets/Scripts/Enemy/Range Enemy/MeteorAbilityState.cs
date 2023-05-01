using System.Collections;
using UnityEngine;

namespace Enemy.Range
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
            _manager.enemyAnimator.SetTrigger("meteor");

            _manager.abilityLocked = true;

            _meteorCoroutine = _manager.StartCoroutine(CoroutineMeteorCast());
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            _manager.meteorOnCooldown = true;
            _manager.abilityLocked = false;
        }

        private IEnumerator CoroutineMeteorCast()
        {
            _meteorOverlayGO = InstantiateMeteorOverlay();
            var meteorHitLocation = _meteorOverlayGO.transform.position;

            yield return new WaitForSecondsRealtime(2f);

            GameObject.Destroy(_meteorOverlayGO);
            InstantiateMeteor(meteorHitLocation);
            _manager.TransitionToState(_manager.chaseState);
        }

        public GameObject InstantiateMeteorOverlay()
        {
            var instanciatedMeteorOverlay = GameObject.Instantiate(_manager.meteorOverlayGO, new Vector3(_manager.targetTransform.position.x, 0.22f, _manager.targetTransform.position.z), Quaternion.identity);
            return instanciatedMeteorOverlay;
        }

        public GameObject InstantiateMeteor(Vector3 meteorHitLocation)
        {
            var instanciatedMeteor = GameObject.Instantiate(_manager.meteorGO, new Vector3(meteorHitLocation.x + Random.Range(-5f, 5f), 5f, meteorHitLocation.z + Random.Range(-5f, 5f)), Quaternion.identity);

            Meteor meteorCS = instanciatedMeteor.GetComponent<Meteor>();

            if (meteorCS != null)
            {
                meteorCS.targetPosition = meteorHitLocation;
                meteorCS.rangeEnemyStateManagerCS = _manager;
            }
            return instanciatedMeteor;
        }

        
    }
}