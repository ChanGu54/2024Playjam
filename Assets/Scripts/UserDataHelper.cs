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
            }

            return _instance;
        }
    }

    /// <summary>
    /// 유저 ID
    /// </summary>
    public string ID => SystemInfo.deviceUniqueIdentifier;

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
        }
    }

    /// <summary>
    /// 보유 재화
    /// </summary>
    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt("KEY_Coin");
        }
        set
        {
            PlayerPrefs.SetInt("KEY_Coin", value);
        }
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
                PlayerPrefs.SetString("KEY_OwnedCostumes", string.Join(',', _ownedCostumes));
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
        PlayerPrefs.SetString("KEY_OwnedCostumes", string.Join(',', inCostumes));
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

        _ownedCostumes.Append(inCostume);
        PlayerPrefs.SetString("KEY_OwnedCostumes", string.Join(',', _ownedCostumes));
    }
}
