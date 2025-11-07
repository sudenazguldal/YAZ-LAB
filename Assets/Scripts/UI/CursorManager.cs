using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        //  Uçta -sol üstte- tıklama noktası
        Vector2 hotSpot = Vector2.zero;

        //  Yeni imleci ayarlae
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        //  Görünür olduğundan emin oluruz
        Cursor.visible = true;
    }
}
