using UnityEngine;
using UnityEngine.AI;

public class doctor : MonoBehaviour
{
    [Header("Target")]
    public Transform player; // Hedef (Player)

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 2f; // Ne kadar yaklaşsın

    [Header("Animation")]
    public Animator animator;
    public string walkBoolName = "isWalk"; // Animator'daki parametre ismi (örnek: isWalk)

    private NavMeshAgent agent;
    private bool isChasing = false;
    private HealthEnemy healthEnemy;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.isStopped = true;
        healthEnemy = GetComponent<HealthEnemy>();
    }

    void Update()
    {
        if (healthEnemy != null && healthEnemy.isDead)
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
            if (animator != null)
            {
                animator.SetBool(walkBoolName, false);
                // Burada ölüm animasyonu tetikleyicisini de çağırabilirsiniz:
               // animator.SetTrigger("Die"); 
            }
            isChasing = false; // Kovalamayı tamamen durdur
            return; // Update'in geri kalanını çalıştırma
        }
        if (isChasing && player != null)
        {
            agent.SetDestination(player.position);

            // Hedefe yaklaşınca dur
            if (Vector3.Distance(transform.position, player.position) <= stoppingDistance)
            {
                agent.isStopped = true;
                animator.SetBool(walkBoolName, false);
            }
            else
            {
                agent.isStopped = false;
                animator.SetBool(walkBoolName, true);
            }
        }
    }

    // 🔥 Kapıdan geçince çağrılacak fonksiyon
    public void ActivateChase(Transform target)
    {
        player = target;
        isChasing = true;
        agent.isStopped = false;
        animator.SetBool(walkBoolName, true);
        Debug.Log($"{name} player'ı fark etti ve yürümeye başladı!");
    }
}
