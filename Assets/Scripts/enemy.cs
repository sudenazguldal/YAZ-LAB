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
    public float attackCooldown = 0.7f; // Her tam saldırı döngüsü (animasyon + bekleme) arası süre
    private float lastAttackTime;

    [Header("Throw")]
    public GameObject bonePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    // public float throwUpward = 0; // Doğrudan fırlatma için bu değişkene artık gerek yok

    [Header("Animation")]
    public Animator animator;
    public string runBoolName = "isrun";
    public string attackTriggerName = "isattack"; // Trigger olarak değiştirildi!

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

        // 1) Takip (Menzil İçi Ama Saldırı Dışı)
        if (distance <= detectRange && distance > stopRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (animator)
            {
                animator.SetBool(runBoolName, true);
                // Saldırıya başlama sinyalini burada kesiyoruz
                // NOT: Trigger'lar otomatik sıfırlandığı için burada SetBool kullanmadık.
            }
        }
        // 2) Saldırı Menzilinde
        else if (distance <= stopRange)
        {
            agent.isStopped = true;

            // Oyuncuya döner (Saldırı öncesi hedefi doğrular)
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

            // Cooldown dolduysa saldırı animasyonunu BAŞLAT
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                if (animator)
                {
                    // SADECE animasyonu tetikle! Kemik fırlatma işi Animasyon Olayında.
                    animator.SetTrigger(attackTriggerName);
                }
                // lastAttackTime, kemik FİİLİ OLARAK fırlatıldığında (ThrowBone() içinde) güncellenecek.
            }
        }

        // 3) Çok Uzakta
        else
        {
            agent.isStopped = true;
            if (animator)
            {
                animator.SetBool(runBoolName, false);
                // Saldırı sinyallerini temizle
            }
        }
    }

    // --- Kemik fırlatma (Animation Event tarafından çağrılacak) ---
    // PUBLIC OLMALIDIR!
    public void ThrowBone()
    {
        // YALNIZCA KEMİK FIRLATILDIĞINDA COOLDOWN SÜRESİNİ SIFIRLA
        lastAttackTime = Time.time;

        if (bonePrefab == null || throwPoint == null || player == null) return;

        // 1. Kemik oluştur
        GameObject bone = Instantiate(bonePrefab, throwPoint.position, throwPoint.rotation);

        // 2. Yön hesaplama (Düzeltilmiş: Doğrudan hedefe)
        // Oyuncunun göğüs yüksekliğini hedefle
        Vector3 targetPosition = player.position + Vector3.up;

        // Fırlatma noktasından hedefe olan saf yön vektörünü hesapla
        Vector3 direction = (targetPosition - throwPoint.position).normalized;

        // 3. Kuvvet Uygulama
        if (bone.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Hesaplanan yönde Impulse (Ani Kuvvet) uygula
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }

        // sahneyi kalabalıklaştırmamak için
        Destroy(bone, 6f);
    }
}
