using UnityEngine;

public class WorldCrosshairController : MonoBehaviour
{
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float crossHairOffsetMultiplier = 0.001f;
    //nişangah üstünde durudğu nesneye gömülmesin biraz dışarda dursun ki görülebilsin diye
    [SerializeField] private LayerMask raycastMask = ~0;
    //hemen hemen her şeyle çarpışabilir demek layerlarla ayarlayabiliyorsun hangisinde çıksın diye
    //sadece enemy layerı yapabilirsin

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        //fareyi gizler
        Cursor.lockState = CursorLockMode.Locked;
        //cursoru otomatik ekranın ortasına kitler
    }

    // Update is called once per frame
    void LateUpdate()
    //lateUpdate olma sebebi update de genel hareket hesaplanıyor. 
    //tüm bu hesaplamalar bitince lateupdate çalışıp cursoru yine sayfanın ortasına ayarlıyor
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        //ekranın tam orta koordinatları alınıyor
        Ray ray = aimCamera.ScreenPointToRay(screenCenter);
        //ekranın tam ortasına bir ışın gönderiyosun örn fener

        Vector3 targetPos;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, raycastMask))
        //bool döndürür ışının bir şeye çarpıp çarpmadığına bakar
        {
            targetPos = hit.point + hit.normal * crossHairOffsetMultiplier;
            crosshairUI.rotation = Quaternion.LookRotation(hit.normal);
            Debug.DrawLine(hit.point, hit.point + hit.normal * 2f, Color.green);
        }
        else
        {
            targetPos = ray.GetPoint(maxDistance);
            crosshairUI.forward = aimCamera.transform.forward;
        }

        crosshairUI.position = targetPos;
    }
    //Ekranın ortasından hayali bir lazer atıyoruz.

    //Bir şeye çarparsa → oraya nişangah koy.

    //Çarpmazsa → ileriye doğru boşluğa koy.

    //Fareyi gizle, sadece oyundaki nişangah kalsın.
}
