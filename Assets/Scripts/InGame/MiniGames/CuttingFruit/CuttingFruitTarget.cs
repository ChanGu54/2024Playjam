using System;
using UnityEngine;

namespace PlayJam.InGame.CuttingFruit
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class CuttingFruitTarget : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D[] _fragments;

        [SerializeField]
        private float _weight;

        [SerializeField]
        private float _gravity;

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            Array.ForEach(_fragments, (x) =>
            {
                x.velocity = Vector3.zero;
                x.mass = _weight;
                x.gravityScale = _gravity;
                x.transform.localPosition = Vector3.zero;
                x.bodyType = RigidbodyType2D.Static;
            });


        }
    }
}
