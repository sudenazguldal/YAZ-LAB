using UnityEngine;

public class PathDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log(" Application.dataPath = " + Application.dataPath);
        Debug.Log(" Application.persistentDataPath = " + Application.persistentDataPath);
    }
}
