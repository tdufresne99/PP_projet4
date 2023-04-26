using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Enemy.Range
{
    public class Meteor : MonoBehaviour
    {
        public Vector3 targetPosition;
        public float damage;
        public float speed = 25f;
        private bool isMoving = true;

        private void Update()
        {
            if (targetPosition == null) return;
            if (isMoving)
            {
                float distance = Vector3.Distance(transform.position, targetPosition);
                float step = speed * Time.deltaTime;

                if (distance <= step)
                {
                    transform.position = targetPosition;
                    isMoving = false;
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
                targetDamageReceiver.ReceiveDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
