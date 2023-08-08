#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Consumable))]
public class ConsumableEditor : Editor
{
    Consumable value;

    void OnEnable()
    {
        value = (Consumable)target;
    }

    public override void OnInspectorGUI()
    {
       
        value.ItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", value.ItemIcon, typeof(Sprite), true);


        base.OnInspectorGUI();
    }
}
#endif

