#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(OtherItem))]
public class OtherItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        OtherItem value = (OtherItem)target;

        value.ItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", value.ItemIcon, typeof(Sprite), true);
        
        base.OnInspectorGUI();
        MarkSceneDirty();
        serializedObject.ApplyModifiedProperties();
    }

    private void MarkSceneDirty()
    {
        if (!GUI.changed) return;

        if (!UnityEditor.EditorApplication.isPlaying) {
            var behavior = target as OtherItem;
            if (behavior) {
                EditorUtility.SetDirty(behavior);
            }
        }
    }
}
#endif

