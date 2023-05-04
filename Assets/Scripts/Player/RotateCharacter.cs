using UnityEngine;
using Cinemachine;

public class RotateCharacter : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float turnSpeed = 10f;
    public float deadZone = 0.1f;

    void Update()
    {
        // Check if both mouse buttons are pressed
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            // Get the rotation of the camera around the y-axis
            float cameraYRotation = freeLookCamera.transform.rotation.eulerAngles.y;

            // Create a new Quaternion that represents the rotation of the character around the y-axis
            Quaternion targetRotation = Quaternion.Euler(0f, cameraYRotation, 0f);

            // Set the rotation of the character to the target rotation
            transform.rotation = targetRotation;
        }
        // Check if any movement key is pressed
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            // Get the rotation of the camera's current lookat target
            Quaternion cameraRotation = Quaternion.LookRotation(freeLookCamera.LookAt.position - freeLookCamera.transform.position, Vector3.up);

            // Calculate the angle between the x-axis and the camera's forward direction
            float angle = Mathf.Atan2(cameraRotation.eulerAngles.x, cameraRotation.eulerAngles.z) * Mathf.Rad2Deg;

            // Create a new Quaternion that represents the rotation of the character around the y-axis
            Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);

            // Set the rotation of the character to the target rotation
            transform.rotation = targetRotation;
        }
    }
}
