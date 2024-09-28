using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;


public enum ERegionEndpoint
{
    APNortheast1,
    APNortheast2,
    APSouth1,
    APSoutheast1,
    APSoutheast2,
    CNNorth1,
    EUCentral1,
    EUWest1,
    SAEast1,
    USEast1,
    USEast2,
    USGovCloudWest1,
    USWest1,
    USWest2,
}

public enum ECountryCode
{
    JP
}
public struct GCSSettings
{
    public string projectId;
    public string bucketName;
    public string credentialPath;
    public string remotePath;
}

public class AssetBundleUploader : EditorWindow
{
    [MenuItem("ANIPANG5/AssetBundle/AssetBundleUploader")]
    public static void ShowWindow()
    {
        AssetBundleUploader wnd = GetWindow<AssetBundleUploader>();
        wnd.titleContent = new GUIContent("AssetBundleUploader");
    }

    public static readonly GCSSettings SETTING_JP = new GCSSettings
    {
        projectId = "anipang5",
        bucketName = "anipang5-resources",
        credentialPath = "Assets/Key/anipang5-key.json",
        remotePath = "assetbundles"
    };

    public const string LOCAL_PATH_ROOT = "AssetBundles";
    
    private List<GCSSettings> _settingList;
    private List<StorageClient> _clientList;
    private string _syncPlatform = string.Empty;
    private string _remotePath = string.Empty;
    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        _settingList = new List<GCSSettings> { SETTING_JP };
        _clientList = new List<StorageClient>();
        foreach (var setting in _settingList)
        {
            var client = StorageClient.Create(GoogleCredential.FromFile(setting.credentialPath));
            _clientList.Add(client);
        }
    }
    
    private void OnDestroy()
    {
        Cancel();

    }
    
    public void Cancel() => _cancellationTokenSource?.Cancel();

    public void OnEnable()
    {
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ThirdParty/AssetBundles-Browser-master/Editor/AssetBundleUploader.uxml");
        root.Add(visualTree.CloneTree());

        root.Q<Button>("BTN_Android").clickable.clicked += () => OnClickButton("aos");
        root.Q<Button>("BTN_iOS").clickable.clicked += () => OnClickButton("ios");
        root.Q<Button>("BTN_Windows").clickable.clicked += () => OnClickButton("win");
        root.Q<Button>("BTN_OSX").clickable.clicked += () => OnClickButton("osx");

        var currentPlatformButton = "";
#if UNITY_ANDROID
        currentPlatformButton = "BTN_Android";
#elif UNITY_IOS
        currentPlatformButton = "BTN_iOS";
#elif UNITY_STANDALONE_WIN
        currentPlatformButton = "BTN_Windows";
#elif UNITY_STANDALONE_OSX
        currentPlatformButton = "BTN_OSX";
#endif
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ThirdParty/AssetBundles-Browser-master/Editor/AssetBundleUploader.uss");
        root.Q<Button>(currentPlatformButton).styleSheets.Add(styleSheet);
        
        var uxmlField = root.Q<EnumField>("ENUM_Server");
        uxmlField.Init(ECountryCode.JP);
        uxmlField.RegisterCallback<ChangeEvent<Enum>>(evt =>
        {

        });

    }
    

    private void OnClickButton(string inName)
    {
        Sync(inName).ContinueWith(_ => { });
    }

    /// <summary>
    /// 서버와 로컬 에셋번들을 서로 동기화 시킨다
    /// </summary>
    /// <param name="inPlatform"></param>
    private async Task Sync(string inPlatform)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _syncPlatform = inPlatform;
        _remotePath = $"{_settingList[0].remotePath}/{inPlatform}/";

        var newKeyMap = new Dictionary<int, Dictionary<string, bool>>();

        for (int i = 0; i < _clientList.Count; i++)
        {
            await AssetBundleUploadHelper.Sync((index, inNewKeyDic) => newKeyMap.Add(index, inNewKeyDic.ToDictionary(x => x, x => false))
                                             , _remotePath
                                             , _syncPlatform
                                             , _clientList[i]
                                             , _settingList[i]
                                             , i
                                             , _cancellationTokenSource);
        }

        SelectEditorWindow window = GetWindowWithRect<SelectEditorWindow>(new Rect(0, 0, 500, 750)
                                                                        , true
                                                                        , $"[{_syncPlatform.ToUpper()}] Send to GCS index"
                                                                        , true);

        window.SetData(_settingList, newKeyMap, this);
    }

    /// <summary>
    /// AWS 에 올림
    /// </summary>
    /// <param name="inIndex"></param>
    /// <param name="inCallback"></param>
    /// <param name="inNewKeyList"></param>
    public async void ApplyModifiedAssets(int inIndex, Action<bool, int> inCallback, List<string> inNewKeyList)
    {
        Debug.Log("<color=#FCDC79FF>서버 동기화 성공</color>");
        Debug.Log("Apply choose assets");

        await AssetBundleUploadHelper.ApplyModifiedAssets(success =>
        {
            if (success)
            {
                Debug.Log("Apply Complete!");
            }
        }, inNewKeyList, _settingList[inIndex], _remotePath, _syncPlatform, _clientList[inIndex], _cancellationTokenSource);

        inCallback?.Invoke(true, inIndex);
    }
}

