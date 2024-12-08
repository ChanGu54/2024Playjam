using UnityEngine;

namespace PlayJam.InGame.SqueezeLemon
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/레몬 짜기(SqueezeLemonData)", order = int.MaxValue)]
    public class SqueezeLemonData : MiniGameData
    {
        [Header("휘저어야 하는 횟수 기본값")]
        public int SqueezeCount;

        [Header("스테이지 상승에 따른 증가값")]
        public int IncreaseCountPerStageCount;

        [Header("한 바퀴를 드래그할 때 정상적으로 돌았는지 체크하는 가중치 (테스트용)")]
        public int CheckpointCorrection;
    }
}
