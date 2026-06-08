using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SatpamAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float wanderSpeed = 2f;
    public float idleWaitTime = 2f;

    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 15f;
    public float detectionAngle = 60f;
    public LayerMask obstacleMask;

    [Header("Vision Light")]
    public Light visionLight;

    [Header("Caught Settings")]
    public GameObject caughtPanel;
    public float caughtDelay = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip caughtSound;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex = 0;
    private bool isIdle = false;
    private bool sudahKetahuan = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.speed = wanderSpeed;

        if (caughtPanel != null) caughtPanel.SetActive(false);

        // Cari player otomatis kalau belum di-assign
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (sudahKetahuan) return;

        UpdatePatrol();
        UpdateVisionLight();
        CheckPlayerDetection();
    }

    // =====================
    // PATROL
    // =====================

    void UpdatePatrol()
    {
        if (isIdle) return;
        if (patrolPoints.Length == 0) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(IdleAtPoint());
        }

        // Update animasi
        if (animator != null)
            animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
    }

    IEnumerator IdleAtPoint()
    {
        isIdle = true;

        if (animator != null)
            animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(idleWaitTime);

        isIdle = false;
        GoToNextPatrolPoint();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

        if (animator != null)
            animator.SetBool("IsWalking", true);
    }

    // =====================
    // VISION LIGHT
    // =====================

    void UpdateVisionLight()
    {
        if (visionLight == null) return;

        // Sinkronkan range dan angle light dengan detection settings
        visionLight.range = detectionRange;
        visionLight.spotAngle = detectionAngle;
    }

    // =====================
    // DETEKSI PLAYER
    // =====================

    void CheckPlayerDetection()
    {
        if (player == null) return;

        Vector3 dirKePlayer = player.position - transform.position;
        float jarak = dirKePlayer.magnitude;

        // Cek jarak
        if (jarak > detectionRange) return;

        // Cek sudut
        float sudut = Vector3.Angle(transform.forward, dirKePlayer);
        if (sudut > detectionAngle / 2f) return;

        // Cek ada obstacle atau tidak (raycast)
        RaycastHit hit;
        if (Physics.Raycast(
            transform.position + Vector3.up * 1.6f,
            dirKePlayer.normalized,
            out hit,
            detectionRange,
            ~obstacleMask))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                PlayerKetahuan();
            }
        }
    }

    // =====================
    // KETAHUAN
    // =====================

    void PlayerKetahuan()
    {
        if (sudahKetahuan) return;
        sudahKetahuan = true;

        Debug.Log("Player ketahuan satpam!");

        // Stop patrol
        agent.isStopped = true;

        if (animator != null)
            animator.SetBool("IsWalking", false);

        // Ganti warna light jadi merah
        if (visionLight != null)
            visionLight.color = Color.red;

        // Suara ketahuan
        if (audioSource != null && caughtSound != null)
            audioSource.PlayOneShot(caughtSound);

        StartCoroutine(TriggerKalah());
    }

    IEnumerator TriggerKalah()
    {
        // Tampilkan panel caught
        if (caughtPanel != null)
            caughtPanel.SetActive(true);

        yield return new WaitForSeconds(caughtDelay);

        // Reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    // =====================
    // GIZMOS (debug visual di Scene view)
    // =====================

    void OnDrawGizmosSelected()
    {
        // Gambar jangkauan deteksi
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Gambar sudut deteksi
        Vector3 kiri = Quaternion.Euler(0, -detectionAngle / 2f, 0) * transform.forward;
        Vector3 kanan = Quaternion.Euler(0, detectionAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + Vector3.up * 1.6f, kiri * detectionRange);
        Gizmos.DrawRay(transform.position + Vector3.up * 1.6f, kanan * detectionRange);
    }
}