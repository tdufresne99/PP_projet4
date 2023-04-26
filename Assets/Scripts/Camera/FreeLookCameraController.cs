using UnityEngine;
using Cinemachine;

public class FreeLookCameraController : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;
    [SerializeField] private float rotateSpeed = 1f;
    private bool rightMouseButtonDown;

    void Awake()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        if(freeLookCamera == null) Debug.Log("no free look camera");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rightMouseButtonDown = true;
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeLookCamera.m_YAxisRecentering.m_enabled = false;
            freeLookCamera.m_XAxis.m_InputAxisName = "Mouse X";
            freeLookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            rightMouseButtonDown = false;
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = true;
            freeLookCamera.m_YAxisRecentering.m_enabled = true;
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
            Cursor.lockState = CursorLockMode.None;
        }
    }
}