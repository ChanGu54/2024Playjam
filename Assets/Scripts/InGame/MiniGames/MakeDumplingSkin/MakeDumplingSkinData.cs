using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.MakeDumplingSkin
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/만두피 만들기(MakeDumplingSkinPlayer)", order = int.MaxValue)]
    public class MakeDumplingSkinData : MiniGameData
    {
        [Header("클리어 하기 위해 밀대가 이동해야 할 거리")]
        public float ClearDistance = 2700;

        [Header("밀대 이동 거리 가중치")]
        public float ClearDistanceWeight = 10;
    }
}