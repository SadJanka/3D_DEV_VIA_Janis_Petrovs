using UnityEngine;

public class JumpscareTrigger : MonoBehaviour
{
    public MonsterAI monsterScript; // Šeit Inspectorā ievelc Monstru

    private bool hasTriggered = false; // Lai tas notiktu tikai VIENU reizi spēlē

    private void OnTriggerEnter(Collider other)
    {
        // Pārbauda, vai zonā iegāja tieši Player un vai tas jau nav noticis
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (monsterScript != null)
            {
                monsterScript.TriggerJumpscare(); // Pasaka monstram, lai izlec
                hasTriggered = true; // Atzīmē, ka triks ir izpildīts
            }
        }
    }
}