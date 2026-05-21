using UnityEngine;
using UnityEngine.AI; // Nepieciešams, lai strādātu ar NavMesh (botiem)

public class MonsterAI : MonoBehaviour
{
    [Header("Monster Settings")]
    public float walkRadius = 30f;     // Cik tālu no savas pozīcijas viņš var maldīties
    public float roarInterval = 8f;    // Ik pēc cik sekundēm viņš rēks

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private float roarTimer;

    void Start()
    {
        // Automātiski paņem komponentes, kas uzliktas uz monstra
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        // Uzstāda sākuma laiku pirmajam rēcienam
        roarTimer = roarInterval;

        // Pārbaudām, vai nav aizmirsts uzlikt NavMesh Agent
        if (agent == null)
        {
            Debug.LogError("Uz monstra trūkst 'NavMesh Agent' komponentes! Pievieno to caur Add Component.");
        }

        // Liekam monstram uzreiz sākt iet uz pirmo nejaušo punktu
        GoToRandomPoint();
    }

    void Update()
    {
        // Ja nav NavMesh, neko tālāk nedarām, lai nemestu kļūdas
        if (agent == null || !agent.isOnNavMesh) return;

        // 1. KUSTĪBA: Ja monstrs ir ticis līdz galam vai gandrīz apstājies, meklējam jaunu mērķi
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToRandomPoint();
        }

        // 2. SKAŅA: Skaita laiku uz leju. Kad sasniedz 0, monstrs norūcas
        roarTimer -= Time.deltaTime;
        if (roarTimer <= 0f)
        {
            Roar();
            roarTimer = roarInterval; // Atiestata taimeri uz nākamajām 8 sekundēm
        }
    }

    void GoToRandomPoint()
    {
        // Atrod nejaušu punktu sfērā ap monstru
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Pārbauda, vai šis nejaušais punkts atrodas uz zilās "izceptās" NavMesh zonas
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position); // Pasaka monstram, kurp doties
        }
    }

    void Roar()
    {
        // Pārbauda, vai Audio Source ir un vai tajā ir ielikts MP3 fails
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("Monstrs norūcās!");
        }
        else
        {
            Debug.LogWarning("Monstrs gribēja rēkt, bet Audio Source nav ielikta skaņa!");
        }
    }
}