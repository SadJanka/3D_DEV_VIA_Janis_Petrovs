using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool canCollect = false;
    private PlayerInventory playerInventory;

    private void Update()
    {
        // If player is in zone and presses E
        if (canCollect && Input.GetKeyDown(KeyCode.E))
        {
            Collect();
        }
    }

    void Collect()
    {
        if (playerInventory != null)
        {
            playerInventory.AddItem();
            Destroy(gameObject); // Item disappears
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            Debug.Log("Press E to collect!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = false;
            playerInventory = null;
        }
    }
}