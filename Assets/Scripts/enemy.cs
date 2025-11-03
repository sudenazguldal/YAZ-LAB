using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class enemy1 : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;

    [Header("Target")]
    public Transform player;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    private int patrolIndex = 0;

    [Header("Ranges & Timing")]
    public float detectRange = 15f;
    public float stopRange = 3f;
    public float idleDuration = 3f;
    private float idleTimer = 0f;
    public float attackCooldown = 0.7f;
    private float lastAttackTime;

    [Header("Throw")]
    public GameObject WeapeonPrefab;
    public Transform throwPoint;
    public float throwForce = 7f;
    public float projectileLifetime = 6f;

    [Header("Animation")]
    public Animator animator;
    public string runBoolName = "isrun";
    public string attackBoolName = "isattack";

    private NavMeshAgent agent;
    private HealthEnemy healthEnemy;
    private float distance;
    private bool triedWarpOnce = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthEnemy = GetComponent<HealthEnemy>();
    }

    void Start()
    {
        agent.speed = 1.5f;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        EnsureOnNavMesh();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (healthEnemy != null && healthEnemy.isDead)
            return;

        if (agent == null || !agent.enabled)
            return;

        if (player == null)
            return;

        distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;

            case EnemyState.Patrol:
                PatrolState();
                break;

            case EnemyState.Chase:
                ChaseState();
                break;

            case EnemyState.Attack:
                AttackState();
                break;
        }
    }


    private void IdleState()
    {
        agent.isStopped = true;
        animator.SetBool(runBoolName, false);
        animator.SetBool(attackBoolName, false);

        idleTimer += Time.deltaTime;


        if (distance <= detectRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            ChangeState(EnemyState.Patrol);
        }
    }

    private void PatrolState()
    {
        if (patrolPoints.Length == 0)
        {
            ChangeState(EnemyState.Idle);
            return;
        }
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[patrolIndex].position);
        animator.SetBool(runBoolName, true);


        if (distance <= detectRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            patrolIndex = (patrolIndex == 0) ? 1 : 0;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void ChaseState()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool(runBoolName, true);
        animator.SetBool(attackBoolName, false);


        if (distance <= stopRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        if (distance > detectRange * 1.2f)
        {
            ChangeState(EnemyState.Idle);
        }
    }

    private void AttackState()
    {
        agent.isStopped = true;
        animator.SetBool(runBoolName, false);

        Vector3 faceDir = (player.position - transform.position);
        faceDir.y = 0;
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(faceDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 6f);
        }


        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetBool(attackBoolName, true);
        }

        if (distance > stopRange + 1f)
        {
            ChangeState(EnemyState.Chase);
        }
    }


    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        idleTimer = 0f;
    }


    public void ThrowBone()
    {
        if (WeapeonPrefab == null || throwPoint == null || player == null)
            return;

        float currentDistance = Vector3.Distance(transform.position, player.position);
        if (currentDistance > stopRange)
            return;

        GameObject bone = Instantiate(WeapeonPrefab, throwPoint.position, throwPoint.rotation);
        Vector3 direction = (player.position + Vector3.up - throwPoint.position).normalized;

        if (bone.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }

        lastAttackTime = Time.time;
        Destroy(bone, projectileLifetime);
    }

    public void EndAttack()
    {
        animator.SetBool(attackBoolName, false);
        lastAttackTime = Time.time;

        // Oyuncu hala menzildeyse tekrar saldırabilir
        if (distance <= stopRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else
        {
            ChangeState(EnemyState.Chase);
        }
    }

    private void EnsureOnNavMesh()
    {
        if (agent == null) return;
        if (!agent.enabled) agent.enabled = true;

        if (agent.isOnNavMesh) return;

        if (!triedWarpOnce)
        {
            triedWarpOnce = true;
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
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
