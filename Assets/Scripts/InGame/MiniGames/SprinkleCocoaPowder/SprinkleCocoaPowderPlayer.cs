using DG.Tweening;
using PlayJam.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayJam.InGame.SprinkleCocoaPowder
{
    /// <summary>
    /// 
    /// </summary>
    public class SprinkleCocoaPowder : MiniGamePlayer
    {
        [SerializeField]
        private Collider2D _touchCollider;

        [SerializeField]
        private SpriteRenderer _touchSpriteRenderer;

        [SerializeField]
        private CocoaPowder _powderPrefab;

        [SerializeField]
        private Transform _powderParent;

        [SerializeField]
        private SprinkleCocoaPowderData _config;

        [SerializeField]
        private SpriteRenderer _sprTiraBefore;

        [SerializeField]
        private SpriteRenderer _sprTiraAfter;

        private float _curScore;

        private float _curDegree;

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            _config = null;

            _curScore = 0;
            _curDegree = 0;

            StopAllCoroutines();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as SprinkleCocoaPowderData;

            if (_config == null)
                return;

            _touchSpriteRenderer.transform.rotation = Quaternion.Euler(Vector3.zero);
            _sprTiraBefore.gameObject.SetActive(true);
            _sprTiraAfter.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inCallback"></param>
        /// <returns></returns>
        public override IEnumerator OnSuccess(Action inCallback)
        {
            IsPlaying = false;
            yield return new WaitForSeconds(0.5f);
            inCallback?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            yield return null;
            MiniGameManager.OnMiniGamePostStart.Invoke();
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
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }

            Touch[] touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
                }
            };
#else
            if (Input.touchCount <= 0)
            {
                return;
            }

            Touch[] touches = Input.touches;
#endif

            for (int i = 0; i < touches.Length; i++)
            {
                Touch touch = touches[i];

                if (touch.phase != TouchPhase.Began)
                {
                    continue;
                }

                Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

                // �̹����� �浹 ���� Ȯ��
                Collider2D hit = Physics2D.OverlapPoint(pos);
                if (hit == null)
                    continue;

                Vector3 relativeTouchPos = hit.transform.position - pos;
                if (relativeTouchPos.x == 0)
                    relativeTouchPos.x = 0.1f;

                float normalizedRotateVal = relativeTouchPos.x * 2 / (_touchSpriteRenderer.sprite.rect.width / 2f);
                float moveRotation = _config.MaxRotationDegree * normalizedRotateVal;
                _curDegree = Mathf.Clamp(_curDegree + moveRotation, -_config.MaxRotationDegree, _config.MaxRotationDegree);
                _touchSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _curDegree));
                _curScore += Mathf.Lerp(_config.MinScorePerTouch, _config.MaxScorePerTouch, 1 - Mathf.Abs(_curDegree / _config.MaxRotationDegree));
                PlayPowderEffect();

                if (_curScore >= _config.ClearScore + _config.ClearScoreWeight * MiniGameSharedData.Instance.StageCount)
                {
                    MiniGameManager.OnMiniGamePause.Invoke();
                    _sprTiraBefore.gameObject.SetActive(false);
                    _sprTiraAfter.gameObject.SetActive(true);
                    StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void PlayPowderEffect()
        {
            GameObject powderObj = Instantiate(_powderPrefab.gameObject, _powderParent);
            powderObj.SetActive(true);
            powderObj.transform.localPosition = Vector3.up * 125;

            CocoaPowder cocoaPowder = powderObj.GetComponent<CocoaPowder>();
            cocoaPowder.RendPowder.transform.localPosition = Vector3.up * 186;
            cocoaPowder.RendPowder.transform.DOLocalMoveY(-186, 0.5f).OnComplete(() => Destroy(powderObj));
        }
    }
}
