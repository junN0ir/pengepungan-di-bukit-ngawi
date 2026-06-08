using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    public static EntityController Instance;

    [Header("Referensi")]
    public GameObject entityModel;
    public AIHunter aiHunter;
    public Transform player;

    [Header("Wander Points")]
    public Transform[] wanderPoints;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Speed Per Ritual")]
    public float wanderSpeed_Ritual1 = 2f;
    public float chaseSpeed_Ritual1 = 4f;
    public float wanderSpeed_Ritual2 = 2.5f;
    public float chaseSpeed_Ritual2 = 5f;
    public float wanderSpeed_Ritual3 = 3f;
    public float chaseSpeed_Ritual3 = 6f;
    public float wanderSpeed_Ritual4 = 3.5f;
    public float chaseSpeed_Ritual4 = 7f;

    [Header("Teleport Settings")]
    public float teleportCheckInterval_Ritual1 = 15f;
    public float teleportCheckInterval_Ritual2 = 14f;
    public float teleportCheckInterval_Ritual3 = 12f;
    public float teleportCheckInterval_Ritual4 = 10f;
    public float teleportJarakMin = 25f;
    public float teleportJarakMax = 50f;
    public float teleportTriggerJarak = 70f;

    [Header("Jumpscare")]
    public GameObject jumpscareVideoPanel;

    [Header("Ending")]
    public GameObject endingCutsceneObject;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip suaraTeriakan;
    public AudioClip suaraDetakJantung;

    public enum EntityMode { Inactive, Patrol, Chase, Ending }
    public EntityMode currentMode = EntityMode.Inactive;

    private NavMeshAgent agent;
    private Animator animator;
    private bool jumpscareTriggered = false;
    private int ritualCount = 0;
    private float currentWanderSpeed;
    private float currentChaseSpeed;
    private float currentTeleportInterval;
    private Coroutine teleportCoroutine;
    private Coroutine wanderCoroutine;

    // =====================
    // SETUP
    // =====================

    void Awake() { Instance = this; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (entityModel != null)
            animator = entityModel.GetComponentInChildren<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (entityModel != null) entityModel.SetActive(false);
        if (jumpscareVideoPanel != null) jumpscareVideoPanel.SetActive(false);
        if (endingCutsceneObject != null) endingCutsceneObject.SetActive(false);

        // AIHunter handle deteksi dan chase
        // Kita cukup pantau state-nya dari sini
        if (aiHunter != null) aiHunter.enabled = false;
    }

    void Update()
    {
        if (currentMode == EntityMode.Inactive) return;
        if (currentMode == EntityMode.Ending) return;

        SyncChaseState();
        UpdateAnimator();
    }

    // Pantau apakah AIHunter sedang chase atau tidak
    void SyncChaseState()
    {
        if (aiHunter == null) return;

        bool sedangChase = aiHunter.isChasing;

        if (sedangChase && currentMode == EntityMode.Patrol)
        {
            // Masuk chase
            currentMode = EntityMode.Chase;
            agent.speed = currentChaseSpeed;
            TriggerChaseSounds();
            Debug.Log("Masuk mode Chase");
        }
        else if (!sedangChase && currentMode == EntityMode.Chase)
        {
            // Keluar chase, kembali patrol
            currentMode = EntityMode.Patrol;
            agent.speed = currentWanderSpeed;
            StopChaseSounds();
            Debug.Log("Kembali ke Patrol");
        }
    }

    void UpdateAnimator()
    {
        if (animator == null || agent == null) return;
        if (!agent.isOnNavMesh) return;
        float speed = agent.velocity.magnitude;
        float moveValue = Mathf.Clamp01(speed / Mathf.Max(agent.speed, 0.1f));
        animator.SetFloat("Move", moveValue);
    }

    // =====================
    // RITUAL CALLBACKS
    // =====================

    public void OnRitual1Burned()
    {
        ritualCount = 1;
        SetSpeedByRitual();
        AktivasiEntitas();
        currentTeleportInterval = teleportCheckInterval_Ritual1;
        Debug.Log("Ritual 1 - Entitas aktif. Wander: " + currentWanderSpeed + " Chase: " + currentChaseSpeed);
    }

    public void OnRitual2Burned()
    {
        ritualCount = 2;
        SetSpeedByRitual();
        currentTeleportInterval = teleportCheckInterval_Ritual2;
        Debug.Log("Ritual 2 - Speed naik. Wander: " + currentWanderSpeed + " Chase: " + currentChaseSpeed);
    }

    public void OnRitual3Burned()
    {
        ritualCount = 3;
        SetSpeedByRitual();
        currentTeleportInterval = teleportCheckInterval_Ritual3;
        Debug.Log("Ritual 3 - Speed naik. Wander: " + currentWanderSpeed + " Chase: " + currentChaseSpeed);
    }

    public void OnRitual4Burned()
    {
        ritualCount = 4;
        SetSpeedByRitual();
        currentTeleportInterval = teleportCheckInterval_Ritual4;
        Debug.Log("Ritual 4 - Speed maksimal. Wander: " + currentWanderSpeed + " Chase: " + currentChaseSpeed);
    }

    public void OnRitual5Burned()
    {
        currentMode = EntityMode.Ending;
        StopSemuaCoroutine();

        if (entityModel != null) entityModel.SetActive(false);
        if (aiHunter != null) aiHunter.enabled = false;
        SetAgentStopped(true);
        StopChaseSounds();
        TriggerEnding();
    }

    void SetSpeedByRitual()
    {
        switch (ritualCount)
        {
            case 1:
                currentWanderSpeed = wanderSpeed_Ritual1;
                currentChaseSpeed = chaseSpeed_Ritual1;
                break;
            case 2:
                currentWanderSpeed = wanderSpeed_Ritual2;
                currentChaseSpeed = chaseSpeed_Ritual2;
                break;
            case 3:
                currentWanderSpeed = wanderSpeed_Ritual3;
                currentChaseSpeed = chaseSpeed_Ritual3;
                break;
            case 4:
                currentWanderSpeed = wanderSpeed_Ritual4;
                currentChaseSpeed = chaseSpeed_Ritual4;
                break;
        }

        // Update speed di agent dan AIHunter langsung
        if (agent != null && agent.isOnNavMesh)
            agent.speed = currentWanderSpeed;

        if (aiHunter != null)
        {
            aiHunter.wanderSpeed = currentWanderSpeed;
            aiHunter.chaseSpeed = currentChaseSpeed;
        }
    }

    void AktivasiEntitas()
    {
        // Spawn awal jauh dari player
        SpawnJauh();
        StartCoroutine(AktivasiDelay());
    }

    IEnumerator AktivasiDelay()
    {
        yield return null;
        yield return null;

        entityModel.SetActive(true);
        currentMode = EntityMode.Patrol;

        if (aiHunter != null) aiHunter.enabled = true;
        agent.speed = currentWanderSpeed;

        // Mulai wander dan teleport
        StopSemuaCoroutine();
        wanderCoroutine = StartCoroutine(WanderRoutine());
        teleportCoroutine = StartCoroutine(TeleportCheckRoutine());

        Debug.Log("Entitas aktif dan mulai patroli");
    }

    // =====================
    // WANDER
    // =====================

    IEnumerator WanderRoutine()
    {
        while (currentMode != EntityMode.Inactive && currentMode != EntityMode.Ending)
        {
            // Hanya wander kalau tidak sedang chase dan AIHunter tidak handle movement
            if (currentMode == EntityMode.Patrol && !aiHunter.isChasing)
            {
                if (wanderPoints.Length > 0 && agent.isOnNavMesh)
                {
                    // Pilih wander point random
                    Transform target = wanderPoints[Random.Range(0, wanderPoints.Length)];
                    agent.speed = currentWanderSpeed;
                    SetAgentDestination(target.position);

                    // Tunggu sampai sampai atau timeout 10 detik
                    float timeout = 0f;
                    while (agent.isOnNavMesh &&
                           (agent.pathPending || agent.remainingDistance > 1f) &&
                           timeout < 10f)
                    {
                        timeout += Time.deltaTime;
                        yield return null;
                    }

                    // Idle sebentar di waypoint
                    yield return new WaitForSeconds(Random.Range(1f, 3f));
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // =====================
    // TELEPORT CHECK
    // =====================

    IEnumerator TeleportCheckRoutine()
    {
        while (currentMode != EntityMode.Inactive && currentMode != EntityMode.Ending)
        {
            yield return new WaitForSeconds(currentTeleportInterval);

            // Jangan teleport kalau sedang chase
            if (currentMode == EntityMode.Chase || aiHunter.isChasing)
            {
                Debug.Log("Teleport skip - sedang chase");
                continue;
            }

            if (player == null) continue;

            float jarakKePlayer = Vector3.Distance(transform.position, player.position);

            if (jarakKePlayer > teleportTriggerJarak)
            {
                Debug.Log("Jarak terlalu jauh (" + jarakKePlayer.ToString("F0") + "m), teleport ke dekat player");
                Vector3 posisi = CariPosisiTeleport();

                if (posisi != Vector3.zero)
                {
                    entityModel.SetActive(false);
                    yield return new WaitForSeconds(0.2f);

                    agent.Warp(posisi);
                    yield return null;

                    // Hadap player
                    Vector3 arah = player.position - posisi;
                    arah.y = 0;
                    if (arah != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(arah);

                    entityModel.SetActive(true);
                    Debug.Log("Teleport ke: " + posisi);
                }
            }
            else
            {
                Debug.Log("Jarak OK (" + jarakKePlayer.ToString("F0") + "m), tidak perlu teleport");
            }
        }
    }

    Vector3 CariPosisiTeleport()
    {
        Camera cam = Camera.main;

        // Coba 15 kali cari posisi yang valid
        for (int i = 0; i < 15; i++)
        {
            Vector3 arahAcak = Random.onUnitSphere;
            arahAcak.y = 0;
            float jarak = Random.Range(teleportJarakMin, teleportJarakMax);
            Vector3 coba = player.position + arahAcak.normalized * jarak;

            NavMeshHit navHit;
            if (!NavMesh.SamplePosition(coba, out navHit, 5f, NavMesh.AllAreas))
                continue;

            // Cek tidak terlihat kamera
            Vector3 vp = cam.WorldToViewportPoint(navHit.position);
            bool terlihat = vp.x > 0.05f && vp.x < 0.95f
                         && vp.y > 0.05f && vp.y < 0.95f
                         && vp.z > 0;

            if (!terlihat)
                return navHit.position;
        }

        // Fallback ke spawn point yang tidak terlihat kamera
        Transform terpilih = null;
        float jarakTerbaik = 0f;

        foreach (Transform sp in spawnPoints)
        {
            if (sp == null) continue;

            float j = Vector3.Distance(sp.position, player.position);
            if (j < teleportJarakMin || j > teleportJarakMax) continue;

            Vector3 vp = cam.WorldToViewportPoint(sp.position);
            bool terlihat = vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1 && vp.z > 0;

            if (!terlihat && j > jarakTerbaik)
            {
                jarakTerbaik = j;
                terpilih = sp;
            }
        }

        if (terpilih != null) return terpilih.position;

        Debug.LogWarning("Tidak ada posisi teleport valid ditemukan");
        return Vector3.zero;
    }

    // =====================
    // SPAWN AWAL
    // =====================

    void SpawnJauh()
    {
        if (spawnPoints.Length == 0)
        {
            Vector3 arahAcak = Random.onUnitSphere;
            arahAcak.y = 0;
            float jarak = Random.Range(35f, 60f);
            Vector3 target = player.position + arahAcak.normalized * jarak;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 10f, NavMesh.AllAreas))
                agent.Warp(hit.position);
        }
        else
        {
            Transform terpilih = spawnPoints[0];
            float max = 0f;
            foreach (Transform sp in spawnPoints)
            {
                if (sp == null) continue;
                float j = Vector3.Distance(sp.position, player.position);
                if (j > max) { max = j; terpilih = sp; }
            }
            agent.Warp(terpilih.position);
        }
    }

    // =====================
    // HELPER
    // =====================

    void StopSemuaCoroutine()
    {
        if (teleportCoroutine != null) { StopCoroutine(teleportCoroutine); teleportCoroutine = null; }
        if (wanderCoroutine != null) { StopCoroutine(wanderCoroutine); wanderCoroutine = null; }
    }

    void SetAgentStopped(bool stopped)
    {
        if (agent == null || !agent.isOnNavMesh) return;
        agent.isStopped = stopped;
    }

    void SetAgentDestination(Vector3 dest)
    {
        if (agent == null || !agent.isOnNavMesh) return;
        agent.SetDestination(dest);
    }

    // =====================
    // AUDIO
    // =====================

    void TriggerChaseSounds()
    {
        if (audioSource == null) return;
        if (suaraTeriakan != null) audioSource.PlayOneShot(suaraTeriakan);
        if (suaraDetakJantung != null)
        {
            audioSource.clip = suaraDetakJantung;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopChaseSounds()
    {
        if (audioSource == null) return;
        audioSource.Stop();
        audioSource.loop = false;
    }

    // =====================
    // JUMPSCARE
    // =====================

    public void TriggerJumpscare()
    {
        if (jumpscareTriggered) return;
        jumpscareTriggered = true;

        if (jumpscareVideoPanel != null)
            jumpscareVideoPanel.SetActive(true);

        Time.timeScale = 0f;
        StartCoroutine(GameOverDelay());
    }

    IEnumerator GameOverDelay()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        jumpscareTriggered = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    // =====================
    // ENDING
    // =====================

    public void TriggerEnding()
    {
        if (endingCutsceneObject != null)
            endingCutsceneObject.SetActive(true);
        Debug.Log("Ending triggered!");
    }
}