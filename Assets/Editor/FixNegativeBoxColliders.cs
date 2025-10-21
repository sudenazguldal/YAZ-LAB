using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class FixNegativeBoxColliders : MonoBehaviour
{
    
    [MenuItem("Tools/Fix Negative Box Colliders")]
    public static void FixSelectedNegativeBoxColliders()
    {
        if (Selection.activeGameObject == null)
            return;

        // editing a Prefab in isolation (or maybe other weird situations)?
        if (StageUtility.GetStage(Selection.activeGameObject) !=
            StageUtility.GetMainStage())
            return;

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Fix Negative Box Colliders");
        int undoID = Undo.GetCurrentGroup();

        foreach (GameObject target in Selection.gameObjects)
        {
            foreach (BoxCollider box in
                target.GetComponentsInChildren<BoxCollider>())
            {
                FixNegativeBoxCollider(box);
            }
        }
        Undo.CollapseUndoOperations(undoID);
    }

    public static void FixNegativeBoxCollider(BoxCollider box)
    {
        Vector3 lossy = box.transform.lossyScale;
        Vector3 flip = new Vector3(
            Mathf.Sign(lossy.x),
            Mathf.Sign(lossy.y),
            Mathf.Sign(lossy.z));
        Vector3 sign = new Vector3(
            Mathf.Sign(box.size.x),
            Mathf.Sign(box.size.y),
            Mathf.Sign(box.size.z));
        if (flip != sign)
        {
            Undo.RecordObject(box, "Fix Negative Box Collider");
            box.size = Vector3.Scale(box.size, flip);
        }
    }
}