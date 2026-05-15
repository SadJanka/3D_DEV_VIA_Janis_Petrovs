using UnityEngine;
using TMPro; // We use TextMeshPro for the UI

public class PlayerInventory : MonoBehaviour
{
    public int collectedCount = 0;
    public int targetAmount = 5;
    public GameObject winTextObject; // Drag your UI Text here

    void Start()
    {
        // Hide the "You Win" text at the start
        if (winTextObject != null)
            winTextObject.SetActive(false);
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
            winTextObject.SetActive(true); // Show the "You Win" text
        }

        // Optional: Unlock the mouse so you can close the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("YOU WIN!");
    }
}