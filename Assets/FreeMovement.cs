using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float lookSpeed = 2f;

    private float rotationX;
    private float rotationY;

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        var moveX = Input.GetAxis("Horizontal"); // A/D
        var moveZ = Input.GetAxis("Vertical"); // W/S
        var moveY = 0f;

        if (Input.GetKey(KeyCode.E)) moveY += 1f; // Up
        if (Input.GetKey(KeyCode.Q)) moveY -= 1f; // Down

        var move = transform.right * moveX + transform.forward * moveZ + transform.up * moveY;
        transform.position += move * movementSpeed * Time.deltaTime;
    }

    private void HandleLook()
    {
        if (Input.GetMouseButton(1)) // Right Mouse Button
        {
            var mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            var mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            rotationX -= mouseY;
            rotationY += mouseX;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }
}