using UnityEngine;

namespace PlayJam.InGame
{
    /// <summary>
    /// 미니게임 전역 설정값
    /// </summary>
    [CreateAssetMenu(fileName = "MiniGameConfig", menuName = "PlayJam/Scriptable Object/미니게임 Config 파일 생성", order = int.MaxValue)]
    public class MiniGameConfig : ScriptableObject
    {
        [Header("한 판당 플레이 시간")]
        public float PlayTime;

        [Header("초기 하트 개수 (1~3)")]
        public int InitialHeartCount;

        [Header("난이도에 따른 시간 감소량")]
        public float DecresedPlayTimePerRound;

        [Header("최소 플레이 시간")]
        public float MinimumPlayTime;
    }
}