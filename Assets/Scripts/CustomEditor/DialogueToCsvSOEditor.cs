using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueToCsvSO))]
public class DialogueToCsvSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueToCsvSO updater = (DialogueToCsvSO)target;

        if (GUILayout.Button("Extract Text and Update CSV"))
        {
            updater.ExecuteUpdate();
        }
        GUILayout.Label($"Last Updated: {PlayerPrefs.GetString("DialogueCSV-LastUpdated")}");
    }
}
