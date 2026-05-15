using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool canCollect = false;
    private PlayerInventory playerInv;

    void Update()
    {
        // Ja spēlētājs ir zonā un nospiež E
        if (canCollect && Input.GetKeyDown(KeyCode.E))
        {
            if (playerInv != null)
            {
                playerInv.AddItem(); // Pieskaita punktu
                Destroy(gameObject); // Sfēra pazūd
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = true;
            playerInv = other.GetComponent<PlayerInventory>();
            Debug.Log("Press E to collect!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = false;
            playerInv = null;
        }
    }
}