#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SuperEquip))]
public class SuperEquipEditor : Editor
{
    SuperEquip value;

    void OnEnable()
    {
        value = (SuperEquip)target;
    }
    public override void OnInspectorGUI()
    {
        value.ItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", value.ItemIcon, typeof(Sprite), true);
        value.superIcon = (Sprite)EditorGUILayout.ObjectField("Super Icon", value.superIcon, typeof(Sprite), true);
        base.OnInspectorGUI();
    }
}
#endif

