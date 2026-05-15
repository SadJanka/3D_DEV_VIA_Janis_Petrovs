using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public float openAngle = 90f; // Cik grādos durvis atvērsies
    public float smoothing = 2f;  // Cik lēni vērsies
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;
    }

    void Update()
    {
        if (isOpen)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, openRotation, Time.deltaTime * smoothing);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRotation, Time.deltaTime * smoothing);
        }
    }

    // Šo funkciju izsauksim no spēlētāja
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}