#if UNITY_EDITOR
namespace AssetBundleBrowser
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

    public class STZAssetPopupWindow : PopupWindowContent
    {
        public AssetBundleBuild[] BuildList { get; set; }
        private Dictionary<string, bool> _assetBundleDataDict = new Dictionary<string, bool>();
        private string _subject;
        private STZAssetConfigTab _parent;
        private Vector2 scrollAdd = Vector2.one;

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 150);
        }

        public void SetSubject(string subject, STZAssetConfigTab parent)
        {
            _subject = subject;
            _parent = parent;
        }

        public override void OnOpen()
        {
            Reset();
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginVertical();
            scrollAdd = EditorGUILayout.BeginScrollView(scrollAdd);
            List<string> keys = _assetBundleDataDict.Keys.ToList<string>();
            foreach (var key in keys)
            {
                _assetBundleDataDict[key] = EditorGUILayout.Toggle(key, _assetBundleDataDict[key]);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        }

        public void Reset()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            _assetBundleDataDict.Clear();

            // 에셋 번들 목록을 가져옴
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            BuildList = new AssetBundleBuild[assetBundleNames.Length];
            for (int i = 0; i < BuildList.Length; i++)
            {
                string[] nameArr = assetBundleNames[i].Split('.');
                BuildList[i].assetBundleName = nameArr[0];

                // 레이아웃에 리스트 목록을 추가
                string data = BuildList[i].assetBundleName;

                if (data.Equals(_subject))
                {
                    continue;
                }

                if (AssetConfigData.Instance.assetConfigDict.TryGetValue(_subject, out var value))
                {
                    _assetBundleDataDict.Add(data, value.dependList.Contains(data));
                }
                else
                {
                    _assetBundleDataDict.Add(data, false);
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            if (AssetConfigData.Instance.assetConfigDict.TryGetValue(_subject, out var value) == false)
            {
                return;
            }

            value.dependList.Clear();
            foreach (var keyValuePair in _assetBundleDataDict)
            {
                if (keyValuePair.Value)
                {
                    value.dependList.Add(keyValuePair.Key);
                }
            }

            AssetConfigData.Save(AssetConfigData.Instance);
        }
    }
}
#endif