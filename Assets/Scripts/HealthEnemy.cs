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

    public Animator animator;
    public string dieTriggerName = "isdie";

    private bool isDead = false;

    Collider[] colliders;
    bool _dead;
    NavMeshAgent agent;
    void Awake()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        colliders = GetComponentsInChildren<Collider>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        // Test için: H tuþuna basýnca 20 hasar ver
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(20f);
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(10f);
        }


    }
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        // --- YENÝ EKLENTÝ (SADECE EDITOR'DE GÜNCELLEMEK ÝÇÝN) ---
#if UNITY_EDITOR
        // Eðer oyun çalýþýyorsa ve bu bileþen seçiliyse, Inspector'ý yeniden çiz.
        if (EditorUtility.IsDirty(this))
        {
            EditorUtility.SetDirty(this);
        }
#endif
        // --------------------------------------------------------

        if (currentHealth <= 0)
            Die();

    }
    void Die()
    {
        _dead = true;

        if (agent && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        foreach (var col in colliders) col.enabled = false;

        animator.SetTrigger(dieTriggerName);

        Destroy(gameObject, destroyDelay);
    }

}
