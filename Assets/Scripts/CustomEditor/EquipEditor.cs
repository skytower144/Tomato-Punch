#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Equip))]
public class EquipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Equip value = (Equip)target;
        
        value.ItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", value.ItemIcon, typeof(Sprite), true);
        
        base.OnInspectorGUI();
        MarkSceneDirty();
        serializedObject.ApplyModifiedProperties();
    }

    private void MarkSceneDirty()
    {
        if (!GUI.changed) return;

        if (!UnityEditor.EditorApplication.isPlaying) {
            var behavior = target as Equip;
            if (behavior) {
                EditorUtility.SetDirty(behavior);
            }
        }
    }
}
#endif

