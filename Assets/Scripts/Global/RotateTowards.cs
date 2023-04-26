using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    private GameObject target; // The target GameObject to rotate towards
    [SerializeField] private float rotationSpeed = 10f; // The speed of rotation
    [SerializeField] private float yAngleOffset = 90f; // The angle offset in the Y axis

    void Start()
    {
        target = Camera.main.gameObject;
    }

    void Update()
    {
        // Calculate the direction from this GameObject to the target GameObject
        Vector3 direction = target.transform.position - transform.position;

        // Calculate the angle in degrees between this GameObject and the target GameObject in the Y axis
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // Add the angle offset to the calculated angle
        angle += yAngleOffset;

        // Create a rotation in the Y axis based on the new calculated angle and rotation speed
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        // Rotate this GameObject towards the target GameObject with the angle offset
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}


