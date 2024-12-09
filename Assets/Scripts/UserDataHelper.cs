using System;
using System.Linq;
using PlayJam.Character;
using UnityEngine;

public class UserDataHelper
{
    private static UserDataHelper _instance;

    public static UserDataHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UserDataHelper();
                _instance.Initialize();
            }

            return _instance;
        }
    }

    /// <summary>
    /// 유저 ID
    /// </summary>
    public string ID => SystemInfo.deviceUniqueIdentifier;

    /// <summary>
    /// 유저 ID
    /// </summary>
    public string SAVED_ID
    {
        get
        {
            return PlayerPrefs.GetString("KEY_ID");
        }
        set
        {
            PlayerPrefs.SetString("KEY_ID", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 유저 ID
    /// </summary>
    public string Name
    {
        get
        {
            return PlayerPrefs.GetString("KEY_NAME");
        }
        set
        {
            PlayerPrefs.SetString("KEY_NAME", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 현재 장비한 코스튬
    /// </summary>
    public ECostume EquippedCostume
    {
        get
        {
            return (ECostume)PlayerPrefs.GetInt("KEY_EquippedCostume");
        }
        set
        {
            PlayerPrefs.SetInt("KEY_EquippedCostume", (int)value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 보유 재화
    /// </summary>
    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt("KEY_Coin", 0);
        }
        set
        {
            PlayerPrefs.SetInt("KEY_Coin", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// BGM 켜져 있는지
    /// </summary>
    public bool IsBGMOn
    {
        get
        {
            return PlayerPrefs.GetInt("KEY_BGM", 0) == 0;
        }
        set
        {
            PlayerPrefs.SetInt("KEY_BGM", value ? 0 : 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// SFX 켜져 있는지
    /// </summary>
    public bool IsSFXOn
    {
        get
        {
            return PlayerPrefs.GetInt("KEY_SFX", 0) == 0;
        }
        set
        {
            PlayerPrefs.SetInt("KEY_SFX", value ? 0 : 1);
            PlayerPrefs.Save();
        }
    }

    public int CostumeCost => 1500;

    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    {
        if (SAVED_ID != ID)
        {
            Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
        IsBGMOn = true;
        IsSFXOn = true;

        EquippedCostume = ECostume.IDLE;

        Coin = 0;

        SAVED_ID = ID;

        Name = UnityEngine.Random.Range(1000000000, int.MaxValue).ToString();

        PlayerPrefs.SetString("KEY_OwnedCostumes", string.Empty);
        PlayerPrefs.Save();
    }

    private ECostume[] _ownedCostumes;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ECostume[] GetOwnedCostumes()
    {
        if (_ownedCostumes == null)
        {
            string phrase = PlayerPrefs.GetString("KEY_OwnedCostumes");
            if (string.IsNullOrEmpty(phrase))
            {
                _ownedCostumes = new ECostume[] { ECostume.IDLE };
                int[] ownedCostumesInt = Array.ConvertAll(_ownedCostumes, e => (int)e);
                PlayerPrefs.SetString("KEY_OwnedCostumes", string.Join(',', ownedCostumesInt));
                phrase = PlayerPrefs.GetString("KEY_OwnedCostumes");
                PlayerPrefs.Save();
            }

            string delimiter = ",";
            _ownedCostumes = Array.ConvertAll(phrase.Split(delimiter), (x) => (ECostume)int.Parse(x));
        }

        return _ownedCostumes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inCostumes"></param>
    public void OverrideOwnedCostumes(ECostume[] inCostumes)
    {
        int[] ownedCostumesInt = Array.ConvertAll(inCostumes, e => (int)e);
        PlayerPrefs.SetString("KEY_OwnedCostumes", string.Join(',', ownedCostumesInt));
        PlayerPrefs.Save();
        _ownedCostumes = inCostumes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inCostumes"></param>
    public void AddOwnedCostumes(ECostume inCostume)
    {
        for (int i = 0; i < _ownedCostumes.Length; i++)
        {
            if (_ownedCostumes[i] == inCostume)
            {
                Debug.LogError($"[AddOwnedCostumes] 추가하려는 코스튬을 이미 가지고 있습니다! : {inCostume}");
                return;
            }
        }

        _ownedCostumes = _ownedCostumes.Append(inCostume).ToArray();
        int[] ownedCostumesInt = Array.ConvertAll(_ownedCostumes, e => (int)e);
        PlayerPrefs.SetString("KEY_OwnedCostumes", string.Join(',', ownedCostumesInt));
        PlayerPrefs.Save();
    }
}
