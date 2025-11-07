using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class doctor : MonoBehaviour
{
    [Header("Target")]
    public Transform player; 

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 2f; 

    [Header("Animation")]
    public Animator animator;
    public string walkBoolName = "isWalk"; 
    private string talkBoolName = "isTalking";

    private NavMeshAgent agent;
    private bool isChasing = false; 
    private HealthEnemy healthEnemy;
    private GameManager gameManager;
    private bool isCurrentlyTalking = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.isStopped = true; // Başlangıçta dursun diye
        healthEnemy = GetComponent<HealthEnemy>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (healthEnemy != null && healthEnemy.isDead)
        {
            // --- ÖLÜM KODU  ---
            if (agent.enabled)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
            if (animator != null)
            {
                animator.SetBool(walkBoolName, false);
            }
            StartCoroutine(DeathSequence());
            isChasing = false;
            return;
            
        }

        
        if (isChasing && player != null)
        {
            
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0; // Y ekseninde dönmesin

            if (directionToPlayer.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Hızlı ve net dönsün diye
            }
        }
    }

    //  Kapıdan geçince çağrılacak fonksiyon
    public void ActivateChase(Transform target)
    {
        player = target;
        isChasing = true;   // Update'teki Slerp -dönme- kodunu başlatır

        
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        

        // başlangıç animasyonunu 'Idle' yapar
        animator.SetBool(walkBoolName, false);

        
    }

    public void SetTalking(bool isTalking)
    {
        
        isCurrentlyTalking = isTalking;
        if (animator != null)
        {
            animator.SetBool(talkBoolName, isTalking);
        }
    }

    private IEnumerator DeathSequence()
    {
        
        yield return new WaitForSeconds(3.5f);

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
            gameManager.PlayerWin();
       
    }
}