using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Player
{
    public class IceShieldDamageReductionDebuff : MonoBehaviour
    {
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
        }

        void Start()
        {
            _coroutineIceShieldDamageReductionDebuff = StartCoroutine(CoroutineIceShieldDamageReductionDebuff());
        }

        public void GetDebuffValues(float duration, float damageReduction)
        {
            _duration = duration;
            _damageReduction = damageReduction;
        }

        private IEnumerator CoroutineIceShieldDamageReductionDebuff()
        {
            _enemyDamageDealerCS.damageMultiplier *= _damageReduction;
            yield return new WaitForSecondsRealtime(_duration);
            _enemyDamageDealerCS.damageMultiplier /= _damageReduction;
            Destroy(this);
        }
    }
}