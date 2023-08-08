#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Equip))]
public class EquipEditor : Editor
{
    Equip value;

    void OnEnable()
    {
        value = (Equip)target;
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

