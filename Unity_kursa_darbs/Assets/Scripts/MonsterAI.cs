using UnityEngine;
using UnityEngine.AI; // Nepieciešams, lai strādātu ar NavMesh (botiem)

public class MonsterAI : MonoBehaviour
{
    // Monstra stāvokļi (Sākumā viņš guļ zem zemes un gaida rituāla apli)
    public enum MonsterState { Sleeping, Jumpscare, Chasing, Wandering }
    private MonsterState currentState = MonsterState.Sleeping;

    [Header("Base Settings")]
    public float walkRadius = 30f;     // Cik tālu viņš var maldīties parastajā režīmā
    public float roarInterval = 8f;    // Ik pēc cik sekundēm viņš rēks

    [Header("Chase Settings")]
    public Transform player;           // Ievelc šeit savu Player objektu Inspectorā
    public float chaseSpeed = 6.5f;    // Ātrums, kad dzenas pakaļ
    public float normalSpeed = 3.5f;   // Parastais staigāšanas ātrums
    public float chaseDuration = 15f;  // Cik sekundes skries pakaļ pirms padodas

    [Header("New Roar Settings")]
    public float warningDistance = 5f;       // Attālums, kurā monstrs draudīgi norūksies
    public float warningRoarCooldown = 4f;   // Cik sekundes nogaidīs pirms nākamā brīdinājuma rēciena
    private float warningRoarTimer = 0f;

    [Header("Jumpscare Settings")]
    public Transform jumpscareSpawnPoint; // Punkts pie apļa, kur monstram jāuzrodas
    private Vector3 targetSurfacePosition;
    private bool isRising = false;

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private Animator animator;
    private float roarTimer;
    private float chaseTimer;
    private bool isRoaring = false;

    void Start()
    {
        // Automātiski paņem komponentes, kas uzliktas uz monstra
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = normalSpeed;
        }

        roarTimer = roarInterval; // Uzstāda sākuma laiku pirmajam rēcienam

        if (agent == null) Debug.LogError("Uz monstra trūkst 'NavMesh Agent' komponentes!");
        if (animator == null) Debug.LogError("Uz monstra trūkst 'Animator' komponentes!");

