using UnityEngine;

public class CharacterControllerTemp : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravitymultiplier = 10f;
    public float mouseSensitivity = 100f;
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private HealthManager _healthBarManagerCS;
    private float rotationX = 0f;
    private bool isCursorLocked = false;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _healthBarManagerCS = GetComponent<HealthManager>();

        if(_healthBarManagerCS != null) _healthBarManagerCS.OnDamageReceived += OnDamageReceived;
        if(_healthBarManagerCS != null) _healthBarManagerCS.OnHealthPointsEmpty += OnHealthPointsEmpty;
    }

    private void OnDamageReceived()
    {

    }
    private void OnHealthPointsEmpty()
    {

    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * vertical + transform.right * horizontal;
        movement = movement.normalized * moveSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + movement);

        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundMask);
        // Draw a red or green sphere at the ground check position to indicate whether the player is grounded or not
        Color debugColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(groundCheck.position, Vector3.down * 0.1f, debugColor);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if(!isGrounded && rb.velocity.y < 0)
        {
            rb.AddForce((Vector3.down * gravitymultiplier), ForceMode.Acceleration);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            isCursorLocked = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            isCursorLocked = false;
        }

        if (isCursorLocked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            rotationX += mouseX;

            transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
        }
    }
}
