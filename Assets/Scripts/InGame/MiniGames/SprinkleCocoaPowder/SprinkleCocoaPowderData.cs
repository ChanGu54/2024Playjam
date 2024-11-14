using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.SprinkleCocoaPowder
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/코코아 파우더 뿌리기(SprinkleCocoaPowderData)", order = int.MaxValue)]
    public class SprinkleCocoaPowderData : MiniGameData
    {
        [Header("클리어까지 모아야 할 점수")]
        public int ClearScore = 10;

        [Header("터치 한 번으로 받을 수 있는 최대 점수 (평행에 가까운 상태)")]
        public float MaxScorePerTouch = 1;

        [Header("터치 한 번으로 받을 수 있는 최소 점수 (평행과 먼 상태)")]
        public float MinScorePerTouch = 0.5f;

        [Header("최대 회전 각도")]
        public int MaxRotationDegree = 15;
    }
}
