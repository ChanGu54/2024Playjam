#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;

public class AssetBundleAutoUploadEditor : EditorWindow
{
    private const string SLACK_CHANNEL_URL = "https://hooks.slack.com/services/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";

    public string Platform
    {
        get
        {
            string platform = "aos";

#if UNITY_ANDROID
            platform = "aos";
#elif UNITY_IOS
        platform = "ios";
#endif
            return platform;
        }
    }

    public bool IsInit { get; private set; }
    public bool IsProc { get; private set; }

    private GCSSettings _setting;
    private StorageClient _client;
    private string _syncPlatform = string.Empty;
    private string _remotePath = string.Empty;
    private CancellationTokenSource _cancellationTokenSource;

    public static void Start(AssetBundleUploadHelper.REGION inTargetRegion, List<string> addUrlList)
    {
        List<string> urlList = new List<string>(addUrlList);
        AssetBundleAutoUploadEditor window = GetWindow<AssetBundleAutoUploadEditor>();
        window.Init(inTargetRegion, urlList);
    }

    private List<string> _urlList;
    private AssetBundleUploadHelper.REGION _targetRegion;

    private void Init(AssetBundleUploadHelper.REGION inTargetRegion, List<string> addUrlList)
    {
        _targetRegion = inTargetRegion;
        _urlList = addUrlList;
    }

    private void OnGUI()
    {
        if (IsInit && !IsProc)
        {
            GCSSettings setting = AssetBundleUploader.SETTING_JP;

            Debug.Log($"[AssetBundlesAutoUploadEditor] OnGUI // _urlList = {string.Join(",", _urlList)}, setting.remotePath = {setting.remotePath}");

            Sync(_targetRegion).ContinueWith(_ => { });
        }

        EditorGUILayout.LabelField("실행중 입니다.");
    }

    private void Awake()
    {
        Focus();
    }

    private void OnDestroy()
    {
        //EditorApplication.isPlaying = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        IsInit = true;
        IsProc = false;
    }

    /// <summary>
    /// 서버와 로컬 에셋번들을 서로 동기화 시킨다
    /// </summary>
    /// <param name="inTargetRegion"></param>
    /// <param name="inUrlList"></param>
    /// <returns></returns>
    public async Task Sync(AssetBundleUploadHelper.REGION inTargetRegion)
    {
        IsProc = true;

        _cancellationTokenSource = new CancellationTokenSource();

        switch (inTargetRegion)
        {
            case AssetBundleUploadHelper.REGION.GLOBAL:
                break;
            case AssetBundleUploadHelper.REGION.JP:
                _setting = AssetBundleUploader.SETTING_JP;
                _client = StorageClient.Create(GoogleCredential.FromFile(_setting.credentialPath));
                break;
            case AssetBundleUploadHelper.REGION.KAKAO:
                break;
        }

        _syncPlatform = Platform;
        _remotePath = $"{_setting.remotePath}/{_syncPlatform}/";

        await AssetBundleUploadHelper.Sync(ApplyModifiedAssets
                                         , _remotePath
                                         , _syncPlatform
                                         , _client
                                         , _setting
                                         , _cancellationTokenSource);

        SendToSlackApplyList(_urlList);

        IsProc = false;  // 작업 완료 후 IsProc를 false로 설정
    }

    /// <summary>
    /// AWS 에 해당 파일들 올림
    /// </summary>
    /// <param name="inNewList"></param>
    public async void ApplyModifiedAssets(List<string> inNewList)
    {
        Debug.Log("<color=#FCDC79FF>서버 동기화 성공</color>");
        Debug.Log("Apply choose assets");

        await AssetBundleUploadHelper.ApplyModifiedAssets(success =>
        {
            if (success)
            {
                Debug.Log("Apply Complete!");
            }
        }, inNewList, _setting, _remotePath, _syncPlatform, _client, _cancellationTokenSource);

        _urlList = null;

        EditorUtility.DisplayDialog("에셋 번들 빌드 및 업로드 성공!", $"업로드 리스트 = {string.Join(", ", inNewList)}", "OK");

        IsProc = false;  // 작업 완료 후 IsProc를 false로 설정

        // 윈도우 끄기
        Close();
    }

    /// <summary>
    /// 슬랙 보내기
    /// </summary>
    /// <param name="inAssets"></param>
    private void SendToSlackApplyList(List<string> inAssets)
    {
        List<string> assets = new List<string>();

        foreach (var o in inAssets)
        {
            assets.Add($"`{o}`");
        }

        string assetList = string.Join("\n", assets);

        string data = $"{_setting.bucketName}/{_remotePath} 업로드 완료...!\n\n{assetList}";

        SendToSlack(data);
    }

    /// <summary>
    /// 슬랙 보내기
    /// </summary>
    /// <param name="message"></param>
    public static void SendToSlack(string message)
    {
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();

        string data = $"{{\"text\":\"{message}\"}}";

        UnityWebRequest request = UnityWebRequest.Put(SLACK_CHANNEL_URL, encoder.GetBytes(data));
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SendWebRequest();
    }
}
#endif