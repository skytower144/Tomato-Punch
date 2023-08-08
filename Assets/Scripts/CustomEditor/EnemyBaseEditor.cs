#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(EnemyBase))]
public class EnemyBaseEditor : Editor
{
    EnemyBase value;

    void OnEnable()
    {
        value = (EnemyBase)target;
    }

    public override void OnInspectorGUI()
    {
       
        value.defaultFace = (Sprite)EditorGUILayout.ObjectField("Def", value.defaultFace, typeof(Sprite), true);
        value.hurtFace =  (Sprite)EditorGUILayout.ObjectField("Hurt", value.hurtFace, typeof(Sprite), true);
        value.koFace =  (Sprite)EditorGUILayout.ObjectField("KO", value.koFace, typeof(Sprite), true);
        GUILine(4);
        EditorGUILayout.Space();

        base.OnInspectorGUI();
        MarkSceneDirty();
        serializedObject.ApplyModifiedProperties();
    }

    void GUILine( int lineHeight = 1 ) {
        EditorGUILayout.Space();
        Rect rect = EditorGUILayout.GetControlRect(false, lineHeight );
        rect.height = lineHeight;
        EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
        EditorGUILayout.Space();
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