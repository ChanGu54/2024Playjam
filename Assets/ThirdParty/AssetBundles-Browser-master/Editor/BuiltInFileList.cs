#if UNITY_EDITOR
namespace AssetBundleBrowser
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.IO;
    using System.Linq;
    using UnityEditor;

    [Serializable]
    public class BuiltInFileList : ScriptableObject
    {
        private const string BUILT_IN_FILE_ASSET_NAME = "BuiltInFileList";
        private const string ASSET_ROOT_PATH = "Assets/AssetBundles";
        private const string ASSET_TRASH_PATH = "Assets/AssetBundles/Trash";

        private static BuiltInFileList instance;
        public static BuiltInFileList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(BUILT_IN_FILE_ASSET_NAME) as BuiltInFileList;
                    if (instance == null)
                    {
                        instance = CreateInstance();
                    }
                }

                return instance;
            }
        }

        [SerializeField] private string _assetBundleMoveDir;
        [SerializeField] private List<BuiltInFileInfo> AOS_BuiltInList;
        [SerializeField] private List<BuiltInFileInfo> IOS_BuiltInList;
        [SerializeField, ReadOnlyProperty] private List<string> _builtInFileList;

        public bool CheckBuiltInFile(string resourceName)
        {
            if (_builtInFileListEditor == null || _builtInFileListEditor.Count <= 0)
            {
                RefreshBuiltInFileListForEditor();
            }

            return _builtInFileListEditor.Any(file => file.Equals(resourceName));
        }

        private List<string> _builtInFileListEditor;

        public static BuiltInFileList CreateInstance()
        {
            if (instance == null)
            {
                instance = CreateInstance<BuiltInFileList>();

                AssetDatabase.CreateAsset(instance, $"Assets/Resources/{BUILT_IN_FILE_ASSET_NAME}.asset");
            }

            return instance;
        }

        [MenuItem("ANIPANG5/AssetBundle/Built In Data/Move Built In Files")]
        public static void MoveFile()
        {
#if UNITY_IOS
        Instance.MoveBuiltInFiles(BuildTarget.iOS);
#else
            Instance.MoveBuiltInFiles(BuildTarget.Android);
#endif
        }

        [MenuItem("ANIPANG5/AssetBundle/Built In Data/Revert Move Built In Files")]
        public static void RevertFile()
        {
#if UNITY_IOS
        Instance.RevertMoveFiles(BuildTarget.iOS);
#else
            Instance.RevertMoveFiles(BuildTarget.Android);
#endif
        }

        public void MoveBuiltInFiles(BuildTarget buildTarget, bool isBuildAB = false)
        {
            var rootPath = $"{ASSET_ROOT_PATH}/{Instance._assetBundleMoveDir}";
            var assetMoveMap = new Dictionary<string, string>();

            foreach (var info in buildTarget != BuildTarget.iOS ? Instance.AOS_BuiltInList : Instance.IOS_BuiltInList)
            {
                if (info == null || info.AssetPath.IsNullOrEmpty())
                {
                    Debug.LogWarning(" Built In INFO Data Is inCompleted");
                    continue;
                }

                // TODO : 시간날떄 fonts를 제외하는게 아니라, 번들 빌드에 필요한걸 제외하도록 수정해야함.
                if (isBuildAB && info.AssetPath.ToLower() == "fonts")
                {
                    continue;
                }

                var oldPath = $"{ASSET_ROOT_PATH}/{info.AssetPath}";
                var newPath = $"{rootPath}/{info.AssetPath}";

                // FilePath 없으면 생성
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                // fileNamseList 에 있는 Directory or File 들 옮기기
                if ((info.AssetNameList == null || info.AssetNameList.Count <= 0) && info.Type == BuiltInFileInfo.EAssetType.ROOT)
                {
                    if (!Directory.Exists(oldPath))
                    {
                        continue;
                    }

                    if (Directory.Exists(newPath))
                    {
                        Directory.Delete(newPath, true);
                    }

                    assetMoveMap.Add(oldPath, newPath);
                }
                else
                {
                    foreach (var assetName in info.AssetNameList)
                    {
                        // 옮길 directory 가 이미 존재 한다면.. 
                        // 지웠다가 다시 옮김.
                        if (Directory.Exists($"{newPath}/{assetName}"))
                        {
                            Directory.Delete($"{newPath}/{assetName}", true);
                        }

                        if (info.Type == BuiltInFileInfo.EAssetType.DIRECTORY)
                        {
                            if (!Directory.Exists($"{oldPath}/{assetName}"))
                            {
                                continue;
                            }

                            assetMoveMap.Add($"{oldPath}/{assetName}", $"{newPath}/{assetName}");
                        }
                        else if (info.Type == BuiltInFileInfo.EAssetType.FILE)
                        {
                            if (!Directory.Exists(oldPath))
                            {
                                continue;
                            }

                            foreach (var file in Directory.GetFiles(oldPath).Where(o => !o.Contains(".meta")))
                            {
                                var fileName = file.Replace("\\", "/").Split('/').Last();
                                if (fileName.Split('.')[0].Equals(assetName))
                                {
                                    // 기존 파일이 없다면 넘어감..?
                                    if (!File.Exists($"{oldPath}/{fileName}"))
                                    {
                                        continue;
                                    }

                                    // 옮길 위치에 이미 동일한 파일이 있다면 넘어감.
                                    if (File.Exists($"{newPath}/{fileName}"))
                                    {
                                        continue;
                                    }

                                    assetMoveMap.Add($"{oldPath}/{fileName}", $"{newPath}/{fileName}");
                                }
                            }
                        }
                    }
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.StartAssetEditing();

            foreach (var pair in assetMoveMap)
            {
                AssetDatabase.MoveAsset(pair.Key, pair.Value);
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        public void RevertMoveFiles(BuildTarget buildTarget)
        {
            var rootPath = $"{ASSET_ROOT_PATH}/{Instance._assetBundleMoveDir}";
            var assetMoveMap = new Dictionary<string, string>();

            foreach (var info in buildTarget != BuildTarget.iOS ? Instance.AOS_BuiltInList : Instance.IOS_BuiltInList)
            {
                if (info == null || info.AssetPath.IsNullOrEmpty())
                {
                    Debug.LogWarning(" Built In INFO Data Is inCompleted");
                    continue;
                }

                if (!Directory.Exists(rootPath))
                {
                    break;
                }

                var oldPath = $"{rootPath}/{info.AssetPath}";
                var newPath = $"{ASSET_ROOT_PATH}/{info.AssetPath}";

                if ((info.AssetNameList == null || info.AssetNameList.Count <= 0) && info.Type == BuiltInFileInfo.EAssetType.ROOT)
                {
                    if (!Directory.Exists(oldPath))
                    {
                        continue;
                    }

                    assetMoveMap.Add(oldPath, newPath);
                }
                else
                {
                    if (info.AssetNameList == null)
                    {
                        return;
                    }

                    foreach (var assetName in info.AssetNameList)
                    {
                        if (info.Type == BuiltInFileInfo.EAssetType.DIRECTORY)
                        {
                            if (!Directory.Exists($"{oldPath}/{assetName}"))
                            {
                                continue;
                            }

                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }

                            assetMoveMap.Add($"{oldPath}/{assetName}", $"{newPath}/{assetName}");
                        }
                        else if (info.Type == BuiltInFileInfo.EAssetType.FILE)
                        {
                            if (!Directory.Exists(oldPath))
                            {
                                continue;
                            }

                            foreach (var file in Directory.GetFiles(oldPath).Where(o => !o.Contains(".meta")))
                            {
                                var fileName = file.Replace("\\", "/").Split('/').Last();

                                // 기존 파일이 없다면 넘어감..?
                                if (!File.Exists($"{oldPath}/{fileName}"))
                                {
                                    continue;
                                }

                                // 옮길 위치에 이미 동일한 파일이 있다면 넘어감.
                                if (File.Exists($"{newPath}/{fileName}"))
                                {
                                    continue;
                                }

                                if (!Directory.Exists(newPath))
                                {
                                    Directory.CreateDirectory(newPath);
                                }

                                AssetDatabase.MoveAsset($"{oldPath}/{fileName}", $"{newPath}/{fileName}");
                            }
                        }
                    }
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.StartAssetEditing();

            foreach (var pair in assetMoveMap)
            {
                AssetDatabase.MoveAsset(pair.Key, pair.Value);
            }

            if (Directory.Exists(rootPath))
            {
                foreach (var directory in Directory.GetDirectories(rootPath))
                {
                    AssetDatabase.DeleteAsset(directory);
                }
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        [MenuItem("ANIPANG5/AssetBundle/Built In Data/Move Trash AssetBundles")]
        public static void MoveTrashes()
        {
            Instance.MoveTrashAssetBundles();
        }

        [MenuItem("ANIPANG5/AssetBundle/Built In Data/Revert Move Trash AssetBundles")]
        public static void RevertMoveTrashes()
        {
            Instance.RevertMoveTrashAssetBundles();
        }

        public void MoveTrashAssetBundles()
        {
            if (_builtInFileList.IsNullOrEmpty())
            {
                var buildTarget = BuildTarget.Android;
#if UNITY_IOS
            buildTarget = BuildTarget.iOS;
#endif
                RefreshBuiltInFileList(buildTarget);
            }

            if (Directory.Exists(ASSET_TRASH_PATH))
            {
                Directory.Delete(ASSET_TRASH_PATH, true);
            }

            var assetMoveMap = new Dictionary<string, string>();
            foreach (var assetName in AssetDatabase.GetAllAssetBundleNames())
            {
                Debug.Log($"label : {assetName}");

                // 폰트 폴더는 제거되지 않도록.
                if (assetName.Contains("fonts"))
                {
                    continue;
                }

                var assets = AssetDatabase.FindAssets($"{assetName} t:folder", new[] { ASSET_ROOT_PATH });
                if (assets == null || assets.Length <= 0)
                {
                    continue;
                }

                foreach (var asset in assets)
                {
                    var dir = AssetDatabase.GUIDToAssetPath(asset);
                    var filePaths = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);

                    foreach (var filePath in filePaths)
                    {
                        if (filePath.Contains(".meta"))
                        {
                            continue;
                        }

                        var replacedFilePath = filePath.Replace($"{ASSET_ROOT_PATH}/", "");
                        if (!_builtInFileList.Contains(replacedFilePath))
                        {
                            var fileName = replacedFilePath.Split('/').Last();
                            var parentPath = $"{ASSET_TRASH_PATH}/{replacedFilePath}".Replace(fileName, "");
                            if (!Directory.Exists(parentPath))
                            {
                                Directory.CreateDirectory(parentPath);
                            }

                            assetMoveMap.TryAdd(filePath, $"{ASSET_TRASH_PATH}/{replacedFilePath}");
                        }
                    }
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.StartAssetEditing();

            foreach (var pair in assetMoveMap)
            {
                AssetDatabase.MoveAsset(pair.Key, pair.Value);
            }

            RemoveEmptyDirectory(ASSET_ROOT_PATH);

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        public void RevertMoveTrashAssetBundles()
        {
            var assetMoveMap = new Dictionary<string, string>();
            foreach (var assetName in AssetDatabase.GetAllAssetBundleNames())
            {
                Debug.Log($"label : {assetName}");

                // 폰트 폴더는 제거되지 않도록.
                if (assetName.Contains("fonts"))
                {
                    continue;
                }

                var assets = AssetDatabase.FindAssets($"{assetName} t:folder", new[] { ASSET_TRASH_PATH });
                if (assets == null || assets.Length <= 0)
                {
                    continue;
                }

                foreach (var asset in assets)
                {
                    var dir = AssetDatabase.GUIDToAssetPath(asset);
                    var filePaths = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);

                    foreach (var filePath in filePaths)
                    {
                        if (filePath.Contains(".meta"))
                        {
                            continue;
                        }

                        var replacedFilePath = filePath.Replace($"{ASSET_TRASH_PATH}/", "");
                        var fileName = replacedFilePath.Split('/').Last();
                        var parentPath = $"{ASSET_ROOT_PATH}/{replacedFilePath}".Replace(fileName, "");
                        if (!Directory.Exists(parentPath))
                        {
                            Directory.CreateDirectory(parentPath);
                        }

                        assetMoveMap.TryAdd(filePath, $"{ASSET_ROOT_PATH}/{replacedFilePath}");
                    }
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.StartAssetEditing();

            foreach (var pair in assetMoveMap)
            {
                AssetDatabase.MoveAsset(pair.Key, pair.Value);
            }

            if (Directory.Exists(ASSET_TRASH_PATH))
            {
                Directory.Delete(ASSET_TRASH_PATH, true);
            }

            _builtInFileList.Clear();

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private void RemoveEmptyDirectory(string start)
        {
            var dirs = Directory.GetDirectories(start);
            foreach (var directory in dirs)
            {
                RemoveEmptyDirectory(directory);
                var files = Directory.GetFiles(directory);
                var subDirs = Directory.GetDirectories(directory);

                if (files.All(o => o.Contains(".meta")) && subDirs.Length == 0)
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        #region RERESH BUILT IN DATA

        public void RefreshBuiltInFileList(BuildTarget buildTarget)
        {
            _builtInFileList.Clear();

            var saveList = buildTarget != BuildTarget.iOS ? Instance.AOS_BuiltInList : Instance.IOS_BuiltInList;
            foreach (var info in saveList)
            {
                switch (info.Type)
                {
                    case BuiltInFileInfo.EAssetType.DIRECTORY: _builtInFileList.AddRange(GetDirectoryFileNamesToList(info)); break;
                    case BuiltInFileInfo.EAssetType.ROOT: _builtInFileList.AddRange(GetRootFileNameToList(info)); break;
                    default: _builtInFileList.AddRange(GetFileNamesToList(info)); break;
                }
            }

            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private void RefreshBuiltInFileListForEditor()
        {
            _builtInFileListEditor = new List<string>();

            var buildTarget = BuildTarget.Android;
#if UNITY_IOS
        buildTarget = BuildTarget.iOS;
#endif

            var saveList = buildTarget != BuildTarget.iOS ? Instance.AOS_BuiltInList : Instance.IOS_BuiltInList;
            foreach (var info in saveList)
            {
                switch (info.Type)
                {
                    case BuiltInFileInfo.EAssetType.DIRECTORY: _builtInFileListEditor.AddRange(GetDirectoryFileNamesToList(info)); break;
                    case BuiltInFileInfo.EAssetType.ROOT: _builtInFileListEditor.AddRange(GetRootFileNameToList(info)); break;
                    default: _builtInFileListEditor.AddRange(GetFileNamesToList(info)); break;
                }
            }
        }

        private List<string> GetDirectoryFileNamesToList(BuiltInFileInfo info)
        {
            var result = new List<string>();

            if (info == null || info.AssetNameList.IsNullOrEmpty() || info.AssetPath.IsNullOrEmpty())
            {
                return result;
            }

            string rootpath = $"{ASSET_ROOT_PATH}/{info.AssetPath}";
            if (!Directory.Exists(rootpath))
            {
                return result;
            }

            // 번들 빌드 후 FilePath Directory 의 하위에 fileName 으로 된 directory 가 있는지 체크
            foreach (var assetName in info.AssetNameList.Where(assetName => Directory.Exists($"{rootpath}/{assetName}")))
            {
                result.AddRange(Directory.GetFiles($"{rootpath}/{assetName}", "*", SearchOption.AllDirectories)
                                         .Where(path => !path.Contains(".meta"))
                                         .Select(path => path.Replace($"{ASSET_ROOT_PATH}/", "").Replace("\\", "/")));
            }

            return result;
        }

        private List<string> GetRootFileNameToList(BuiltInFileInfo info)
        {
            var result = new List<string>();
            if (info == null || info.AssetPath.IsNullOrEmpty())
            {
                return result;
            }

            string rootpath = $"{ASSET_ROOT_PATH}/{info.AssetPath}";
            if (!Directory.Exists(rootpath))
            {
                return result;
            }

            result.AddRange(Directory.GetFiles(rootpath, "*", SearchOption.AllDirectories)
                                     .Where(path => !path.Contains(".meta"))
                                     .Select(path => path.Replace($"{ASSET_ROOT_PATH}/", "").Replace("\\", "/")));

            return result;
        }

        private List<string> GetFileNamesToList(BuiltInFileInfo info)
        {
            var result = new List<string>();
            if (info == null || info.AssetNameList.IsNullOrEmpty() || info.AssetPath.IsNullOrEmpty())
            {
                return result;
            }

            string rootPath = $"{ASSET_ROOT_PATH}/{info.AssetPath}";
            if (!Directory.Exists(rootPath))
            {
                return result;
            }

            // 번들 빌드 후 해당 directory 밑에 FileNameList 와 동일 한 파일이 있는지 체크하여 있으면 result 에 추가
            foreach (string assetName in info.AssetNameList)
            {
                result.AddRange(Directory.GetFiles(rootPath)
                                         .Where(path => !path.Contains(".meta") && path.Contains(assetName))
                                         .Select(path => path.Replace($"{ASSET_ROOT_PATH}/", "").Replace("\\", "/")));
            }

            return result;
        }

        #endregion

    }

    [Serializable]
    public class BuiltInFileInfo
    {
        public enum EAssetType
        {
            DIRECTORY
          , FILE
          , ROOT
        }

        public string AssetPath;
        public EAssetType Type;
        public List<string> AssetNameList;
    }
}
#endif