using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HealerEnemy
{
    public class TeleportLocationFinder : MonoBehaviour
    {
        public float radius = 5f;
        public float minRadius = 5f;
        public float maxGroundDistance = 5f;
        private LayerMask groundLayer;

        void Start()
        {
            groundLayer = LayersEnum.instance.GroundLayer;
        }

        public Vector3 GetRandomPosition(Transform centerTransform)
        {
            Vector3 center = centerTransform.position;
            float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            float distance = UnityEngine.Random.Range(minRadius, radius);
            float x = distance * Mathf.Cos(angle) + center.x;
            float y = center.y;
            float z = distance * Mathf.Sin(angle) + center.z;
            Vector3 randomPosition = new Vector3(x, y, z);
            if (HasGround(randomPosition))
            {
                return randomPosition;
            }
            else
            {
                // if there's no ground under the random position, try again
                return GetRandomPosition(centerTransform);
            }
        }

        private bool HasGround(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, maxGroundDistance, groundLayer))
            {
                // if the raycast hit something on the ground layer, there's ground under the position
                return true;
            }
            else
            {
                // if the raycast didn't hit anything on the ground layer, there's no ground under the position
                return false;
            }
        }
    }
}
