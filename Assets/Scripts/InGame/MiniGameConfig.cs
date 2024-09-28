using UnityEngine;

namespace PlayJam.MiniGame
{
    /// <summary>
    /// 미니게임 전역 설정값
    /// </summary>
    [CreateAssetMenu(fileName = "MiniGameConfig", menuName = "PlayJam/Scriptable Object/미니게임 Config 파일 생성", order = int.MaxValue)]
    public class MiniGameConfig : ScriptableObject
    {
        [Header("미니게임 난이도")]
        public EMiniGameDifficulty Difficulty;

        [Header("한 판당 플레이 시간")]
        public int PlayTime;

        [Header("초기 하트 개수")]
        public int InitialHeartCount;

        [Header("난이도 변경되는 스테이지")]
        public int DifficultyChangedStageNum;

        [Header("난이도 변경 배수 (n배만큼 게임이 빨라짐)")]
        public float DifficultyIncreasedMultiplier;
    }
}