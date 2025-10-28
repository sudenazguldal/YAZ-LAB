using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class enemy1 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Ranges & Timing")]
    public float detectRange = 15f;
    public float stopRange = 2.5f;
    public float attackCooldown = 0.7f;
    private float lastAttackTime;

    [Header("Throw")]
    public GameObject WeapeonPrefab;
    public Transform throwPoint;
    public float throwForce = 8f;

    [Header("Throw Settings")]
    public float projectileSpeed = 2f;
    public float projectileLifetime = 6f;
    public float upBias = 0.5f;

    [Header("Animation")]
    public Animator animator;
    public string runBoolName = "isrun";
    public string attackBoolName = "isattack";

    private NavMeshAgent agent;
    private bool triedWarpOnce = false; // güvenlik flag'i
    private HealthEnemy healthEnemy;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 2.5f;     

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        EnsureOnNavMesh();
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthEnemy = GetComponent<HealthEnemy>();
    }
    private void Update()
    {
        if (healthEnemy != null && healthEnemy.isDead)
            return;
        // Agent yok veya devre dışıysa çık
        if (agent == null || !agent.enabled)
            return;

        // Eğer NavMesh’te değilse bir kere daha yerleştir
        if (!agent.isOnNavMesh)
        {
            EnsureOnNavMesh();
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isAttacking = animator && animator.GetBool(attackBoolName);

        if (distance <= detectRange && distance > stopRange)
        {
            // Takip et
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (animator)
            {
                animator.SetBool(runBoolName, true);
                animator.SetBool(attackBoolName, false);
            }
        }
        else if (distance <= stopRange)
        {
            // Yakın mesafede dur ve yüzünü çevir
            agent.isStopped = true;

            Vector3 faceDir = (player.position - transform.position);
            faceDir.y = 0;
            if (faceDir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(faceDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 6f);
            }

            if (animator)
                animator.SetBool(runBoolName, false);

            if (!isAttacking && (Time.time - lastAttackTime >= attackCooldown))
            {
                if (animator)
                    animator.SetBool(attackBoolName, true);
            }
        }
        else
        {
            // Oyuncu çok uzakta → idle
            agent.isStopped = true;
            if (animator)
            {
                animator.SetBool(runBoolName, false);
                animator.SetBool(attackBoolName, false);
            }
        }
    }

    // --- Kemik fırlatma (Animation Event) ---
    public void ThrowBone()
    {
        if (WeapeonPrefab == null || throwPoint == null || player == null)
            return;

        float currentDistance = Vector3.Distance(transform.position, player.position);
        if (currentDistance > stopRange)
        {
            Debug.Log("Hedef menzil dışına çıktı. Fırlatma iptal edildi.");
            return;
        }

        GameObject bone = Instantiate(WeapeonPrefab, throwPoint.position, throwPoint.rotation);
        Vector3 targetPosition = player.position + Vector3.up;
        Vector3 direction = (targetPosition - throwPoint.position).normalized;

        if (bone.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("prefabında Rigidbody yok!", bone);
            Destroy(bone);
            return;
        }

        lastAttackTime = Time.time;
        Destroy(bone, 6f);
    }

    // --- NavMesh kontrolü ve düzeltme ---
    private void EnsureOnNavMesh()
    {
        if (agent == null)
            return;

        if (!agent.enabled)
            agent.enabled = true;

        if (agent.isOnNavMesh)
            return;

        if (!triedWarpOnce)
        {
            triedWarpOnce = true;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position); // en yakına ışınla
                Debug.Log($"{name}: NavMesh'e warp yapıldı -> {hit.position}");
            }
            else
            {
                Debug.LogError($"{name}: Yakında NavMesh bulunamadı!");
                agent.enabled = false;
            }
        }
    }
}
