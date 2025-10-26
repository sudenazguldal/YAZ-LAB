using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class enemy1 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Ranges & Timing")]
    public float detectRange = 10f;
    public float stopRange = 3.0f;
    public float attackCooldown = 0.7f;
    private float lastAttackTime;

    [Header("Throw")]
    public GameObject bonePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    [Header("Animation")]
    public Animator animator;
    public string runBoolName = "isrun";
    public string attackBoolName = "isattack";

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // [YENİ]: Düşmanın mevcut durumda saldırmakta olup olmadığını kontrol eder.
        bool isAttacking = animator && animator.GetBool(attackBoolName);


        if (distance <= detectRange && distance > stopRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (animator)
            {
                animator.SetBool(runBoolName, true);
                // Bu, NPC'nin koşarken saldırı duruşunda kalmasını engeller.
                animator.SetBool(attackBoolName, false);
            }
        }
        else if (distance <= stopRange)
        {
            agent.isStopped = true;
            Vector3 faceDir = (player.position - transform.position);
            faceDir.y = 0;
            if (faceDir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(faceDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 6f);
            }

            if (animator)
            {
                animator.SetBool(runBoolName, false);
            }

            if (!isAttacking && (Time.time - lastAttackTime >= attackCooldown))
            {
                if (animator)
                {
                    animator.SetBool(attackBoolName, true);
                }
            }
        }
        else
        {
            agent.isStopped = true;
            if (animator)
            {
                animator.SetBool(runBoolName, false);
                animator.SetBool(attackBoolName, false);

                // Ayrıca, eğer Idle (Bekleme) animasyonunuzun adını biliyorsanız:
                // animator.CrossFade("Idle", 0.1f); // Animasyonu zorla keser (önceden tartışıldı)
            }
        }
    }
// --- Kemik fırlatma (Animation Event tarafından çağrılacak) ---
// PUBLIC OLMALIDIR!
public void ThrowBone()
    {
        if (bonePrefab == null || throwPoint == null || player == null)
        { 
            return;
        }
        float currentDistance = Vector3.Distance(transform.position, player.position);

        if (currentDistance > stopRange)
        {
             Debug.Log("Hedef menzil dışına çıktı. Fırlatma iptal edildi.");
            return;
        }

        GameObject bone = Instantiate(bonePrefab, throwPoint.position, throwPoint.rotation);
        Debug.Log("Fırlatılan nesne: " + bone.name + " - Script var mı? " + (bone.GetComponent<BoneDamage>() != null));
        Vector3 targetPosition = player.position + Vector3.up;

        Vector3 direction = (targetPosition - throwPoint.position).normalized;

        // Kuvvet Uygulama
        if (bone.TryGetComponent<Rigidbody>(out var rb))
        {
            // Önceki hızları sıfırlayarak temiz bir fırlatma sağlar
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Hesaplanan yönde Impulse (Ani Kuvvet) uygula
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Fırlatılan kemik prefabrikasında Rigidbody bileşeni bulunamadı!", bone);
            Destroy(bone);
            return; 
        }
        lastAttackTime = Time.time;
        Destroy(bone, 6f);
    }
}
