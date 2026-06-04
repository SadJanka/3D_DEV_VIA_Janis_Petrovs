using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Iestatījumi")]
    public float openAngle = 90f;
    public float smoothing = 2f;

    [Header("UI Ikonas (World Space)")]
    public GameObject eIcon;        // Ieliec šeit E_Icon objektu parasto durvju Canvas

    private bool isOpen = false;
    private bool playerNearby = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;

        if (eIcon != null) eIcon.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothing);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (eIcon != null) eIcon.SetActive(true); // Parādām E burtu
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (eIcon != null) eIcon.SetActive(false); // Paslēpjam E burtu
        }
    }
}