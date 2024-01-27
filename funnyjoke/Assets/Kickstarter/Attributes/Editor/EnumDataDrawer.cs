using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumData))]
public class EnumDataDrawer : PropertyDrawer
{
    private SerializedProperty array;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string path = property.propertyPath;
        if (array == null)
        {
            array = property.serializedObject.FindProperty(path[..path.LastIndexOf('.')]);
            if (array == null)
            {
                EditorGUI.LabelField(position, "Use EnumData Attribute on Arrays");
                return;
            }
        }
        
        var enumData = (EnumData)attribute;
        if (array.arraySize != enumData.Names.Length)
            array.arraySize = enumData.Names.Length;

        int index = Convert.ToInt32(path[(path.IndexOf('[') + 1)..].Replace("]", ""));
        label.text = enumData.Names[index];
        EditorGUI.PropertyField(position, property, label, true);
    }
}
