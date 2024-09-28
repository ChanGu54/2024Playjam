using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class AssetBundleUploadHelper
{
    public enum REGION
    {
        JP,
        KAKAO,
        GLOBAL,
    }

    public static async Task Sync(Action<int, List<string>> inCallback, string remotePath, string syncPlatform, StorageClient client, GCSSettings settings, int index, CancellationTokenSource tokenSource)
    {
        string localPath = GetLocalPath(syncPlatform);
        if (!Directory.Exists(localPath))
        {
            Debug.Log("[AssetBundleUploadHelper] <color=#FF0000>에셋 번들 로컬 경로가 없습니다!</color>");
            inCallback?.Invoke(index, null);
            return;
        }

        var outNewKeyList = new List<string>();

        await SyncAssetBundle(client, settings, localPath, remotePath, outNewKeyList, tokenSource);

        inCallback?.Invoke(index, outNewKeyList);
    }

    public static async Task Sync(Action<List<string>> inCallback, string remotePath, string syncPlatform, StorageClient client, GCSSettings settings, CancellationTokenSource tokenSource)
    {
        string localPath = GetLocalPath(syncPlatform);
        if (!Directory.Exists(localPath))
        {
            Debug.Log("[AssetBundleUploadHelper] <color=#FF0000>에셋 번들 로컬 경로가 없습니다!</color>");
            inCallback?.Invoke(null);
            return;
        }

        var outNewKeyList = new List<string>();

        await SyncAssetBundle(client, settings, localPath, remotePath, outNewKeyList, tokenSource);

        inCallback?.Invoke(outNewKeyList);
    }

    private static async Task SyncAssetBundle(StorageClient client, GCSSettings settings, string localPath, string remotePath, List<string> outNewKeyList, CancellationTokenSource tokenSource)
    {
        var localCRCList = new Dictionary<string, string>();
        var remoteCRCList = new Dictionary<string, string>();

        var remoteKeyList = new List<string>();
        var newKeyList = new List<string>();

        LoadLocalManifest(localCRCList, localPath);

        await SyncAssetBundles(client, settings, remotePath, localCRCList, remoteKeyList, tokenSource);
        await LoadRemoteManifest(client, settings, remotePath, remoteKeyList, remoteCRCList, tokenSource);

        CompareCRCBetweenLocalAndRemote(localCRCList, remoteCRCList, newKeyList);

        outNewKeyList.AddRange(newKeyList);
    }

    private static async Task SyncAssetBundles(StorageClient client, GCSSettings settings, string remotePath, Dictionary<string, string> inLocalCRCList, List<string> refRemoteKeyList, CancellationTokenSource tokenSource)
    {
        var objects = client.ListObjects(settings.bucketName, remotePath);

        foreach (var obj in objects)
        {
            Debug.Log($"[AssetBundleUploadHelper] [{settings.bucketName}] Response [{obj.Name}].");
            if (obj.Name.Contains(".manifest") && inLocalCRCList.ContainsKey(obj.Name.Replace(remotePath, "").Replace(".manifest", "")))
            {
                refRemoteKeyList.Add(obj.Name);
            }
        }
    }

    private static async Task LoadRemoteManifest(StorageClient client, GCSSettings settings, string remotePath, List<string> inRemoteKeyList, Dictionary<string, string> refRemoteCRCList, CancellationTokenSource tokenSource)
    {
        Debug.Log($"[AssetBundleUploadHelper] [{settings.bucketName}] Load Remote Manifest.");

        for (int i = 0; i < inRemoteKeyList.Count; i++)
        {
            Debug.Log($"[AssetBundleUploadHelper] [{settings.bucketName}] DownLoading : {inRemoteKeyList.Count - i} Remain");

            using (var stream = new MemoryStream())
            {
                await client.DownloadObjectAsync(settings.bucketName, inRemoteKeyList[i], stream, null, tokenSource.Token);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    string crc = GetCRC(reader);
                    if (crc != null)
                    {
                        refRemoteCRCList.Add(inRemoteKeyList[i].Replace(remotePath, "").Replace(".manifest", ""), crc);
                    }
                }
            }
        }
    }

    private static void LoadLocalManifest(Dictionary<string, string> refLocalCRCList, string localPath)
    {
        var fileInfos = new DirectoryInfo(localPath).GetFiles("*.manifest", SearchOption.AllDirectories);
        foreach (var fileInfo in fileInfos)
        {
            using (StreamReader reader = new StreamReader(fileInfo.FullName))
            {
                var crc = GetCRC(reader);

                if (crc != null)
                {
                    refLocalCRCList.Add(fileInfo.FullName.Replace("\\", "/")
                                         .Replace(localPath, "")
                                         .Replace(".manifest", ""), crc);
                }
            }
        }
    }

    private static string GetCRC(StreamReader inReader)
    {
        string crcStr = null;
        while (!inReader.EndOfStream)
        {
            crcStr = inReader.ReadLine();

            if (crcStr != null && crcStr.Contains("CRC"))
            {
                crcStr = crcStr.Replace("CRC:", "").Trim();
                break;
            }
        }

        return crcStr;
    }

    private static void CompareCRCBetweenLocalAndRemote(Dictionary<string, string> inLocalCRCList, Dictionary<string, string> inRemoteCRCList, List<string> refNewKeyList)
    {
        refNewKeyList.AddRange(inLocalCRCList.Where(x => !inRemoteCRCList.ContainsKey(x.Key)).Select(x => x.Key).ToList());
    }

    public static async Task ApplyModifiedAssets(Action<bool> inCallback, List<string> inAddKeyList, GCSSettings settings, string remotePath, string syncPlatform, StorageClient client, CancellationTokenSource tokenSource)
    {
        foreach (var key in inAddKeyList)
        {
            await UploadFile(client, settings, remotePath + key, GetLocalPath(syncPlatform) + key, tokenSource);
            await UploadFile(client, settings, remotePath + key + ".manifest", GetLocalPath(syncPlatform) + key + ".manifest", tokenSource);
            Debug.Log($"[AssetBundleUploadHelper] Uploaded: {key}");
        }

        Debug.Log("[AssetBundleUploadHelper] Apply Complete!");

        inCallback?.Invoke(true);
    }

    private static async Task UploadFile(StorageClient client, GCSSettings settings, string objectName, string filePath, CancellationTokenSource tokenSource)
    {
        using (var fileStream = File.OpenRead(filePath))
        {
            await client.UploadObjectAsync(
                settings.bucketName, 
                objectName, 
                null, 
                fileStream, 
                new UploadObjectOptions {PredefinedAcl = PredefinedObjectAcl.PublicRead}, 
                tokenSource.Token);
        }
    }

    private static string GetLocalPath(string syncPlatform)
    {
        var localPaths = Application.dataPath.Split('/');
        return localPaths.Take(localPaths.Length - 1).Aggregate(string.Empty, (current, t) => current + $"{t}/") + $"{AssetBundleUploader.LOCAL_PATH_ROOT}/{syncPlatform}/";
    }
}