using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PotionScriptable))]
public class PotionScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("potionType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("potionName"));
        
        EditorGUILayout.LabelField("Potion Properties", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();

        SerializedProperty iconProp = serializedObject.FindProperty("potionIcon");
        iconProp.objectReferenceValue = EditorGUILayout.ObjectField(iconProp.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(64), GUILayout.Height(64));

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("potionScoreValue"), new GUIContent("Score Value"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("potency"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Potion Description");
        SerializedProperty descriptionProp = serializedObject.FindProperty("potionDescription");
        descriptionProp.stringValue = EditorGUILayout.TextArea(descriptionProp.stringValue, GUILayout.Height(100));

        serializedObject.ApplyModifiedProperties();
    }
}
