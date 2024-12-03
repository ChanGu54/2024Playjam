using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.SprinkleCocoaPowder
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/���ھ� �Ŀ�� �Ѹ���(SprinkleCocoaPowderData)", order = int.MaxValue)]
    public class SprinkleCocoaPowderData : MiniGameData
    {
        [Header("클리어 요구 점수")]
        public int ClearScore = 10;

        [Header("클리어 요구 점수 가중치")]
        public int ClearScoreWeight = 10;

        [Header("터치 1회당 받을 수 있는 최대 점수")]
        public float MaxScorePerTouch = 1;

        [Header("터치 1회당 받을 수 있는 최소 점수")]
        public float MinScorePerTouch = 0.1f;

        [Header("최대 회전 각")]
        public int MaxRotationDegree = 15;
    }
}
