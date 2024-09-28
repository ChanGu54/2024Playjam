using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    
public class AssetConfigData : ScriptableObject, ISerializationCallbackReceiver
{
    public Dictionary<string, AssetConfig> assetConfigDict = new Dictionary<string, AssetConfig>();
    const string configAssetName = "AssetConfigData";

    private static AssetConfigData instance;
    public static AssetConfigData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load(configAssetName) as AssetConfigData;

                if (instance == null)
                {
                    instance = CreateInstance<AssetConfigData>();

#if UNITY_EDITOR
                    if (!System.IO.Directory.Exists("Assets/Resources/"))
                        AssetDatabase.CreateFolder("Assets", "Resources");

                    AssetDatabase.CreateAsset(instance, string.Format("Assets/Resources/{0}.asset", configAssetName));
#endif

                    return null;
                }
            }

            return instance;
        }
    }

	public List<string> _keys = new List<string>();
    public List<AssetConfig> _values = new List<AssetConfig>();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in assetConfigDict)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        assetConfigDict = new Dictionary<string, AssetConfig> ();

        for (var i = 0; i != Mathf.Min(_keys.Count, _values.Count); i++)
            assetConfigDict.Add(_keys[i], _values[i]);
    }

    public static void Save(AssetConfigData _config)
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(_config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    [Serializable]
    public class AssetConfig
    {
        public List<string> dependList;
        public bool isCompressed = true;

        public AssetConfig()
        {
            dependList = new List<string>();
        }
    }
}
