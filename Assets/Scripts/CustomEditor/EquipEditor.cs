#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

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
    }
}
#endif

