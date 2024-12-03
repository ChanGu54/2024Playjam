using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayJam.InGame.StackHamburger.StackHamburgerData;

namespace PlayJam.InGame.StackHamburger
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "PlayJam/Scriptable Object/MiniGames/햄버거 쌓기(StackHamburgerData)", order = int.MaxValue)]
    public class StackHamburgerData : MiniGameData
    {
        public enum EElement
        {
            Ham,
            Tomato,
            cabbage,
            Bread,
            Cheese,
            Trap,
        }

        public List<StackHamburgerIngredientData> Targets;

        [Header("함정 등장 난이도")]
        public int TrapAppearLevel;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class StackHamburgerIngredientData
    {
        [Header("떨어지는 오브젝트")]
        public EElement Element;

        [Header("떨어지는 데 걸리는 시간")]
        public float DropTIme;
    }
}
