using UnityEngine;

namespace PlayJam.InGame.PickingCabbage
{
    public enum EDir
    {
        Left,
        Right,
    }

    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/양배추 떼기(PickingCabbage)", order = int.MaxValue)]
    public class PickingCabbageData : MiniGameData
    {
        [Header("양배추 껍질 개수 증가량")]
        public float CabbageSkinWeight = 0.1f;
    }
}
