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
    private string talkBoolName = "isTalking";

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
            // 1. Hareketi durdur (Garanti olsun)
            agent.isStopped = true;
            animator.SetBool(walkBoolName, false);

            // 2. Oyuncuya doğru dön
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0; // Y ekseninde dönmesin

            if (directionToPlayer.sqrMagnitude > 0.01f) // Çok yakınsa dönmeyi durdur
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
            }
        }
    }

    //  Kapıdan geçince çağrılacak fonksiyon
    public void ActivateChase(Transform target)
    {
        player = target;
        isChasing = true; 

       
        agent.isStopped = true;

       
        animator.SetBool(walkBoolName, false);

        Debug.Log($"{name} player'ı fark etti ve YERİNDE DURUYOR!");

        
    }

    public void SetTalking(bool isTalking)
    {
        if (animator != null)
        {
            animator.SetBool(talkBoolName, isTalking);
        }
    }
}