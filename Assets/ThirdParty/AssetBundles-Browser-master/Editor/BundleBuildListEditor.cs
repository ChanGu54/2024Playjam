#if UNITY_EDITOR
namespace AssetBundleBrowser
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;

    [CustomEditor(typeof(BundleBuildList))]

    public class BundleBuildListEditor : Editor
    {
        private SerializedProperty _bundleList;

        private void OnEnable()
        {
            // do this only once here
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BundleBuildList script = (BundleBuildList)target;
            _bundleList = serializedObject.FindProperty("bundleList");

            Dictionary<BundleInfo.E_BUILD_TARGET, List<SerializedProperty>> bundleList = new Dictionary<BundleInfo.E_BUILD_TARGET, List<SerializedProperty>>();

            for (int i = 0; i < script.bundleList.Count; i++)
            {
                var info = script.bundleList[i];
                var infoSP = _bundleList.GetArrayElementAtIndex(i);

                if (bundleList.TryGetValue(info.target, out var targetList) == false)
                {
                    targetList = new List<SerializedProperty>();
                    bundleList.Add(info.target, targetList);
                }

                targetList.Add(infoSP);
            }

            foreach (var bundleInfoList in bundleList)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{bundleInfoList.Key}", EditorStyles.boldLabel, GUILayout.MinHeight(30));
                EditorGUILayout.EndHorizontal();
                var infoList = bundleInfoList.Value;

                foreach (var info in infoList)
                {
                    if (EditorGUILayout.PropertyField(info, true) == true)
                    {
                        Debug.LogError(info.name + "    " + "IS TRUE??");
                    }
                }

                EditorGUILayout.Separator();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif