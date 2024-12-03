using UnityEngine;

namespace PlayJam.InGame.StirFriedRice
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/볶음밥 만들기(StirFriedRiceData)", order = int.MaxValue)]
    public class StirFriedRiceData : MiniGameData
    {
        [Header("원 최소 속도")]
        public float CircleMinSpeed = 1;

        [Header("원 최대 속도")]
        public float CircleMaxSpeed = 10;

        [Header("원 속도 증가량")]
        public float SpeedIncreaseWeight = 1;
    }
}
