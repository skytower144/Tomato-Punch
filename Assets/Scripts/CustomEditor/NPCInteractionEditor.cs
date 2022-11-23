using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCController))]
public class NPCInteractionEditor : Editor
{
    private SerializedProperty lock_u, lock_r, lock_d, lock_l, lock_ru, lock_rd, lock_ld, lock_lu;
    private SerializedProperty canBattle, instantBattle, enemyData;
    internal void OnEnable()
    {
        lock_u = serializedObject.FindProperty("lock_u");
        lock_r = serializedObject.FindProperty("lock_r");
        lock_d = serializedObject.FindProperty("lock_d");
        lock_l = serializedObject.FindProperty("lock_l");

        lock_ru = serializedObject.FindProperty("lock_ru");
        lock_rd = serializedObject.FindProperty("lock_rd");
        lock_ld = serializedObject.FindProperty("lock_ld");
        lock_lu = serializedObject.FindProperty("lock_lu");

        canBattle = serializedObject.FindProperty("canBattle");
        instantBattle = serializedObject.FindProperty("instantBattle");
        enemyData = serializedObject.FindProperty("enemyData");
    }
    public override void OnInspectorGUI()
    {
        // If we call base the default inspector will get drawn too.
        // Remove this line if you don't want that to happen.
        base.OnInspectorGUI();

        serializedObject.Update();

        NPCController npcControl = target as NPCController;

        npcControl.banInteractDirection = EditorGUILayout.Toggle("Ban Interact Direction", npcControl.banInteractDirection);

        if (npcControl.banInteractDirection)
        {
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            npcControl.lock_u = EditorGUILayout.Toggle("UP: ", npcControl.lock_u);
            npcControl.lock_r = EditorGUILayout.Toggle("RIGHT: ", npcControl.lock_r);
            npcControl.lock_d = EditorGUILayout.Toggle("DOWN: ", npcControl.lock_d);
            npcControl.lock_l = EditorGUILayout.Toggle("LEFT: ", npcControl.lock_l);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            npcControl.lock_ru = EditorGUILayout.Toggle("RU: ", npcControl.lock_ru);
            npcControl.lock_rd = EditorGUILayout.Toggle("RD: ", npcControl.lock_rd);
            npcControl.lock_ld = EditorGUILayout.Toggle("LD: ", npcControl.lock_ld);
            npcControl.lock_lu = EditorGUILayout.Toggle("LU: ", npcControl.lock_lu);
            EditorGUILayout.EndHorizontal();
        
            EditorGUI.indentLevel--;   
        }

        EditorGUILayout.Space();
        npcControl.canBattle = EditorGUILayout.Toggle("Can Battle", npcControl.canBattle);
        

        if (npcControl.canBattle)
        {
            // GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 200;
            EditorGUI.indentLevel++;

            npcControl.instantBattle = EditorGUILayout.Toggle("Instant Battle", npcControl.instantBattle);
            npcControl.enemyData = EditorGUILayout.ObjectField("Battle Data", npcControl.enemyData, typeof(EnemyBase), true) as EnemyBase;
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }

}
