using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.CuttingFruit
{
    public enum EFruit
    {
        NULL = -1,
        Apple,
        Banana,
        Orange,
        Pineapple,
        Watermelon,
    }


    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/과일 자르기(CuttingFruitData)", order = int.MaxValue)]
    public class CuttingFruitData : MiniGameData
    {

    }
}
