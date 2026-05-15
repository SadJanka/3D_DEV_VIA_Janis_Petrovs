using UnityEngine;
using TMPro; // ????yes or no?

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;
    public float sensitivity = 2f;

    [Header("Scavenger Hunt Settings")]
    public int collectedCount = 0;
    public int targetAmount = 5;
    public GameObject winTextObject; // Šeit Inspector ieliec savu "You Win" tekstu

    Vector3 velocity;
    float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();

        // Paslēpjam uzvaras tekstu spēles sākumā
        if (winTextObject != null)
            winTextObject.SetActive(false);
    }

    void Update()
    {
        // 1. SKATĪŠANĀS
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        // 2. KUSTĪBA
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 3. GRAVITĀCIJA
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void AddItem()
    {
        collectedCount++;
        Debug.Log("Items: " + collectedCount + " / " + targetAmount);

        if (collectedCount >= targetAmount)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        if (winTextObject != null)
        {
            winTextObject.SetActive(true); // Parāda "YOU WIN" uz ekrāna
        }

        // Atbloķē peli, lai varētu iziet
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("All items collected! YOU WIN!");
    }
}