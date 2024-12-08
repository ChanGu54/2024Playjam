using PlayJam.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame.MakeDumplingSkin
{
    /// <summary>
    /// 
    /// </summary>
    public class MakeDumplingSkinPlayer : MiniGamePlayer
    {
        [SerializeField]
        private MakeDumplingSkinData _config;

        [SerializeField]
        private Transform _trBar;

        [SerializeField]
        private SpriteRenderer _rendSkin;


        private float _curMoveDistance;

        private float _objectiveMoveDistance;



        private float _startSkinSize = 200;

        private float _endSkinSize = 400;

        private float _barLimitY_Upper = -40;

        private float _barLimitY_Under = -305;


        public override void Clear()
        {
            base.Clear();

            _config = null;
            _curMoveDistance = 0;
            _objectiveMoveDistance = 0;

            _lastTouchPos = Vector3.zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as MakeDumplingSkinData;

            _curMoveDistance = 0;
            _objectiveMoveDistance = _config.ClearDistance + _config.ClearDistanceWeight * MiniGameSharedData.Instance.StageCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            _rendSkin.size = Vector2.one * _startSkinSize;
            _trBar.localPosition = new Vector3(0, (_barLimitY_Under + _barLimitY_Upper) / 2f, 1);

            yield return new WaitForSeconds(0.5f);

            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        public override void OnPostStart()
        {
            base.OnPostStart();
        }

        private Vector3 _lastTouchPos;
        private bool _isPlus = false;

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (IsPlaying == false)
            {
                return;
            }

#if UNITY_EDITOR
            if (Input.GetMouseButton(0) == false)
            {
                _lastTouchPos = Vector3.zero;
                return;
            }

            Touch[] touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                    phase = _lastTouchPos == Vector3.zero ? TouchPhase.Began : TouchPhase.Moved,
                }
            };
#else
            if (Input.touchCount <= 0)
            {
                _lastTouchPos = Vector3.zero;
                return;
            }

            Touch[] touches = Input.touches;
#endif
            Touch touch = touches[0];
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                _lastTouchPos = pos;
                return;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (_lastTouchPos == Vector3.zero)
                    _lastTouchPos = pos;

                Vector3 moveDist = pos - _lastTouchPos;
                Vector3 prevBarPos = _trBar.localPosition;
                Vector3 curBarPos = new Vector3(prevBarPos.x, Mathf.Clamp(moveDist.y + prevBarPos.y, _barLimitY_Under, _barLimitY_Upper), prevBarPos.z);

                float realMoveDistY = curBarPos.y - prevBarPos.y;

                if (realMoveDistY > 0)
                {
                    if (_isPlus == false)
                    {
                        _isPlus = true;
                        SoundManager.Instance.Play(ESoundType.SFX, "MakeDumplingSkin");
                    }
                }
                else
                {
                    if (_isPlus == true)
                    {
                        _isPlus = false;
                        SoundManager.Instance.Play(ESoundType.SFX, "MakeDumplingSkin");
                    }
                }

                _trBar.localPosition = curBarPos;

                _curMoveDistance = Mathf.Min(_objectiveMoveDistance, _curMoveDistance + Mathf.Abs(realMoveDistY));
                _rendSkin.size = Vector3.Lerp(Vector3.one * _startSkinSize, Vector3.one * _endSkinSize, _curMoveDistance / _objectiveMoveDistance);

                _lastTouchPos = pos;

                // 게임 클리어
                if (_curMoveDistance == _objectiveMoveDistance)
                {
                    StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
                }
            }
        }
    }
}
