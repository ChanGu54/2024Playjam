using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.World
{
    public class CloudAnimator : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _rendCloud1;

        [SerializeField]
        private SpriteRenderer _rendCloud2;

        [SerializeField]
        private float _speed = 3f;

        private int _leftCloudIndex;

        // Start is called before the first frame update
        void Start()
        {
            _leftCloudIndex = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
