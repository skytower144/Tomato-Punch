#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBase))]
public class EnemyBaseEditor : Editor
{
    private SerializedProperty isFixedBg, isParallaxBg, isBoss, bgSprites, bgTexture, parallaxDirection;

    internal void OnEnable()
    {
        isFixedBg = serializedObject.FindProperty("isFixedBg");
        isParallaxBg = serializedObject.FindProperty("isParallaxBg");
        isBoss = serializedObject.FindProperty("isBoss");
        bgSprites = serializedObject.FindProperty("bgSprites");
        bgTexture = serializedObject.FindProperty("bgTexture");
        parallaxDirection = serializedObject.FindProperty("parallaxDirection");
    }

    public override void OnInspectorGUI()
    {
        EnemyBase value = (EnemyBase)target;
        
        value.defaultFace = (Sprite)EditorGUILayout.ObjectField("Def", value.defaultFace, typeof(Sprite), true);
        value.hurtFace = (Sprite)EditorGUILayout.ObjectField("Hurt", value.hurtFace, typeof(Sprite), true);
        value.koFace = (Sprite)EditorGUILayout.ObjectField("KO", value.koFace, typeof(Sprite), true);

        value.isBoss = EditorGUILayout.Toggle("Is Boss", value.isBoss);
        if (value.isBoss)
        {
            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 200;
            value.bossBanner = (Sprite)EditorGUILayout.ObjectField("Banner", value.bossBanner, typeof(Sprite), true);
            EditorGUI.indentLevel--;
        }
        GUILine(4);
        EditorGUILayout.Space();

        value.isFixedBg = EditorGUILayout.Toggle("Fixed BG", value.isFixedBg);
        if (value.isFixedBg)
        {
            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth = 200;
            EditorGUILayout.PropertyField(bgSprites, new GUIContent("Sprite List"));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();

        value.isParallaxBg = EditorGUILayout.Toggle("Parallax BG", value.isParallaxBg);
        if (value.isParallaxBg)
        {
            EditorGUI.indentLevel++;
            
            value.parallaxDirection = EditorGUILayout.Vector2Field("Direction", value.parallaxDirection);

            EditorGUIUtility.labelWidth = 200;
            value.bgTexture = (Texture2D)EditorGUILayout.ObjectField("Parallax", value.bgTexture, typeof(Texture2D), true);
            EditorGUI.indentLevel--;
        }
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
            var behavior = target as EnemyBase;
            if (behavior) {
                EditorUtility.SetDirty(behavior);
            }
        }
    }
}
#endif