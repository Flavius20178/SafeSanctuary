using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float lookSpeed = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S
        float moveY = 0f;

        if (Input.GetKey(KeyCode.E)) moveY += 1f;  // Up
        if (Input.GetKey(KeyCode.Q)) moveY -= 1f;  // Down

        Vector3 move = transform.right * moveX + transform.forward * moveZ + transform.up * moveY;
        transform.position += move * movementSpeed * Time.deltaTime;
    }

    void HandleLook()
    {
        if (Input.GetMouseButton(1)) // Right Mouse Button
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            rotationX -= mouseY;
            rotationY += mouseX;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }
}
