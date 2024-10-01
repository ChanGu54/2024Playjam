using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.World
{
    public class CloudMover : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _listClouds;

        [SerializeField]
        private float _speed = 20;

        [SerializeField]
        private float _limitX = 1440;

        [SerializeField]
        private float _baseX = -1440;

        void Update()
        {
            float deltaTime = Time.deltaTime;
            float speedPerFrame = _speed * deltaTime;

            for (int i = 0; i < _listClouds.Length; i++)
            {
                _listClouds[i].position += Vector3.right * speedPerFrame;

                if (_listClouds[i].position.x > _limitX)
                    _listClouds[i].position -= Vector3.right * (_limitX - _baseX);
            }
        }
    }
}
