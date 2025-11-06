using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    private GameManager gameManager;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.isStopped = true;
        healthEnemy = GetComponent<HealthEnemy>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        //Enemy öldüyse
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
                // animator.SetTrigger("Die"); // Ölüm animasyonu istersen buradan tetikle
            }
            StartCoroutine(DeathSequence());
            isChasing = false;
            return;
        }

        //Normal takip
        if (isChasing && player != null)
        {
            agent.SetDestination(player.position);

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

    //  Kapıdan geçince çağrılacak fonksiyon
    public void ActivateChase(Transform target)
    {
        player = target;
        isChasing = true;
        agent.isStopped = false;
        animator.SetBool(walkBoolName, true);
        Debug.Log($"{name} player'ı fark etti ve yürümeye başladı!");
    }
    private IEnumerator DeathSequence()
    {
        // Ölüm animasyonu süresi kadar bekle (örneğin 2.5 saniye)
        yield return new WaitForSeconds(3.5f);

        // GameManager'ı bul
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        // Lose paneli göster
        if (gameManager != null)
            gameManager.PlayerWin();
        else
            Debug.LogError("❌ GameManager sahnede bulunamadı!");
    }
}