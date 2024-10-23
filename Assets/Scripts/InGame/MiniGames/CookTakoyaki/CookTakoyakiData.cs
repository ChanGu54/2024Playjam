using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.CookTakoyaki
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/타코야끼 만들기(CookTakoyaki)", order = int.MaxValue)]
    public class CookTakoyakiData : MiniGameData
    {
        [Header("타코야끼 최대개수(Min~9")]
        public int TakoyakiMaxCount;

        [Header("타코야끼 최소개수(1~Max)")]
        public int TakoyakiMinCount;
    }
}
