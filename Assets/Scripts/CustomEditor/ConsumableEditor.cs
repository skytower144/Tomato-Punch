#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Consumable))]
public class ConsumableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Consumable value = (Consumable)target;

        value.ItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", value.ItemIcon, typeof(Sprite), true);
        
        base.OnInspectorGUI();
        MarkSceneDirty();
        serializedObject.ApplyModifiedProperties();
    }

    private void MarkSceneDirty()
    {
        if (!GUI.changed) return;

        if (!UnityEditor.EditorApplication.isPlaying) {
            var behavior = target as Consumable;
            if (behavior) {
                EditorUtility.SetDirty(behavior);
            }
        }
    }
}
#endif

