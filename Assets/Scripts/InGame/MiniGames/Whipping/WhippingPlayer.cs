using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlayJam.Character;
using static UnityEngine.EventSystems.EventTrigger;

namespace PlayJam.InGame.Whipping
{
    /// <summary>
    /// 
    /// </summary>
    public class WhippingPlayer : MiniGamePlayer
    {
        [SerializeField]
        private Transform _trWhip;

        [SerializeField]
        private Transform _trCenter;

        [SerializeField]
        private List<GameObject> _whipLevelObjs;

        private WhippingData _config;

        private int _currentSpinCount;

        private int _requireSpinCount;

        private float radiusX = 62f;

        private float radiusY = 32f;

        public override void Clear()
        {
            base.Clear();

            _config = null;
            _currentSpinCount = 0;
            _requireSpinCount = 0;

            for (int i = 0; i < _whipLevelObjs.Count; i++)
            {
                _whipLevelObjs[i].SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as WhippingData;

            if (_config == null)
                return;

            _requireSpinCount = _config.WhippingCount + _config.IncreaseCountPerStageCount * MiniGameSharedData.Instance.StageCount;

            for (int i = 0; i < _whipLevelObjs.Count; i++)
            {
                if (i == 0)
                    _whipLevelObjs[i].SetActive(true);
                else
                    _whipLevelObjs[i].SetActive(false);
            }

            _trWhip.localPosition = new Vector3(134, -393, -2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            yield return new WaitForSeconds(0.5f);

            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        /// <summary>
        /// 타원의 좌표 구하기
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private Vector3 GetEllipsePointAtAngle(float angle)
        {
            // 각도를 라디안 값으로 변환
            float radians = angle * Mathf.Deg2Rad;

            // 타원의 테두리 좌표 계산
            float x = _trCenter.position.x + radiusX * Mathf.Cos(radians);
            float y = _trCenter.position.y + radiusY * Mathf.Sin(radians);

            return new Vector3(x, y, 0f);
        }

        Vector3 _touchStartPos = Vector3.zero;
        float _cachedStartAngle = 0;
        float _cachedCurAngle = 0;

        public float GetAngleBetweenNormals(Vector2 center, Vector2 point1, Vector2 point2)
        {
            // 중심점에서 각 점으로의 벡터 계산
            Vector2 vector1 = point1 - center;
            Vector2 vector2 = point2 - center;
            Vector2 normal1 = vector1.normalized;
            Vector2 normal2 = vector2.normalized;

            // 두 법선 벡터 사이의 각도 계산
            float angle = Vector2.Angle(normal1, normal2);

            if (vector2.y < vector1.y)
                angle = 360 - angle;

            Debug.LogError($"{center}, {vector1}, {vector2}, {angle}");

            // 각도의 절대값을 반환 (항상 양수)
            return angle;
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
                _lastTouchPos = Vector3.zero;
                return;
            }

            Touch[] touches = Input.touches;
#endif
            Touch touch = touches[0];
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                _touchStartPos = pos;
                _cachedCurAngle = 0;
                return;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (_touchStartPos == Vector3.zero)
                    _touchStartPos = pos;

                float angle = GetAngleBetweenNormals(new Vector2(_trCenter.position.x, _trCenter.position.y), _touchStartPos, pos);
                Vector3 whipPos = GetEllipsePointAtAngle(angle);
                _trWhip.transform.localPosition = whipPos;
            }
        }
    }
}
