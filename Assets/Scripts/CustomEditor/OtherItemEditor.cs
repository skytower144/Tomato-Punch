#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(OtherItem))]
public class OtherItemEditor : Editor
{
    OtherItem value;

    void OnEnable()
    {
        value = (OtherItem)target;
    }
    public override void OnInspectorGUI()
    {
        value.ItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", value.ItemIcon, typeof(Sprite), true);
        
        base.OnInspectorGUI();
        MarkSceneDirty();
        serializedObject.ApplyModifiedProperties();
    }

    private void MarkSceneDirty()
    {
        if (!GUI.changed) return;

        if (!UnityEditor.EditorApplication.isPlaying) {
            var behavior = target as MonoBehaviour;
            if (behavior) {
                EditorUtility.SetDirty(behavior);
                EditorSceneManager.MarkSceneDirty(behavior.gameObject.scene);
            }
        }
    }
}
#endif

