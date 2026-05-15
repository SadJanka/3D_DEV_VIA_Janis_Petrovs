using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Iestatījumi")]
    public float openAngle = 90f;   // Cik plaši atvērt
    public float smoothing = 2f;    // Cik lēni atvērt

    private bool isOpen = false;
    private bool playerNearby = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Saglabā sākuma pozīciju
        closedRotation = transform.localRotation;
        // Izrēķina atvērto pozīciju
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;
    }

    void Update()
    {
        // Ja esi zonā UN nospied E
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // Vienmērīgi pagriež durvis
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothing);
    }

    // Šis nostrādā, kad tu ieej zaļajā Sphere Collider zonā
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Spēlētājs ir pie durvīm! Spied E.");
        }
    }

    // Šis nostrādā, kad tu izej no zonas
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}