        // GUDRAIS LABOJUMS SĀKUMAM:
        if (jumpscareSpawnPoint != null && agent != null)
        {
            // Atrodam tuvāko punktu uz NavMesh, lai neizmestu kļūdas par "Warp" gaisā
            NavMeshHit hit;
            if (NavMesh.SamplePosition(jumpscareSpawnPoint.position, out hit, 5f, NavMesh.AllAreas))
            {
                targetSurfacePosition = hit.position;
                Vector3 underGroundPos = targetSurfacePosition - new Vector3(0, 5f, 0); // Paslēpjam 5 metrus zem zemes

                agent.Warp(underGroundPos);
                agent.enabled = false; // Izslēdzam aģentu uz laiku, kamēr viņš guļ zem zemes, lai Unity nebļauj
            }
            else
            {
                Debug.LogError("Jumpscare Spawn Point ir pārāk tālu no NavMesh virsmas! Pabīdi to tuvāk zemei scēnā.");
            }
        }
        else
        {
            Debug.LogWarning("Nav ielikts Jumpscare Spawn Point Inspectorā!");
        }
    }

    void Update()
    {
        // Ja nav NavMesh komponentes, neko tālāk nedarām, lai nemestu kļūdas
        if (agent == null || (agent.enabled && !agent.isOnNavMesh)) return;

        // KUSTĪBAS ANIMĀCIJAS KONTROLE
        if (!isRoaring && currentState != MonsterState.Sleeping && agent.enabled)
        {
            // Pārbaudām, vai monstrs reāli šobrīd pārvietojas pa zemi
            bool isMoving = agent.velocity.magnitude > 0.1f || (agent.hasPath && agent.remainingDistance > 0.1f);
            float animationSpeed = isMoving ? agent.speed : 0f;

            animator.SetFloat("Speed", animationSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f); // Ja guļ, lec vai rēc - kustības animācija apstājas
        }

        // Lēnām velkam monstru augšā virszemē jumpscare laikā
        if (isRising)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetSurfacePosition, Time.deltaTime * 6f);
            if (Vector3.Distance(transform.position, targetSurfacePosition) < 0.1f)
            {
                isRising = false;
            }
        }

        // Samazinām brīdinājuma rēciena pauzes taimeri
        if (warningRoarTimer > 0f) warningRoarTimer -= Time.deltaTime;

        // Atkarībā no pašreizējā stāvokļa, izpildām attiecīgo loģiku
        switch (currentState)
        {
            case MonsterState.Sleeping:
                // Kamēr guļ, viņš vienkārši nekustas
                break;

            case MonsterState.Jumpscare:
                // Izlekšanas laikā aģents vēl nedrīkst patstāvīgi skriet
                if (agent.enabled) agent.isStopped = true;
                break;

            case MonsterState.Chasing:
                HandleChasing();
                break;

            case MonsterState.Wandering:
                HandleWandering();
                break;
        }
    }

    void HandleWandering()
    {
        if (isRoaring)
        {
            if (agent.enabled) agent.isStopped = true;
            return;
        }

        if (agent.enabled) agent.isStopped = false;

        // Ja monstrs ir ticis līdz galam, meklējam jaunu nejaušu mērķi
        if (!agent.pathPending && agent.remainingDistance < 0.8f)
        {
            GoToRandomPoint();
        }

        // Parastais rēciens ik pēc noteiktā intervāla
        roarTimer -= Time.deltaTime;
        if (roarTimer <= 0f)
        {
            Roar(2.5f);
            roarTimer = roarInterval; // Atiestata taimeri
        }
    }

    void HandleChasing()
    {
        if (player == null) return;

        if (!isRoaring)
        {
            if (agent.enabled)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }

            // UZLABOJUMS nr. 2: Pārbaudām attālumu līdz spēlētājam brīdinājuma rēcienam
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= warningDistance && warningRoarTimer <= 0f)
            {
                Roar(1.5f); // Īsais brīdinājuma rēciens (1.5 sekundes)
                warningRoarTimer = warningRoarCooldown; // Uzliekam pauzi, lai nebļauj bez apstājas
            }
        }
        else
        {
            if (agent.enabled) agent.isStopped = true; // Rēkšanas laikā monstrs uz brīdi apstājas
        }

        chaseTimer -= Time.deltaTime;
        if (chaseTimer <= 0f)
        {
            StopChasing();
        }
    }

    // Šo funkciju izsauc rituāla apļa triggeris (JumpscareTrigger.cs)
    public void TriggerJumpscare()
    {
        if (currentState != MonsterState.Sleeping) return;

        currentState = MonsterState.Jumpscare;

        if (jumpscareSpawnPoint != null && agent != null)
        {
            NavMeshHit hit;
            // Drošības nolūkos vēlreiz pārliecināmies par tuvāko NavMesh punktu
            if (NavMesh.SamplePosition(jumpscareSpawnPoint.position, out hit, 5f, NavMesh.AllAreas))
            {
                targetSurfacePosition = hit.position;
                Vector3 underGroundPos = targetSurfacePosition - new Vector3(0, 4f, 0);

                agent.enabled = true; // Ieslēdzam atpakaļ NavMeshAgent komponenti
                agent.Warp(underGroundPos); // Teleportējam monstru zem zemes
                isRising = true;
            }
        }

        if (animator != null)
        {
            animator.SetTrigger("JumpIn"); // Palaižam izlekšanas animāciju
        }

        // UZLABOJUMS nr. 1: Kad izlekšana pabeigta (pēc 1.8 sekundēm), uzreiz seko pirmais rēciens
        Invoke("PostJumpRoar", 1.8f);
    }

    void PostJumpRoar()
    {
        isRising = false;

        // Drošības pārbaude – rēcam tikai tad, ja aģents ir veiksmīgi piesaistīts NavMesh
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            Roar(2.5f); // Skaļais rēciens uz 2.5 sekundēm tieši pēc izlīšanas
        }
        else
        {
            isRoaring = false;
        }

        // Tiklīdz šis sākuma rēciens beidzas (pēc 2.5 sekundēm), sākas pakaļdzīšanās
        Invoke("StartChasing", 2.5f);
    }

    void StartChasing()
    {
        currentState = MonsterState.Chasing;
        isRoaring = false;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        }

        chaseTimer = chaseDuration;
    }

    void StopChasing()
    {
        currentState = MonsterState.Wandering; // Padodas un pāriet uz parasto staigāšanu
        if (agent != null && agent.enabled)
        {
            agent.speed = normalSpeed;
            agent.isStopped = false;
        }
        roarTimer = roarInterval;
        GoToRandomPoint(); // Uzreiz dodas uz jaunu nejaušu punktu
    }

    void GoToRandomPoint()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        // Atrod nejaušu punktu sfērā ap monstru
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Pārbauda, vai šis nejaušais punkts atrodas uz zilās NavMesh zonas
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position); // Pasaka monstram, kurp doties
        }
    }

    // Universāla rēkšanas funkcija ar noteiktu ilgumu sekundēs
    void Roar(float duration)
    {
        isRoaring = true;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.SetTrigger("Roar");
        }

        // Atskaņo skaņas failu, ja tāds ir ielikts
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // Pēc norādītā laika izsaucam EndRoar funkciju, lai atbloķētu monstru
        Invoke("EndRoar", duration);
    }

    void EndRoar()
    {
        isRoaring = false;
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        if (currentState == MonsterState.Wandering)
        {
            GoToRandomPoint();
        }
    }
}