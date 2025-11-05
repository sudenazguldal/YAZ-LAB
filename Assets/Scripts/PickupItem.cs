using UnityEngine;


public enum ItemType
{
    Ammo,
    HealthKit,
    Key
}


public class PickupItem : MonoBehaviour
{
    // HATA ÇÖZÜMÜ: InventoryCollector'ýn beklediði tip bilgisi
    public ItemType type;

    // HATA ÇÖZÜMÜ: Ammo için miktar, HealthKit için iyileþtirme miktarý
    [Header("Item Value")]
    [Tooltip("Mermi ise eklenecek miktar, Can Kiti ise iyileþtirme miktarý.")]
    public int value = 1;

    [Header("Visual Settings")]
    [SerializeField] private float rotationSpeed = 50f;

    void Update()
    {
        // Görsel efekt: Yavaþça dön
        if (rotationSpeed != 0)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // Oyuncuya, bu objenin toplanmaya hazýr olduðunu bildir.
                player.SetNearItem(this);
                // Ýpucu: Burada UI'da "E tuþu ile topla" yazýsý belirebilir.
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Oyuncu uzaklaþtý, referansý sýfýrla.
                player.SetNearItem(null);
            }
        }
    }

    public void Collect(InventoryCollector collector)
    {
        switch (type)
        {
            case ItemType.Ammo:
                collector.CollectAmmo(value); // Miktar (value) ile çaðrýlýr
                break;
            case ItemType.HealthKit:
                collector.CollectHealthKit();
                break;
            case ItemType.Key:
                collector.CollectKey();
                break;
            default:
                Debug.LogWarning($"Bilinmeyen öðe türü: {type}. Toplanmadý.");
                break;
        }

        // Objenin dünyadan kaybolmasý
        Destroy(gameObject);
    }
}