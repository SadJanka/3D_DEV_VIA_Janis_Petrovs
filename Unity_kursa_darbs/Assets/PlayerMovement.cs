using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 5f;      // Parastais ātrums
    public float sprintSpeed = 10f;    // Sprinta ātrums
    public float gravity = -9.81f;
    public float sensitivity = 2f;

    Vector3 velocity;
    float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Skatīšanās apkārt
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        // 2. Noteikt pašreizējo ātrumu (Sprints vai Staigāšana)
        float currentSpeed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

        // 3. Kustība
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 4. Gravitācija
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            { // Ja nospiež E
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Šauj staru no ekrāna centra
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10f))
                { // Ja 3 metru attālumā kaut ko aizskar
                    if (hit.collider.GetComponent<DoorOpener>() != null)
                    {
                        hit.collider.GetComponent<DoorOpener>().ToggleDoor();
                    }
                }
            }
        }
    }
}