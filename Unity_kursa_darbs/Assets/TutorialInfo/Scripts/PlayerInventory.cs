using UnityEngine;
using TMPro; // Nepieciešams UI tekstam

public class PlayerInventory : MonoBehaviour
{
    public int collectedCount = 0;
    public int targetAmount = 5;
    public GameObject winTextObject; // Šeit Inspector ieliksi savu UI tekstu

    void Start()
    {
        if (winTextObject != null)
            winTextObject.SetActive(false); // Sākumā paslēpj uzvaras tekstu
    }

    public void AddItem()
    {
        collectedCount++;
        Debug.Log("Items collected: " + collectedCount + " / " + targetAmount);

        if (collectedCount >= targetAmount)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        if (winTextObject != null)
        {
            winTextObject.SetActive(true); // Parāda "You Win"
        }

        // Atbloķē peli, lai varētu iziet no spēles
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("YOU WIN!");
    }
}