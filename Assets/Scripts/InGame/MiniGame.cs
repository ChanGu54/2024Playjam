using UnityEngine;


namespace PlayJam.MiniGame
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/미니게임 파일 생성", order = int.MaxValue)]
    public class MiniGame : ScriptableObject
    {
        [Header("게임 종류")]
        public EMiniGame GameKind;

        [Header("음식")]
        public EFood Food;

        [Header("값1")]
        public float Value1;

        [Header("값1_설명")]
        public string Value1_Desc;

        [Header("값2")]
        public float Value2;

        [Header("값2_설명")]
        public string Value2_Desc;

        [Header("값3")]
        public float Value3;

        [Header("값3_설명")]
        public string Value3_Desc;

        [Header("난이도에 따른 증가값")]
        public float Value1_Increase_Rate;

        public float Value2_Increase_Rate;

        public float Value3_Increase_Rate;
    }
}
