using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using PlayJam.Sound;

namespace PlayJam.InGame.SqueezeLemon
{
    /// <summary>
    /// 
    /// </summary>
    public class SqueezeLemonPlayer : MiniGamePlayer
    {
        [SerializeField]
        private Transform _trLemonLiquid;

        [SerializeField]
        private Transform _trLemon;

        private SqueezeLemonData _config;

        private int _currentSpinCount;

        private int _requireSpinCount;

        private Vector3 _vecCenter;

        public override void Clear()
        {
            base.Clear();

            _config = null;
            _currentSpinCount = 0;
            _requireSpinCount = 0;
            _vecCenter = Vector3.zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as SqueezeLemonData;

            if (_config == null)
                return;

            int addedSpinCount = (int)(_config.IncreaseCountPerStageCount * MiniGameSharedData.Instance.StageCount);

            _requireSpinCount = _config.SqueezeCount + addedSpinCount;

            _trLemonLiquid.localScale = Vector3.zero;
            _trLemon.rotation = Quaternion.Euler(Vector3.zero);
            _vecCenter = _trLemon.transform.position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            yield return new WaitForSeconds(0.5f);

            MiniGameManager.OnMiniGamePostStart.Invoke();

            if (MiniGameSharedData.Instance.StageCount <= MiniGameSharedData.Instance.AllMiniGameDatas.Count)
            {
                yield return new WaitForSeconds(1f);
                Hand.transform.SetParent(transform);
                yield return null;
                HandAnimator.Play("Drag");
                Hand.transform.localScale = Vector3.one;
                Hand.transform.localPosition = new Vector3(0, -265, 0);
                StartCoroutine(Co_DrawCircle(Hand.transform, 100, 1));
            }
        }

        Vector3 _touchStartPos = Vector3.zero;

        float _cachedStartAngle = 0;
        float _cachedCurAngle = 0;

        float _arriveMinAngle = 0;
        float _arriveMaxAngle = 0;

        float _checkpointAngle = 0;
        float _checkpointMinAngle = 0;
        float _checkpointMaxAngle = 0;

        bool _isCheckpointReached = false;

        public float GetAngleBetweenNormals(Vector2 center, Vector2 point1, Vector2 point2)
        {
            // 중심점에서 각 점으로의 벡터 계산
            Vector2 vector1 = point1 - center;
            Vector2 vector2 = point2 - center;
            Vector2 normal1 = vector1.normalized;
            Vector2 normal2 = vector2.normalized;

            // 두 법선 벡터 사이의 각도 계산
            float angle = Vector2.Angle(normal1, normal2);

            if (vector2.y > vector1.y)
                angle = 360 - angle;

            // 각도의 절대값을 반환 (항상 양수)
            return angle;
        }

        public override IEnumerator OnSuccess(Action inCallback)
        {
            yield return new WaitForSeconds(1f);

            yield return base.OnSuccess(inCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (IsPlaying == false)
            {
                return;
            }

            if (_currentSpinCount >= _requireSpinCount)
            {
                return;
            }

#if UNITY_EDITOR
            if (Input.GetMouseButton(0) == false)
            {
                _touchStartPos = Vector3.zero;
                return;
            }

            Touch[] touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                    phase = _touchStartPos == Vector3.zero ? TouchPhase.Began : TouchPhase.Moved,
                }
            };
#else
            if (Input.touchCount <= 0)
            {
                _touchStartPos = Vector3.zero;
                return;
            }

            Touch[] touches = Input.touches;
#endif
            Touch touch = touches[0];
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                _touchStartPos = pos;

                _cachedStartAngle = GetAngleBetweenNormals(_vecCenter, _touchStartPos, _vecCenter + Vector3.right);
                _arriveMinAngle = _arriveMinAngle - _config.CheckpointCorrection < 0 ? _arriveMinAngle - _config.CheckpointCorrection + 360 : _arriveMinAngle - _config.CheckpointCorrection;
                _arriveMaxAngle = _arriveMinAngle + _config.CheckpointCorrection > 360 ? _arriveMinAngle + _config.CheckpointCorrection - 360 : _arriveMinAngle + _config.CheckpointCorrection;

                _checkpointAngle = (_cachedStartAngle + 180) % 360;

                _checkpointMinAngle = _checkpointAngle - _config.CheckpointCorrection < 0 ? _checkpointAngle - _config.CheckpointCorrection + 360 : _checkpointAngle - _config.CheckpointCorrection;
                _checkpointMaxAngle = _checkpointAngle + _config.CheckpointCorrection > 360 ? _checkpointAngle + _config.CheckpointCorrection - 360 : _checkpointAngle + _config.CheckpointCorrection;

                return;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (_touchStartPos == Vector3.zero)
                {
                    _touchStartPos = pos;

                    _cachedStartAngle = GetAngleBetweenNormals(_vecCenter, _touchStartPos, _vecCenter + Vector3.right);
                    _arriveMinAngle = _arriveMinAngle - _config.CheckpointCorrection < 0 ? _arriveMinAngle - _config.CheckpointCorrection + 360 : _arriveMinAngle - _config.CheckpointCorrection;
                    _arriveMaxAngle = _arriveMinAngle + _config.CheckpointCorrection > 360 ? _arriveMinAngle + _config.CheckpointCorrection - 360 : _arriveMinAngle + _config.CheckpointCorrection;

                    _checkpointAngle = (_cachedStartAngle + 180) % 360;

                    _checkpointMinAngle = _checkpointAngle - _config.CheckpointCorrection < 0 ? _checkpointAngle - _config.CheckpointCorrection + 360 : _checkpointAngle - _config.CheckpointCorrection;
                    _checkpointMaxAngle = _checkpointAngle + _config.CheckpointCorrection > 360 ? _checkpointAngle + _config.CheckpointCorrection - 360 : _checkpointAngle + _config.CheckpointCorrection;
                }

                _cachedCurAngle = GetAngleBetweenNormals(_vecCenter, pos, _vecCenter + Vector3.right);

                Debug.Log(_cachedCurAngle);

                _trLemon.transform.rotation = Quaternion.AngleAxis(_cachedCurAngle, Vector3.forward);

                if (_isCheckpointReached == false && _checkpointMinAngle < _cachedCurAngle && _checkpointMaxAngle > _cachedCurAngle)
                {
                    _isCheckpointReached = true;
                    Debug.Log("체크포인트 도달");
                }

                if (_isCheckpointReached == true && _arriveMinAngle < _cachedCurAngle && _arriveMaxAngle > _cachedCurAngle)
                {
                    _isCheckpointReached = false;
                    _currentSpinCount++;

                    SoundManager.Instance.Play(ESoundType.SFX, "SqueezeLemon");

                    _trLemonLiquid.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 1), Vector3.one, (float)_currentSpinCount / _requireSpinCount);

                    Debug.Log($"{_currentSpinCount}, {_requireSpinCount}");

                    if (_currentSpinCount >= _requireSpinCount)
                    {
                        // 일단 미니게임 일시정지
                        MiniGameManager.OnMiniGamePause.Invoke();

                        // 연출 보여줄거면 보여주고 게임 종료
                        StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
                        return;
                    }
                }
            }
        }
    }
}
