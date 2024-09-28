#if UNITY_EDITOR
namespace AssetBundleBrowser
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using AssetBundleBrowser;
    using System;
    using System.Text;
    using System.Linq;

    public class STZAssetConfigTab
    {
        private AssetBundleBrowserMain _parent;
        private Vector2 scrollAdd = Vector2.one;
        private Rect[] buttonRect;
        public AssetBundleBuild[] BuildList { get; set; }
        private bool _isInit = false;

        internal void OnEnable(EditorWindow parent)
        {
            _parent = (parent as AssetBundleBrowserMain);
            Reset();
        }

        public void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                GUILayout.TextArea("\n\n내용 반영 중 입니다.", GUILayout.Height(100));
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                if (AssetBundleBrowser.AssetBundleModel.Model.Update())
                {
                    Reset();
                }
            }

            EditorGUILayout.LabelField("Asset Bundle Config", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            string[] headers = new string[4] { "에셋 번들 이름", "의존성 있는 에셋 번들", "압축 여부", "목록 삭제" };
            var centerAlignLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField(headers[0], centerAlignLabelStyle, GUILayout.Width(Screen.width / headers.Length));
            EditorGUILayout.LabelField(headers[1], centerAlignLabelStyle, GUILayout.Width(Screen.width / headers.Length));
            EditorGUILayout.LabelField(headers[2], centerAlignLabelStyle, GUILayout.Width(Screen.width / headers.Length));
            EditorGUILayout.LabelField(headers[3], centerAlignLabelStyle, GUILayout.Width(Screen.width / headers.Length));
            EditorGUILayout.EndHorizontal();

            buttonRect = new Rect[AssetConfigData.Instance.assetConfigDict.Count];
            scrollAdd = EditorGUILayout.BeginScrollView(scrollAdd);
            int idx = 0;
            List<string> keys = AssetConfigData.Instance.assetConfigDict.Keys.ToList();
            foreach (var key in keys)
            {
                EditorGUILayout.BeginHorizontal();

                /// costumes/cos01/16/cos01 일 경우 마지막 파일명인 cos01 만 리턴
                string[] vs = key.Split('/');
                string targetName = vs[vs.Length - 1];

                EditorGUILayout.LabelField(targetName, centerAlignLabelStyle, GUILayout.Width(Screen.width / headers.Length));
                GUILayout.FlexibleSpace();
                buttonRect[idx] = EditorGUILayout.BeginVertical();
                string optionButtonName = "Check Dependency Option";
                if (AssetConfigData.Instance.assetConfigDict[key].dependList.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < AssetConfigData.Instance.assetConfigDict[key].dependList.Count; i++)
                    {
                        sb.Append(AssetConfigData.Instance.assetConfigDict[key].dependList[i]);
                        sb.Append(", ");
                    }
                    sb.Remove(sb.Length - 2, 2);
                    optionButtonName = sb.ToString();
                }
                if (GUILayout.Button(optionButtonName, GUILayout.Width(Screen.width / headers.Length)))
                {
                    var popupWindow = new STZAssetPopupWindow();
                    popupWindow.SetSubject(key, this);
                    PopupWindow.Show(buttonRect[idx], popupWindow);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    buttonRect[idx] = GUILayoutUtility.GetLastRect();
                }

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                /* REMOVED @hyeyun 압축 여부 변경이 불가능해서 버튼으로 교체
                AssetConfigData.Instance.assetConfigDict[key].isCompressed = EditorGUILayout.Toggle(AssetConfigData.Instance.assetConfigDict[key].isCompressed, GUILayout.Width(0));
                //*/
                GUIStyle style = new GUIStyle(GUI.skin.button);
                bool isCompressed = AssetConfigData.Instance.assetConfigDict[key].isCompressed;
                string compressionLabel = isCompressed ? "예" : "아니오";
                style.normal.textColor = isCompressed ? Color.white : Color.red;
                if (GUILayout.Button(compressionLabel, style, GUILayout.Width(Screen.width / headers.Length)))
                {
                    AssetConfigData.Instance.assetConfigDict[key].isCompressed = !AssetConfigData.Instance.assetConfigDict[key].isCompressed;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("삭제", GUILayout.Width(Screen.width / headers.Length)))
                {
                    AssetConfigData.Instance.assetConfigDict.Remove(key);
                }
                EditorGUILayout.EndHorizontal();
                idx++;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        public void Reset()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            // 에셋 번들 목록을 가져옴
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            BuildList = new AssetBundleBuild[assetBundleNames.Length];
            for (int i = 0; i < BuildList.Length; i++)
            {
                string[] nameArr = assetBundleNames[i].Split('.');
                BuildList[i].assetBundleName = nameArr[0];

                // 레이아웃에 리스트 목록을 추가
                string data = BuildList[i].assetBundleName;
                if (AssetConfigData.Instance.assetConfigDict.Keys.Contains(data) == false)
                    AssetConfigData.Instance.assetConfigDict.Add(data, new AssetConfigData.AssetConfig());
            }

            //번들 목록에 없는 경우 지워주려고 했는데, 브랜치 변경시 문제가 될것같아서 일단 주석 처리
            //string[] configArray = AssetConfigData.Instance.assetConfigDict.Keys.ToArray();
            //foreach (string configName in configArray)
            //{
            //    bool hasConfigBundle = false;
            //    for (int i = 0; i < BuildList.Length; i++)
            //    {
            //        if(BuildList[i].assetBundleName == configName)
            //        {
            //            hasConfigBundle = true;
            //        }
            //    }

            //    if(hasConfigBundle == false)
            //    {
            //        AssetConfigData.Instance.assetConfigDict.Remove(configName);
            //    }
            //}

            AssetConfigData.Save(AssetConfigData.Instance);
        }
    }
}
#endif