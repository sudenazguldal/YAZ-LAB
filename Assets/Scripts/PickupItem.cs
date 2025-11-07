using UnityEngine;


public enum ItemType
{
    Ammo,
    HealthKit,
    Key
}


public class PickupItem : MonoBehaviour
{
    
    public ItemType type;

   
    [Header("Item Value")]
   
    public int value = 1;

    [Header("Visual Settings")]
    [SerializeField] private float rotationSpeed = 50f;

    void Update()
    {
        // Görsel efekt yavaþça dönme
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
                // item türünü atar
                player.SetNearItem(this);
               
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
                // item referansýný temizler
                player.SetNearItem(null);
            }
        }
    }

    public void Collect(InventoryCollector collector)
    {
        switch (type)
        {
            case ItemType.Ammo:
                collector.CollectAmmo(value); 
                break;
            case ItemType.HealthKit:
                collector.CollectHealthKit();
                break;
            case ItemType.Key:
                collector.CollectKey();
                break;
            default:
                
                break;
        }

        // Objenin dünyadan silinmesi
        Destroy(gameObject);
    }
}