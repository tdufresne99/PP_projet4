using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Enemy.Range
{
    public class Meteor : MonoBehaviour
    {
        public RangeEnemyStateManager rangeEnemyStateManagerCS;
        public Vector3 targetPosition;
        private float _speed = 25f;
        private float _damage = 150f;
        private bool _isMoving = true;

        void Start()
        {
            if(rangeEnemyStateManagerCS != null)
            {
                _speed = rangeEnemyStateManagerCS.meteorSpeed;
                _damage = rangeEnemyStateManagerCS.meteorDamage;
            }
            else Debug.LogWarning("No RangeEnemyStateManager assigned, default values will be used (Meteor.cs)");
        }

        private void Update()
        {
            if (targetPosition == null) return;
            if (_isMoving)
            {
                float distance = Vector3.Distance(transform.position, targetPosition);
                float step = _speed * Time.deltaTime;

                if (distance <= step)
                {
                    transform.position = targetPosition;
                    _isMoving = false;
                    Destroy(gameObject);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                }
            }
        }
        
        void OnTriggerEnter(Collider other)
        {
            var targetDamageReceiver = other.GetComponent<PlayerDamageReceiver>();
            if (targetDamageReceiver != null)
            {
                targetDamageReceiver.ReceiveDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
