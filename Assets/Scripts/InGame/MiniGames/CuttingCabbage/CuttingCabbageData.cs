using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.CuttingCabbage
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/양배추 자르기(CuttingCabbage)", order = int.MaxValue)]
    public class CuttingCabbageData : MiniGameData
    {
        public enum EElement
        {
            /// <summary>
            /// 앵상추
            /// </summary>
            Cabbage,

            /// <summary>
            /// 당근
            /// </summary>
            Carrot,

            /// <summary>
            /// 가지
            /// </summary>
            EggPlant,

            /// <summary>
            /// 작은 무
            /// </summary>
            Radish_Short,

            /// <summary>
            /// 긴 무
            /// </summary>
            Radish_Long,

            /// <summary>
            /// 양파
            /// </summary>
            Onion,
        }

        public List<CuttingCabbageTarget> Targets;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CuttingCabbageTarget
    {
        [Header("자를 대상")]
        public CuttingCabbageData.EElement Element;

        [Header("등장하는 스테이지 (최소)")]
        public int AppeaeredStage;

        [Header("잘라야 하는 최소 횟수")]
        public int CuttingMinCount;

        [Header("잘라야 하는 최대 횟수")]
        public int CuttingMaxCount;

        [HideInInspector]
        public int CuttingCount;
    }
}
