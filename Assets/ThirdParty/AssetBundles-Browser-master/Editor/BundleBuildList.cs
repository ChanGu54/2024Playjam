#if UNITY_EDITOR
namespace AssetBundleBrowser
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    [System.Serializable]
    public class BundleBuildList : ScriptableObject
    {
        const string bundleBuildAssetName = "BundleBuildList";

        private static BundleBuildList instance;
        public static BundleBuildList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(bundleBuildAssetName) as BundleBuildList;
                    if (instance == null)
                    {
                        instance = CreateInstance();
                    }
                }

                return instance;
            }

        }

        public List<BundleInfo> bundleList;
        public SortedDictionary<string, BundleInfo> dic_BundleInfo;

        public void SetDicBundleInfo(BundleInfo.E_BUILD_TARGET target)
        {
            if (dic_BundleInfo == null)
                dic_BundleInfo = new SortedDictionary<string, BundleInfo>();

            dic_BundleInfo.Clear();


            for (int i = 0; i < bundleList.Count; ++i)
            {
                BundleInfo bundleInfo = bundleList[i];

                if (bundleInfo.target != BundleInfo.E_BUILD_TARGET.ALL && bundleInfo.target != target)
                {
                    continue;
                }

                if (dic_BundleInfo.ContainsKey(bundleInfo.fileName.ToLower()))
                    dic_BundleInfo[bundleInfo.fileName] = bundleInfo;

                dic_BundleInfo.Add(bundleInfo.fileName.ToLower(), bundleInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBuildTarget"></param>
        /// <param name="inBundleName"></param>
        public void AddBundle(BundleInfo.E_BUILD_TARGET inBuildTarget, string inBundleName)
        {
            inBuildTarget = BundleInfo.E_BUILD_TARGET.ALL;

            BundleInfo bundleInfo = null;
            for (int i = 0; i < bundleList.Count; i++)
            {
                bundleInfo = bundleList[i];
                if (bundleInfo.target != BundleInfo.E_BUILD_TARGET.ALL && bundleInfo.target != inBuildTarget)
                {
                    continue;
                }

                if (bundleInfo.fileName.ToLower() == inBundleName)
                {
                    break;
                }
                else
                {
                    bundleInfo = null;
                }
            }

            if(bundleInfo != null)
            {
                bundleInfo.version += 1;
            }
            else
            {
                BundleInfo newBundleInfo = new BundleInfo()
                {
                    target = inBuildTarget,
                    fileName = inBundleName,
                    version = 1,
                };

                bundleList.Add(newBundleInfo);
            }

            SaveData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBuildTarget"></param>
        /// <param name="inBundleName"></param>
        public void RemoveBundle(BundleInfo.E_BUILD_TARGET inBuildTarget, string inBundleName)
        {
            inBuildTarget = BundleInfo.E_BUILD_TARGET.ALL;

            BundleInfo bundleInfo = null;
            for (int i = 0; i < bundleList.Count; i++)
            {
                bundleInfo = bundleList[i];
                if (bundleInfo.target != BundleInfo.E_BUILD_TARGET.ALL && bundleInfo.target != inBuildTarget)
                {
                    continue;
                }

                if (bundleInfo.fileName.ToLower() == inBundleName)
                {
                    break;
                }
                else
                {
                    bundleInfo = null;
                }
            }

            if(bundleInfo != null)
            {
                bundleInfo.version -= 1;
                if (bundleInfo.version <= 0)
                {
                    bundleList.Remove(bundleInfo);
                }

                SaveData();
            }
        }

        [MenuItem("ANIPANG5/AssetBundle/Bundle Build List/Create Bundle Build List Instance")]
        public static BundleBuildList CreateInstance()
        {
            if (instance == null)
            {
                instance = CreateInstance<BundleBuildList>();

                AssetDatabase.CreateAsset(instance, $"Assets/Resources/{bundleBuildAssetName}.asset");
            }

            return instance;
        }

        [MenuItem("ANIPANG5/AssetBundle/Bundle Build List/Save Bundle Build List")]
        public static void SaveData()
        {
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public List<string> GetFileNames(string bundleName, string version, string target)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile($"{Application.dataPath}/../AssetBundles/{target}/{version}/{bundleName}");
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            var allbundles = bundle.GetAllAssetNames();

            List<string> result = new List<string>();
            for (int i = 0; i < allbundles.Length; ++i)
            {
                var paths = allbundles[i].Split('/');
                result.Add(paths[paths.Length - 1]);
            }

            return result;
        }
    }

    [System.Serializable]
    public class BundleInfo
    {
        public enum E_BUILD_TARGET
        {
            WIN,
            AOS,
            IOS,
            OSX,
            ALL
        }
        public string fileName;
        public E_BUILD_TARGET target;
        public int version;
        //public ServerStatics.AssetbundleUrl urlData;

    }
}
#endif