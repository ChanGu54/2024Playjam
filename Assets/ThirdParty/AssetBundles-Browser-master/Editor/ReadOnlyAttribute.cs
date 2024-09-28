namespace AssetBundleBrowser
{
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;

    public class ReadOnlyPropertyAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(ReadOnlyPropertyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousGUIState = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = previousGUIState;
        }
    }
#endif
}