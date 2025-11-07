using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture; 
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        // 🔹 Uçta (sol üstte) tıklama noktası
        Vector2 hotSpot = Vector2.zero;

        // 🔹 Yeni imleci ayarla
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        // 🔹 Görünür olduğundan emin ol
        Cursor.visible = true;
    }
}
