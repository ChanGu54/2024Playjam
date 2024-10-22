using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace PlayJam.InGame.CuttingCabbage
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CuttingPrefab
    {
        public Transform Transform;
        public SpriteRenderer SpriteRend;
        public CuttingCabbageData.EElement Element;
        public List<Rigidbody2D> RBParticles;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CuttingCabbagePlayer : MiniGamePlayer
    {
        [SerializeField]
        private Animator _knifeAnimator;

        [SerializeField]
        private Animator _targetAnimator;

        [SerializeField]
        private List<CuttingPrefab> _cuttingPrefabs;

        [SerializeField]
        private CuttingPrefab _selectedCuttingPrefab;

        [SerializeField]
        private Transform _trRBFXParent;

        private CuttingCabbageData _config;

        private CuttingCabbageTarget _target;

        private int _currentCuttingCount;

        private TweenerCore<Vector3, Vector3, VectorOptions> _tween;

        public override void Clear()
        {
            base.Clear();

            _config = null;
            _target = null;
            _currentCuttingCount = 0;

            if (_tween != null)
            {
                if (_tween.IsActive() == true)
                    _tween.Kill();

                _tween = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as CuttingCabbageData;

            if (_config == null)
                return;

            for (int i = _config.Targets.Count - 1; i >= 0; i--)
            {
                CuttingCabbageTarget target = _config.Targets[i];
                if (target.AppeaeredStage > MiniGameSharedData.Instance.StageCount)
                    continue;

                _target = target;
                break;
            }

            if (_target == null)
                return;

            for (int i = 0; i < _cuttingPrefabs.Count; i++)
            {
                if (_target.Element == _cuttingPrefabs[i].Element)
                {
                    _selectedCuttingPrefab = _cuttingPrefabs[i];
                    _selectedCuttingPrefab.Transform.gameObject.SetActive(false);
                }

                _cuttingPrefabs[i].Transform.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            _selectedCuttingPrefab.Transform.gameObject.SetActive(true);
            _selectedCuttingPrefab.Transform.localPosition = Vector3.zero;
            _targetAnimator.Play("APPEAR");

            yield return null;
            yield return new WaitForSeconds(_targetAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

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

            bool hasValidTouch = false;

            for (int i = 0; i < touches.Length; i++)
            {
                Touch touch = touches[i];

                if (touch.phase != TouchPhase.Began)
                {
                    continue;
                }

                Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

                if (MiniGameSharedData.Instance.ContainsGameScreen(pos))
                {
                    hasValidTouch = true;
                    break;
                }
            }

            if (hasValidTouch == true)
            {
                StartCoroutine(CutFruit());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator CutFruit()
        {
            // 이미 클리어 연출 중
            if (_currentCuttingCount >= _target.CuttingCount)
                yield break;

            _currentCuttingCount++;

            float rate = _currentCuttingCount / (float)_target.CuttingCount;
            float rectSizeX = _selectedCuttingPrefab.SpriteRend.sprite.rect.width;
            float posX = rectSizeX * rate;

            _knifeAnimator.Play("Cut");
            yield return null;
            float animLength = _knifeAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            List<Rigidbody2D> cachedRigidBodies = new List<Rigidbody2D>();

            for (int i = 0; i < _selectedCuttingPrefab.RBParticles.Count; i++)
            {
                Rigidbody2D rbFX = Instantiate(_selectedCuttingPrefab.RBParticles[i].gameObject, _trRBFXParent).GetComponent<Rigidbody2D>();
                cachedRigidBodies.Add(rbFX);
                rbFX.gameObject.SetActive(true);
                rbFX.transform.localPosition = Vector3.zero;
                rbFX.velocity = Vector2.zero;
                float randX = UnityEngine.Random.Range(100, 300);
                float randY = UnityEngine.Random.Range(2000, 3000);
                rbFX.velocity = new Vector2(randX, randY);
            }

            if (_tween != null && _tween.IsComplete() == false)
                _tween.Kill();

            _tween = _selectedCuttingPrefab.Transform.DOLocalMove(new Vector3(posX, 0, 0), animLength).SetEase(Ease.Linear);

            bool isClearStage = false;

            if (_currentCuttingCount == _target.CuttingCount)
                isClearStage = true;

            yield return new WaitForSeconds(animLength);

            if (isClearStage == true)
            {
                // 일단 미니게임 일시정지
                MiniGameManager.OnMiniGamePause.Invoke();

                // 연출 보여줄거면 보여주고 게임 종료
                StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
            }

            yield return new WaitForSeconds(3f);

            for (int i = 0; i < cachedRigidBodies.Count; i++)
                Destroy(cachedRigidBodies[i].gameObject);
        }
    }
}
