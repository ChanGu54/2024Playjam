#if UNITY_EDITOR
namespace AssetBundleBrowser
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;
    using System;
    using UnityEngine.U2D;
    using UnityEditor.U2D;
    using UnityEditor.AnimatedValues;

    public class STZAssetBuilderTab
    {
        private AssetBundleBrowserMain _parent;

        [Serializable]
        public class AssetBundleData
        {
            public int version { get; set; }

            public string name { get; set; }
            public string variant { get; set; }
            public bool check { get; set; }

            public string path { get; set; }

            public AnimBool animBool { get; set; }



            public AssetBundleData()
            {
                name = variant = string.Empty;
                animBool = new AnimBool(false);
            }

            /// <summary>
            /// costumes/cos01/16/cos01 일 경우 마지막 파일명인 cos01 만 리턴
            /// </summary>
            /// <returns></returns>
            public string GetFileName()
            {
                string[] vs = name.Split('/');

                return vs[vs.Length - 1];
            }


            public override string ToString()
            {
                return string.Format("{0}", name, variant);
            }
        }

        public Dictionary<string, int> _assetBundleDataIdxDict = new Dictionary<string, int>();
        public List<AssetBundleData> _assetBundleDataList = new List<AssetBundleData>();
        Vector2 scrollAdd = Vector2.one;
        List<SpriteAtlas> _spriteAtlasList = new List<SpriteAtlas>();

        /// <summary>
        /// 어셋 번들용 피씨 인가? (Assets/LocalData/buildaqua.txt 존재 여부)
        /// </summary>
        private bool _isBuildPopPc = false;
        private int _etcTextureCompressorBehavior;
        private int _etcTextureFastCompressor;
        private int _etcTextureNormalCompressor;
        private int _etcTextureBestCompressor;

        public AssetBundleBuild[] BuildList { get; set; }

#if UINTY_STANDALONE
    private int _selectToolBarIdx = 0;
#elif UNITY_ANDROID
        private int _selectToolBarIdx = 1;
#elif UNITY_IOS
    private int _selectToolBarIdx = 2;
#elif UNITY_OSX
    private int _selectToolBarIdx = 3;
#else
    private int _selectToolBarIdx = 0;
#endif

        private string[] _toolbarString = { "Window", "Android", "IOS", "OSX","WEBGL","All" };
        private string[] _targetPathArr = { "win", "aos", "ios", "osx", "webgl" };
        private BuildTarget[] _buildTargetArr = { BuildTarget.StandaloneWindows64, BuildTarget.Android, BuildTarget.iOS, BuildTarget.StandaloneOSX, BuildTarget.WebGL };

        string GetFilePath(int selectIdx)
        {
            return string.Format("{0}/../AssetBundles/{1}", Application.dataPath, _targetPathArr[selectIdx]);
        }

        string GetServerURL(bool isLive)
        {
            return "https://storage.googleapis.com/anipang5-resources";
        }

        private enum VedioType
        {
            intro,
            media
        }

        /// <summary>
        /// 빌드 중이면 true
        /// </summary>
        private bool _isBuild = false;

        /// <summary>
        /// CRC 체크 중이면 true
        /// </summary>
        private bool _isCRC = false;

        /// <summary>
        /// 에셋 번들을 적용시킨다
        /// </summary>
        private void ApplyAssetBundles(int selectIdx, bool isLive)
        {
            if (_isBuild == true)
            {
                return;
            }

            _isBuild = true;

            // 플랫폼 설정
            string targetPath = _targetPathArr[selectIdx];
            BuildTarget buildTarget = _buildTargetArr[selectIdx];

            //번들용 피씨에서만 ETC압축을 Best로 정상 적용시키도록 설정(Assets/LocalData/buildaqua.txt 존재 여부)
            //_isBuildPopPc = File.Exists("Assets/LocalData/buildaqua.txt");
            //if (_isBuildPopPc)
            //{
            //    // etc압축 설정을 빌드용으로 세팅하기 전, 기존 설정 백업
            //    _etcTextureCompressorBehavior = EditorSettings.etcTextureCompressorBehavior;
            //    _etcTextureFastCompressor = EditorSettings.etcTextureFastCompressor;
            //    _etcTextureNormalCompressor = EditorSettings.etcTextureNormalCompressor;
            //    _etcTextureBestCompressor = EditorSettings.etcTextureBestCompressor;

            //    //etcTextureCompressor 변경 - 정의 되어있는 값이 없어서 int 적용
            //    EditorSettings.etcTextureCompressorBehavior = 0; //Legacy
            //    EditorSettings.etcTextureBestCompressor = 2; //ETCPACK Best
            //}

            try
            {
                string path = GetFilePath(selectIdx);

                if (Directory.Exists(path) == true)
                {
                    Directory.Delete(path, true);
                }

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                _spriteAtlasList = new List<SpriteAtlas>();

                // 선택 항목만 에셋 번들 필드
                List<AssetBundleBuild> compressedApplyList = new List<AssetBundleBuild>();
                List<AssetBundleBuild> uncompressedApplyList = new List<AssetBundleBuild>();
                if (BuildList == null)
                {
                    Debug.LogError("Build List Null !!");
                }
                for (int i = 0; i < BuildList.Length; i++)
                {
                    if (BundleBuildList.Instance.dic_BundleInfo.TryGetValue(_assetBundleDataList[i].name, out var info))
                    {
                        BuildList[i].assetBundleName = bundlePath(info).IsNullOrEmpty() ? BuildList[i].assetBundleName : bundlePath(info);
                    }

                    if (_assetBundleDataList[i].check)
                    {
                        if (AssetConfigData.Instance.assetConfigDict.TryGetValue(_assetBundleDataList[i].name, out var assetbundle))
                        {
                            if (assetbundle.isCompressed)
                            {
                                compressedApplyList.Add(BuildList[i]);
                            }
                            else
                            {
                                uncompressedApplyList.Add(BuildList[i]);
                            }
                        }
                        else
                        {
                            compressedApplyList.Add(BuildList[i]);
                        }

                        // NOTE @Hyunmo.goo 번들에 포함시킬 SpriteAtlas 목록 수집
                        foreach (string assetPath in BuildList[i].assetNames)
                        {
                            if (assetPath.LastIndexOf(".spriteatlas") >= 0)
                            {
                                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
                                if (spriteAtlas != null)
                                {
                                    _spriteAtlasList.Add(spriteAtlas);
                                }
                            }
                        }

                        var tempPath = string.Format("{0}/{1}", path, BuildList[i].assetBundleName);
                        FileInfo fileInfo = new FileInfo(tempPath);

                        if (Directory.Exists(fileInfo.Directory.FullName))
                        {
                            var files = fileInfo.Directory.GetFiles(string.Format("{0}.*", fileInfo.Name.Replace(string.Format(".{0}", fileInfo.Extension), string.Empty)));
                            foreach (var file in files)
                            {
                                if(File.Exists(file.FullName))
                                {
                                    File.Delete(file.FullName);
                                }
                            }
                        }
                    }
                }

                BuildAssetBundles(path, compressedApplyList, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
                BuildAssetBundles(path, uncompressedApplyList, BuildAssetBundleOptions.UncompressedAssetBundle, buildTarget);

                // 빌드 후 CRC 체크를 위한 리스트 만들기
                {
                    _currentApplyListCount = 0;
                    _totalApplyListCount = 0;
                    _currentIndex = 0;
                    _CRCCheckerList.Clear();
                    _addUrlList.Clear();

                    _totalApplyListCount = compressedApplyList.Count + uncompressedApplyList.Count;

                    foreach (var value in compressedApplyList)
                    {
                        _currentApplyListCount++;

                        string assetBundleName = "";
                        if (BundleBuildList.Instance.dic_BundleInfo.TryGetValue(value.assetBundleName, out BundleInfo bundleInfo))
                        {
                            if (bundleInfo != null)
                            {
                                assetBundleName = bundlePath(bundleInfo).IsNullOrEmpty() ? value.assetBundleName : bundlePath(bundleInfo);
                            }
                        }

                        for (int i = 0; i < BuildList.Length; i++)
                        {
                            if (BuildList[i].assetBundleName == assetBundleName)
                            {
                                var tempPath = string.Format("{0}/{1}.manifest", path, bundleInfo.fileName);
                                FileInfo fileInfo = new FileInfo(tempPath);

                                if (Directory.Exists(fileInfo.Directory.FullName)
                                     && System.IO.File.Exists(tempPath))
                                {
                                    StreamReader reader = new StreamReader(tempPath);
                                    var tempCRC = CRCChecker.GetCRC(reader);
                                    reader.Dispose();

                                    _CRCCheckerList.Add(new CRCChecker() { CDNUrl = string.Format("{0}/{1}/{2}.{3}.manifest", GetServerURL(false), _targetPathArr[selectIdx], value.assetBundleName, value.assetBundleVariant), CDNKey = value.assetBundleName, CRC = tempCRC });
                                }
                                break;
                            }
                        }

                        IsLastCRCCheckerList();
                    }
                    foreach (var value in uncompressedApplyList)
                    {
                        _currentApplyListCount++;

                        var tempPath = string.Format("{0}/{1}.manifest", path, value.assetBundleName, value.assetBundleVariant);
                        StreamReader reader = new StreamReader(tempPath);
                        var tempCRC = CRCChecker.GetCRC(reader);
                        reader.Dispose();

                        _CRCCheckerList.Add(new CRCChecker() { CDNUrl = string.Format("{0}/{1}/{2}.{3}.manifest", GetServerURL(false), _targetPathArr[selectIdx], value.assetBundleName, value.assetBundleVariant), CDNKey = value.assetBundleName, CRC = tempCRC });

                        IsLastCRCCheckerList();
                    }
                }

                // 해당 빌드 폴더 열리는 코드
                System.Diagnostics.Process.Start(path);

                _isBuild = false;
            }
            catch (Exception e)
            {
                if (_isBuildPopPc)
                {
                    // etc압축 설정을 원복
                    EditorSettings.etcTextureCompressorBehavior = _etcTextureCompressorBehavior;
                    EditorSettings.etcTextureFastCompressor = _etcTextureFastCompressor;
                    EditorSettings.etcTextureNormalCompressor = _etcTextureNormalCompressor;
                    EditorSettings.etcTextureBestCompressor = _etcTextureBestCompressor;
                }

                EditorUtility.DisplayDialog("에셋 번들 빌드 실패...",
                    string.Format("[{0}] {1}", targetPath, System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")), "닫기");

                Debug.LogError(e.StackTrace);
                //Close();

                _isBuild = false;
            }
        }

        private int _currentApplyListCount = 0;
        private int _totalApplyListCount = 0;

        /// <summary>
        /// 마지막인지 체크 후 마지막이라면 다음으로 진행
        /// </summary>
        private void IsLastCRCCheckerList()
        {
            if (_currentApplyListCount >= _totalApplyListCount)
            {
                UdateCRCChecker();

                // 빌드 끝나고 CRC 체크로 넘어가려면 false 로 바꿔줘야 함
                _isBuild = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="inBuildList"></param>
        /// <param name="inOption"></param>
        /// <param name="inTarget"></param>
        private void BuildAssetBundles(string inPath, List<AssetBundleBuild> inBuildList, BuildAssetBundleOptions inOption, BuildTarget inTarget)
        {
            // 빌트인 에셋 이동 후 데이터 갱신
            for (int i = 0; i < inBuildList.Count; ++i)
            {
                var prevBundleBuild = inBuildList[i];
                var assetBundleData = _assetBundleDataList.Find(o => o.path == prevBundleBuild.assetBundleName);
                if(assetBundleData == null)
                {
                    continue;
                }

                var bundleName = assetBundleData.name;
                var assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

                // 기존: assetBundleName: 1/fonts
                //inBuildList[i] = new AssetBundleBuild { assetBundleName = prevBundleBuild.assetBundleName, assetNames = assetNames };

                // 변경: assetBundleName: fonts
                inBuildList[i] = new AssetBundleBuild { assetBundleName = bundleName, assetNames = assetNames };
            }

            BuildPipeline.BuildAssetBundles(inPath, inBuildList.ToArray(), inOption, inTarget);
        }

        internal void OnEnable(EditorWindow parent)
        {
            _parent = (parent as AssetBundleBrowserMain);

        }
        bool _init = false;
        public void OnGUI()
        {
            if (_isBuild == true)
            {
                return;
            }

            if (EditorApplication.isCompiling || _CRCCheckerList.Count > 0)
            {
                GUILayout.TextArea("\n\n내용 반영 중 입니다.", GUILayout.Height(100));
                return;
            }

            if (!_init || BuildList == null)
            {
                _init = true;
                Reset();
            }

            int itemHeight = 22;
            int itemButtonWidth = 80;

            // 번들 빌드 표시에서 제외할 리스트
            List<string> excludedList = new List<string>
        {
            "dynamic_font"
        };

            // 폰트 파일은 다르게 보여줘야 해서 추가
            List<string> fontFileList = new List<string>
        {
            "font_common"
        };

            // 에셋 번들 리스트
            _selectToolBarIdx = GUILayout.Toolbar(_selectToolBarIdx, _toolbarString);
            scrollAdd = EditorGUILayout.BeginScrollView(scrollAdd);
            if (_assetBundleDataList != null)
                for (int i = 0; i < _assetBundleDataList.Count; i++)
                {
                    // bundle target 별로 리스트에 보여줄지 체크

                    var dic_bundleinfo = BundleBuildList.Instance.dic_BundleInfo;

                    if (dic_bundleinfo == null)
                        continue;

                    AssetBundleData targetAssetBundleData = _assetBundleDataList[i];
                    string fileName = targetAssetBundleData.GetFileName();

                    // 표시에서 제외함
                    if (excludedList.IndexOf(fileName) != -1)
                    {
                        continue;
                    }

                    EditorGUILayout.BeginHorizontal();

                    bool isFont = (fontFileList.IndexOf(fileName) != -1);

                    // 해당 번들 선택
                    bool oldCheckValue = targetAssetBundleData.check;

                    if (isFont == false)
                    {
                        targetAssetBundleData.check = EditorGUILayout.ToggleLeft(fileName, targetAssetBundleData.check, GUILayout.Height(itemHeight));
                    }
                    else
                    {
                        GUIStyle fontFileStyle = new GUIStyle(GUI.skin.label);
                        fontFileStyle.normal.textColor = Color.white;
                        targetAssetBundleData.check = EditorGUILayout.ToggleLeft(fileName, targetAssetBundleData.check, fontFileStyle, GUILayout.Height(itemHeight));
                    }
                    if (oldCheckValue != targetAssetBundleData.check)
                    {
                        if (targetAssetBundleData.check)
                        {
                            if (AssetConfigData.Instance.assetConfigDict.TryGetValue(targetAssetBundleData.name, out var configValue))
                            {
                                foreach (var dependItem in configValue.dependList)
                                {
                                    int dependItemIdx = _assetBundleDataIdxDict[dependItem];
                                    _assetBundleDataList[dependItemIdx].check = true;
                                }
                            }
                        }
                        else
                        {
                            foreach (var keyValuePair in AssetConfigData.Instance.assetConfigDict)
                            {
                                if (keyValuePair.Value.dependList != null && keyValuePair.Value.dependList.Count > 0)
                                {
                                    if(_assetBundleDataIdxDict.ContainsKey(keyValuePair.Key) == true)
                                    {
                                        var parent = _assetBundleDataList[_assetBundleDataIdxDict[keyValuePair.Key]];
                                        bool isNeedToUncheckParent = (keyValuePair.Value.dependList.Contains(targetAssetBundleData.name) && parent.check == true);
                                        if (isNeedToUncheckParent)
                                        {
                                            bool answer = EditorUtility.DisplayDialog("주의", targetAssetBundleData.name + "를 해제하면 " + parent.name + "도 같이 해제됩니다. 원하지 않으시면 아니오를 눌러주세요.", "예", "아니오");
                                            if (answer)
                                            {
                                                parent.check = false;
                                            }
                                            else
                                            {
                                                targetAssetBundleData.check = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 현재 버전
                    GUIStyle fileVersionStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 11
                    };


                    if (dic_bundleinfo.ContainsKey(targetAssetBundleData.name) == true)
                    {
                        BundleInfo bundleinfo = dic_bundleinfo[targetAssetBundleData.name];
                        EditorGUILayout.TextField($"{bundleinfo.version}", fileVersionStyle);
                        targetAssetBundleData.path = bundlePath(bundleinfo);
                    }
                    else
                    {
                        EditorGUILayout.TextField($"{targetAssetBundleData.version}", fileVersionStyle);
                    }

                    // 버전 올리기 버튼
                    if (GUILayout.Button("버전 올리기", GUILayout.Width(itemButtonWidth), GUILayout.Height(itemHeight)))
                    {
                        BundleBuildList.Instance.AddBundle((BundleInfo.E_BUILD_TARGET)_selectToolBarIdx, targetAssetBundleData.name);

                        _init = false;
                    }

                    // 상세보기 그룹
                    targetAssetBundleData.animBool.target = EditorGUILayout.ToggleLeft("상세 보기", targetAssetBundleData.animBool.target, GUILayout.Height(itemHeight));
                    if (EditorGUILayout.BeginFadeGroup(targetAssetBundleData.animBool.faded))
                    {
                        EditorGUILayout.TextField(targetAssetBundleData.path);

                        // 버전 내리기 버튼
                        if (GUILayout.Button("버전 내리기", GUILayout.Width(itemButtonWidth), GUILayout.Height(itemHeight)))
                        {
                            BundleBuildList.Instance.RemoveBundle((BundleInfo.E_BUILD_TARGET)_selectToolBarIdx, targetAssetBundleData.name);

                            _init = false;
                        }

                        // 파일 유무
                        bool fileExist = _selectToolBarIdx < _targetPathArr.Length
                            && System.IO.File.Exists(string.Format("{0}/{1}", GetFilePath(_selectToolBarIdx), targetAssetBundleData.ToString()));

                        string strExist = fileExist ? "로컬에 파일 있음" : "로컬에 파일 없음";

                        GUIStyle fileExistStyle = new GUIStyle(GUI.skin.button);
                        fileExistStyle.normal.textColor = fileExist ? Color.white : Color.gray;
                        EditorGUILayout.LabelField(strExist, fileExistStyle);
                    }
                    EditorGUILayout.EndFadeGroup();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    AssetBundleBrowserMain.DrawUILine();
                    EditorGUILayout.EndHorizontal();
                }

            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("All UnSelect"))
            {
                for (int i = 0; i < _assetBundleDataList.Count; i++)
                {
                    _assetBundleDataList[i].check = false;
                }
            }
            if (GUILayout.Button("All Select"))
            {
                for (int i = 0; i < _assetBundleDataList.Count; i++)
                {
                    _assetBundleDataList[i].check = true;
                }
            }
            if (GUILayout.Button("All Version Up"))
            {
                for (int i = 0; i < _assetBundleDataList.Count; i++)
                {
                    AssetBundleData targetAssetBundleData = _assetBundleDataList[i];
                    BundleBuildList.Instance.AddBundle((BundleInfo.E_BUILD_TARGET)_selectToolBarIdx, targetAssetBundleData.name);
                    _init = false;
                }
            }
            GUILayout.EndHorizontal();

            var selectedStyle = new GUIStyle(GUI.skin.button);
            selectedStyle.normal.textColor = Color.cyan;

            CheckUploadImmediately(selectedStyle);
            CheckTargetRegion(selectedStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", GUILayout.Height(100)))
            if (_selectToolBarIdx != 5) ApplyAssetBundles(_selectToolBarIdx, false);
            else
            {
                for (int i = 0; i < 5; i++)
                    ApplyAssetBundles(i, false);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string bundlePath(BundleInfo info)
        {
            string result = string.Empty;

            if (info != null)
                result = $"{info.version}/{info.fileName}";

            return result;
        }

        private bool _uploadImmediately = false;
#if TARGET_GLOBAL
    private AssetBundleUploadHelper.REGION _targetRegion = AssetBundleUploadHelper.REGION.GLOBAL;
#elif TARGET_KOREA
    private AssetBundleUploadHelper.REGION _targetRegion = AssetBundleUploadHelper.REGION.KAKAO;
#else
        private AssetBundleUploadHelper.REGION _targetRegion = AssetBundleUploadHelper.REGION.JP;
#endif

        private void CheckUploadImmediately(GUIStyle selectedStyle)
        {
            GUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("업로드 여부", new GUIStyle() { alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState() { textColor = Color.white } });
                GUILayout.BeginHorizontal(GUILayout.Height(50));
                {
                    if (GUILayout.Button("Ok!", _uploadImmediately ? selectedStyle : new GUIStyle(GUI.skin.button), GUILayout.Height(50)))
                    {
                        _uploadImmediately = true;
                    }
                    if (GUILayout.Button("No!", !_uploadImmediately ? selectedStyle : new GUIStyle(GUI.skin.button), GUILayout.Height(50)))
                    {
                        _uploadImmediately = false;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void CheckTargetRegion(GUIStyle selectedStyle)
        {
            GUILayout.BeginVertical(GUILayout.Height(50));
            {
                if (_uploadImmediately)
                {
                    EditorGUILayout.LabelField("타겟 서버", new GUIStyle() { alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState() { textColor = Color.white } });
                    GUILayout.BeginHorizontal(GUILayout.Height(50));
                    {
                        if (GUILayout.Button("JP", _targetRegion == AssetBundleUploadHelper.REGION.JP ? selectedStyle : new GUIStyle(GUI.skin.button), GUILayout.Height(50)))
                        {
                            _targetRegion = AssetBundleUploadHelper.REGION.JP;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        public string ListToString(List<string> list)
        {
            string result = string.Empty;

            for (int i = 0; i < list.Count - 1; i++)
            {
                result += list[i] + "/";
            }
            result += list[list.Count - 1];

            return result;
        }

        public void Reset()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            _assetBundleDataList.Clear();
            _assetBundleDataIdxDict.Clear();

            BuildTarget buildTarget = _buildTargetArr.Length > _selectToolBarIdx ? _buildTargetArr[_selectToolBarIdx] : BuildTarget.NoTarget;

            BundleInfo.E_BUILD_TARGET target = BundleInfo.E_BUILD_TARGET.ALL;

            //switch (buildTarget)
            //{
            //    case BuildTarget.StandaloneWindows:
            //    case BuildTarget.StandaloneWindows64:
            //        target = BundleInfo.E_BUILD_TARGET.WIN;
            //        break;
            //    case BuildTarget.iOS:
            //        target = BundleInfo.E_BUILD_TARGET.IOS;
            //        break;
            //    case BuildTarget.Android:
            //        target = BundleInfo.E_BUILD_TARGET.AOS;
            //        break;
            //    case BuildTarget.StandaloneOSX:
            //        target = BundleInfo.E_BUILD_TARGET.OSX;
            //        break;

            //}
            BundleBuildList.Instance.SetDicBundleInfo(target);

            // 에셋 번들 목록을 가져옴
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            BuildList = new AssetBundleBuild[assetBundleNames.Length];
            for (int i = 0; i < BuildList.Length; i++)
            {
                string[] nameArr = assetBundleNames[i].Split('.');
                BuildList[i].assetBundleName = nameArr[0];
                if (nameArr.Length > 1)
                    BuildList[i].assetBundleVariant = nameArr[1];
                BuildList[i].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);

                // 레이아웃에 리스트 목록을 추가
                AssetBundleData data = new AssetBundleData();

                data.name = BuildList[i].assetBundleName;

                int currentVersion = 0;
                List<string> split = new List<string>(data.name.Split('/'));
                if (split.Count <= 1)
                {
                    currentVersion = 0;
                    //split.Insert(0, currentVersion.ToString());
                }
                else if (int.TryParse(split[split.Count - 2] ?? string.Empty, out currentVersion) == true)
                {
                    split.RemoveAt(split.Count - 2);
                }
                data.version = currentVersion;
                data.path = ListToString(split);
                data.variant = BuildList[i].assetBundleVariant;
                data.check = false;
                data.animBool = new AnimBool(false);
                data.animBool.valueChanged.AddListener(_parent.Repaint);
                _assetBundleDataList.Add(data);
                _assetBundleDataIdxDict.Add(data.name, _assetBundleDataList.Count - 1);
            }
        }

        int _currentIndex = 0;
        List<CRCChecker> _CRCCheckerList = new List<CRCChecker>();
        List<string> _addUrlList = new List<string>();
        List<string> _addKeyList = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (_isCRC == true)
            {
                IsLastCRCCheckerList();
            }
        }

        void UdateCRCChecker()
        {
            if (_CRCCheckerList.Count == 0)
                return;

            _isCRC = true;

            _addUrlList.Add(_CRCCheckerList[_currentIndex].CDNUrl);
            _addKeyList.Add(_CRCCheckerList[_currentIndex].CDNKey);
            _currentIndex++;

            if (_currentIndex == _CRCCheckerList.Count)
            {
                if (_isBuildPopPc)
                {
                    // etc압축 설정을 원복
                    EditorSettings.etcTextureCompressorBehavior = _etcTextureCompressorBehavior;
                    EditorSettings.etcTextureFastCompressor = _etcTextureFastCompressor;
                    EditorSettings.etcTextureNormalCompressor = _etcTextureNormalCompressor;
                    EditorSettings.etcTextureBestCompressor = _etcTextureBestCompressor;
                }

                _CRCCheckerList.Clear();
                _currentIndex = 0;
                _isCRC = false;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                for (int i = 0; i < _addKeyList.Count; i++)
                {
                    if (BundleBuildList.Instance.dic_BundleInfo.TryGetValue(_addKeyList[i], out var info))
                    {
                        string path = GetFilePath(_selectToolBarIdx);

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        var prevPath = $"{path}/{_addKeyList[i]}";
                        var targetPath = $"{path}/{info.version}/{_addKeyList[i]}";

                        FileInfo fileInfo = new FileInfo(prevPath);
                        FileInfo targetFileInfo = new FileInfo(targetPath);

                        if (Directory.Exists(fileInfo.Directory.FullName))
                        {
                            var files = fileInfo.Directory.GetFiles(string.Format("{0}.*", fileInfo.Name.Replace(string.Format(".{0}", fileInfo.Extension), string.Empty)));
                            foreach (var file in files)
                            {
                                string targetFullName = $"{targetFileInfo.Directory.FullName}/{targetFileInfo.Name}{file.Extension}";
                                if (!Directory.Exists(targetFileInfo.Directory.FullName))
                                {
                                    Directory.CreateDirectory(targetFileInfo.Directory.FullName);
                                }

                                File.Delete(targetFullName);
                                File.Move(file.FullName, targetFullName);
                            }
                        }

                        _addKeyList[i] = $"{info.version}/{_addKeyList[i]}";
                    }
                }

                if (_uploadImmediately)
                {
                    AssetBundleAutoUploadEditor.Start(_targetRegion, _addKeyList);

                    _addUrlList.Clear();
                    _addKeyList.Clear();
                }
                else
                {
                    OnComplete(true);
                }
            }
        }

        private void OnComplete(bool showDialog)
        {
            if (showDialog)
            {
                // 결과값 출력
                bool success = EditorUtility.DisplayDialog("에셋 번들 빌드 성공!",
                            "추가가 필요한 에셋 번들\n" + string.Join("\n", _addUrlList.ToArray()),
                            "에셋번들 폴더 열기", "닫기");

                if (success)
                {
                    if (_selectToolBarIdx < _targetPathArr.Length)
                        Application.OpenURL(GetFilePath(_selectToolBarIdx));
                    else
                        Application.OpenURL(string.Format("{0}/../AssetBundles", Application.dataPath));
                }
            }

            _addUrlList.Clear();
            _addKeyList.Clear();
        }

        class CRCChecker
        {
            public enum EResult
            {
                READY,
                PROCESSING,
                ERROR,
                EXIST,
                NEED_ADD
            }

            public string CDNUrl;
            public string CRC;
            public string CDNKey;

            bool _start = false;
            WWW _www;
            EResult _result;

            public EResult Check()
            {
                if (_result == EResult.EXIST || _result == EResult.ERROR || _result == EResult.NEED_ADD)
                {
                    return _result;
                }

                if (!_start && _www == null)
                {
                    _start = true;
                    _www = new WWW(CDNUrl);
                }

                if (_www == null)
                {
                    _result = EResult.ERROR;
                }
                else if (_start && _www.isDone)
                {
                    if (string.IsNullOrEmpty(_www.error))
                    {
                        try
                        {
                            Stream stream = new MemoryStream(_www.bytes);
                            StreamReader reader = new StreamReader(stream);
                            _result = CRC == GetCRC(reader) ? EResult.EXIST : EResult.NEED_ADD;
                            stream.Dispose();
                            reader.Dispose();
                        }
                        catch
                        {
                            _result = EResult.ERROR;
                            return _result;
                        }
                    }
                    else
                    {
                        _result = _www.error.Contains("404") ? EResult.NEED_ADD : EResult.ERROR;
                    }
                }
                else
                {
                    _result = EResult.PROCESSING;
                }

                return _result;
            }

            public static string GetCRC(StreamReader inReader)
            {
                string crcStr = null;
                while (!inReader.EndOfStream)
                {
                    crcStr = inReader.ReadLine();
                    if (crcStr.Contains("CRC"))
                    {
                        crcStr = crcStr.Replace("CRC:", "").Trim();
                        break;
                    }
                    else if (crcStr.Contains("NoSuchKey"))
                    {
                        break;
                    }
                }
                return crcStr;
            }
        }
    }
}
#endif