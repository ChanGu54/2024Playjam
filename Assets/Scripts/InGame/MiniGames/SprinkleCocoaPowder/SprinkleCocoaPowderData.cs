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
        [Header("Ŭ������� ��ƾ� �� ����")]
        public int ClearScore = 10;

        [Header("��ġ �� ������ ���� �� �ִ� �ִ� ���� (���࿡ ����� ����)")]
        public float MaxScorePerTouch = 1;

        [Header("��ġ �� ������ ���� �� �ִ� �ּ� ���� (����� �� ����)")]
        public float MinScorePerTouch = 0.5f;

        [Header("�ִ� ȸ�� ����")]
        public int MaxRotationDegree = 15;
    }
}
