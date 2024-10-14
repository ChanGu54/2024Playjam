using UnityEngine;


namespace PlayJam.InGame
{
    //[CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames.", order = int.MaxValue)]
    public class MiniGameData : ScriptableObject
    {
        [Header("미니게임 종류")]
        public EMiniGame GameKind;

        [Header("대응되는 음식")]
        public EFood Food;

        [Header("입력 방식")]
        public EInputMethod InputMethod;
    }
}
