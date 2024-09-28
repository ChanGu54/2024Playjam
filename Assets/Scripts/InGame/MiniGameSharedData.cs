using UnityEngine;
using utils;

namespace PlayJam.MiniGame
{
    /// <summary>
    /// 인게임에서 사용할 공용 데이터
    /// </summary>
    public class MiniGameSharedData : Singleton<MiniGameSharedData>
    {
        /// <summary>
        /// 선택된 Config값
        /// </summary>
        public MiniGameConfig Config;

        /// <summary>
        /// 현재 게임 난이도
        /// </summary>
        public EMiniGameDifficulty Difficulty => Config.Difficulty;

        /// <summary>
        /// 초기화
        /// </summary>
        public void Initialize(MiniGameConfig inConfig)
        {
            Config = inConfig;
        }

        /// <summary>
        /// 데이터 초기화 처리
        /// </summary>
        public void Clear()
        {
            Config = null;
        }
    }
}