public class SelectEditorWindow : EditorWindow
{
    private List<GCSSettings> _settings;
    private Dictionary<int, Dictionary<string, bool>> _addKeyDic;
    private Vector2 _addkeyScroll = Vector2.zero;
    private AssetBundleUploader _delegate;
    private bool _isProc;
    private int _index;
    private int _selectIdx;

    public void SetData(List<GCSSettings> inSettings, Dictionary<int, Dictionary<string, bool>> inAddKeyList, AssetBundleUploader inDelegate)
    {
        _settings = inSettings;
        _addKeyDic = inAddKeyList;
        _delegate = inDelegate;
    }

    private static void CreateList(string inLabelText, Dictionary<string, bool> inKeyList, ref Vector2 refScroll)
    {
        var keys = inKeyList.Select(x => x.Key).ToList();
        EditorGUILayout.LabelField($"{inLabelText}({inKeyList.Count(x => x.Value)}/{inKeyList.Count})", EditorStyles.boldLabel);
        
        refScroll = EditorGUILayout.BeginScrollView(refScroll, GUILayout.Height(200));
        
        foreach (var key in keys)
        {
            inKeyList[key] = EditorGUILayout.ToggleLeft(key, inKeyList[key]);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("전체 선택"))
        {
            foreach (var key in keys)
            {
                inKeyList[key] = true;
            }
        }

        if (GUILayout.Button("전체 해제"))
        {
            foreach (var key in keys)
            {
                inKeyList[key] = false;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void OnGUI()
    {
        if (_isProc)
        {
            EditorGUILayout.LabelField("데이터를 업로드 중 입니다.", EditorStyles.boldLabel);
            return;
        }

        string[] menus = new string[_addKeyDic.Keys.Count];
        for (int i = 0; i < _addKeyDic.Keys.Count; i++)
        {
            menus[i] = _settings[i].bucketName;
        }

        _selectIdx = GUILayout.Toolbar(_selectIdx, menus);
        
        for (int i = 0; i < _addKeyDic.Keys.Count; i++)
        {
            if (_selectIdx == i)
            {
                _index = i;

                CreateList("추가된 에셋 번들", _addKeyDic[_index], ref _addkeyScroll);
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (_addKeyDic[_index].Any(x => x.Value))
                {
                    if (GUILayout.Button("선택된 파일 업로드", GUILayout.Height(50)))
                    {
                        _isProc = true;
                        _delegate.ApplyModifiedAssets(_index, (isSuccess, inIndex) =>
                        {
                            _isProc = false;
                            Close();
                        }, _addKeyDic[_index].Where(x => x.Value).Select(x => x.Key).ToList());
                    }
                }

                break;
            }
        }
    }
    
    private void OnDestroy()
    {
        _delegate.Cancel();
    }
}
