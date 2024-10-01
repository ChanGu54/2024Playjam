using UnityEngine;


namespace PlayJam.MiniGame
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/�̴ϰ��� ���� ����", order = int.MaxValue)]
    public class MiniGame : ScriptableObject
    {
        [Header("���� ����")]
        public EMiniGame GameKind;

        [Header("����")]
        public EFood Food;

        [Header("��1")]
        public float Value1;

        [Header("��1_����")]
        public string Value1_Desc;

        [Header("��2")]
        public float Value2;

        [Header("��2_����")]
        public string Value2_Desc;

        [Header("��3")]
        public float Value3;

        [Header("��3_����")]
        public string Value3_Desc;

        [Header("���̵��� ���� ������")]
        public float Value1_Increase_Rate;

        public float Value2_Increase_Rate;

        public float Value3_Increase_Rate;
    }
}
