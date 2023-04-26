using UnityEngine;
using Player;

namespace Enemy.Range
{
    public class RangeEnemyProjectile : MonoBehaviour
    {
        public Transform targetTransform;
        public float damage;
        public float speed = 25f;
        private bool isMoving = true;

        private void Update()
        {
            if(targetTransform == null) return;
            if (isMoving)
            {
                float distance = Vector3.Distance(transform.position, targetTransform.transform.position);
                float step = speed * Time.deltaTime;

                if (distance <= step)
                {
                    transform.position = targetTransform.transform.position;
                    isMoving = false;
                    Destroy(gameObject);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetTransform.transform.position, step);
                }
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.transform == targetTransform)
            {
                var targetDamageReceiver = other.GetComponent<PlayerDamageReceiver>();
                targetDamageReceiver.ReceiveDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
