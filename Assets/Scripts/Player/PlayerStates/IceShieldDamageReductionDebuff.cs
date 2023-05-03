using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using TMPro;

namespace Player
{
    public class IceShieldDamageReductionDebuff : MonoBehaviour
    {
        private GameObject _iceShieldDebuffIconGO;
        private GameObject _instanciatedIceShieldDebuffIconGO;
        private HealthManager _enemyHealthManagerCS;
        private float _duration = 10f;
        private float _damageReduction = 0.7f;
        private Coroutine _coroutineIceShieldDamageReductionDebuff;
        private EnemyDamageDealer _enemyDamageDealerCS;
        
        void Awake()
        {
            _enemyDamageDealerCS = GetComponent<EnemyDamageDealer>();
            if(_enemyDamageDealerCS == null) 
            {
                Debug.LogWarning("No EnemyDamageDealer found on " + gameObject.name + " the debuff will be destroyed (IceShieldDamageReductionDebuff.cs)");
                Destroy(this);
            }

            _enemyHealthManagerCS = GetComponent<HealthManager>();
            if (_enemyHealthManagerCS == null)
            {
                Debug.LogWarning(gameObject.name + " does not have the 'HealthManager' component. (SpreadFireDebuff.cs)");
                Destroy(this);
            }
        }

        void Start()
        {
            _coroutineIceShieldDamageReductionDebuff = StartCoroutine(CoroutineIceShieldDamageReductionDebuff());
        }

        public void GetDebuffValues(float duration, float damageReduction, GameObject debuffIcon)
        {
            _duration = duration;
            _damageReduction = damageReduction;
            _iceShieldDebuffIconGO = debuffIcon;
        }

        private IEnumerator CoroutineIceShieldDamageReductionDebuff()
        {
            _instanciatedIceShieldDebuffIconGO = Instantiate(_iceShieldDebuffIconGO, _enemyHealthManagerCS.iconHolder.transform.position, _enemyHealthManagerCS.iconHolder.transform.rotation, _enemyHealthManagerCS.iconHolder.transform);

            var iconText = _instanciatedIceShieldDebuffIconGO.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            
            _enemyDamageDealerCS.damageMultiplier *= _damageReduction;

            var timeRemaining = _duration;
            while (timeRemaining > 0)
            {
                iconText.text = timeRemaining + "";
                yield return new WaitForSecondsRealtime(1f);
                timeRemaining--;
            }
            _enemyDamageDealerCS.damageMultiplier /= _damageReduction;
            Destroy(this);
        }

        void OnDestroy()
        {
            if(_instanciatedIceShieldDebuffIconGO != null) Destroy(_instanciatedIceShieldDebuffIconGO);
        }
    }
}