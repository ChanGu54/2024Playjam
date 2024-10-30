using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.CuttingFruit
{
    public class CuttingFruitPlayer : MiniGamePlayer
    {
        [SerializeField]
        private CuttingFruitData _config;

        [SerializeField]
        private SerializedDictionary<EFruit, CuttingFruitTarget> _fruits; 
    }
}
