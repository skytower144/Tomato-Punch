#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

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
    }
}
#endif

