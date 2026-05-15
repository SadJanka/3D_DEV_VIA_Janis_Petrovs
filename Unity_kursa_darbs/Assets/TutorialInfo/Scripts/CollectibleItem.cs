using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool canCollect = false;
    private PlayerMovement playerScript;

    void Update()
    {
        // Ja spēlētājs ir zonā UN nospiež E
        if (canCollect && Input.GetKeyDown(KeyCode.E))
        {
            if (playerScript != null)
            {
                playerScript.AddItem(); // Izsauc AddItem no tava PlayerMovement
                Destroy(gameObject);    // Sfēra pazūd
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Pārbauda vai tas, kas iegāja zonā, ir spēlētājs
        if (other.CompareTag("Player"))
        {
            canCollect = true;
            playerScript = other.GetComponent<PlayerMovement>();
            Debug.Log("Press E to collect item!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = false;
            playerScript = null;
        }
    }
}