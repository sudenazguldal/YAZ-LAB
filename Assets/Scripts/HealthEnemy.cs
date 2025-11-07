using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealthEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;


    [SerializeField]
    private float currentHealth;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float, float> onHealthChanged;

    [Header("Death")]
    public float destroyDelay = 6f;
    [SerializeField]
    private string dieTriggerName = "isdie";

    public Animator animator;


    public bool isDead = false;

    Collider[] colliders;
    NavMeshAgent agent;

    public delegate void EnemyDeathEvent(HealthEnemy enemy);
    public static event EnemyDeathEvent OnEnemyDeath;

    void Awake()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        colliders = GetComponentsInChildren<Collider>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(20f);
        }
    }
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        
#if UNITY_EDITOR

        if (EditorUtility.IsDirty(this))
        {
            EditorUtility.SetDirty(this);
        }
#endif
        
        

        if (currentHealth <= 0)
            Die();

    }
    void Die()
    {
        if (isDead) return; // Tekrar tetiklenmesin diye kontrol
        isDead = true;

        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;

        foreach (Collider col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }


        if (animator != null)
            animator.SetTrigger(dieTriggerName);

        onDeath?.Invoke();
        OnEnemyDeath?.Invoke(this);

        Destroy(gameObject, destroyDelay);
    }

}
