using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("UI Ikonas")]
    public GameObject eIcon; // Ieliec šeit savu E_Icon objektu no atslēgas Canvas

    private bool canCollect = false;
    private PlayerMovement playerScript;

    void Start()
    {
        if (eIcon != null) eIcon.SetActive(false);
    }

    void Update()
    {
        if (canCollect && Input.GetKeyDown(KeyCode.E))
        {
            // Ja skripts pazaudēts, mēģinām atrast caur Tagu
            if (playerScript == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) playerScript = playerObj.GetComponent<PlayerMovement>();
            }

            if (playerScript != null)
            {
                playerScript.AddItem(); // Pieskaitām atslēgu spēlētājam

                if (eIcon != null) eIcon.SetActive(false);
                Destroy(gameObject); // Izdzēšam atslēgu no pasaules
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = true;
            playerScript = other.GetComponent<PlayerMovement>();
            if (playerScript == null) playerScript = other.GetComponentInChildren<PlayerMovement>();
            if (playerScript == null) playerScript = other.GetComponentInParent<PlayerMovement>();

            if (eIcon != null) eIcon.SetActive(true); // Parādām E burtu gaisā pie atslēgas
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = false;
            playerScript = null;
            if (eIcon != null) eIcon.SetActive(false);
        }
    }
}