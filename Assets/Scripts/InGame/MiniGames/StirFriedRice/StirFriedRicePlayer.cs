using DG.Tweening;
using PlayJam.Character;
using PlayJam.Sound;
using PlayJam.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace PlayJam.InGame.StirFriedRice
{
    /// <summary>
    /// 
    /// </summary>
    public class StirFriedRicePlayer : MiniGamePlayer
    {
        [SerializeField]
        private Transform _trCharacterBasePos;

        [SerializeField]
        private StirFriedRiceData _config;

        [SerializeField]
        private Transform _trCircle;

        private MainCharacter _mainCharacter;
        private Tween _tweenCircle;
        private float _circleSpeed;
        private IEnumerator _gaugeCoroutione;
        private Vector3 _gaugePos = new Vector3(-260, 0, 0);

        public override void Clear()
        {
            base.Clear();

            _config = null;

            if (_mainCharacter != null)
            {
                Destroy(_mainCharacter.gameObject);
                _mainCharacter = null;
            }

            StopAllCoroutines();
            _gaugeCoroutione = null;

            _circleSpeed = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as StirFriedRiceData;

            if (_config == null)
                return;

            if (_mainCharacter != null)
            {
                Destroy(_mainCharacter.gameObject);
                _mainCharacter = null;
            }

            _mainCharacter = CharacterManager.Instance.GetMainCharacter(_trCharacterBasePos);
            _mainCharacter.Initialize(EEmotion.IDLE, EArm.IDLE, EBody.APRON, EHat.IDLE, EAddOn.PAN, false);
            _mainCharacter.PlayAnimator(EAnim.IDLE);
            _mainCharacter.transform.localPosition = Vector3.zero;

            _circleSpeed = UnityEngine.Random.Range(_config.CircleMinSpeed + _config.SpeedIncreaseWeight * MiniGameSharedData.Instance.StageCount, _config.CircleMaxSpeed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            yield return null;

            _gaugeCoroutione = Co_PlayGaugeAnim();
            StartCoroutine(_gaugeCoroutione);
            SoundManager.Instance.Play(ESoundType.SFX, "StirFriedRice_Idle");

            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator Co_PlayGaugeAnim()
        {
            _trCircle.transform.localPosition = _gaugePos;
            WaitForSignal flag = new WaitForSignal();
            Vector3 dest = _gaugePos;

            while (true)
            {
                dest = -dest;
                _tweenCircle = _trCircle.transform.DOLocalMove(dest, Mathf.Abs(_gaugePos.x) / _circleSpeed).SetEase(Ease.Linear).OnComplete(flag.Signal);
                yield return flag.Wait();
            }
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

                if (_gaugeCoroutione != null)
                {
                    StopCoroutine(_gaugeCoroutione);
                    _gaugeCoroutione = null;
                }

                _tweenCircle.Kill();
                _tweenCircle = null;

                // 일단 미니게임 일시정지
                MiniGameManager.OnMiniGamePause.Invoke();

                SoundManager.Instance.Play(ESoundType.SFX, "StirFriedRice_Click");

                float circleXPos = _trCircle.transform.position.x;
                if (circleXPos >= -15 && circleXPos <= 15)
                {
                    // 연출 보여줄거면 보여주고 게임 종료
                    StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
                }
                else
                {
                    // 연출 보여줄거면 보여주고 게임 종료
                    StartCoroutine(OnFail(() => MiniGameManager.OnMiniGameEnd.Invoke(false)));
                }
            }
        }

        public override IEnumerator OnSuccess(Action inCallback)
        {
            _mainCharacter.PlayAnimator(EAnim.FRIEDRICE_STIR_FAIL);
            yield return null;
            float duration = _mainCharacter.CharacterAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(duration);
            _mainCharacter.ChangeEmotion(EEmotion.SMILE);
            yield return new WaitForSeconds(1f);

            inCallback?.Invoke();
        }

        public override IEnumerator OnFail(Action inCallback)
        {
            _mainCharacter?.PlayAnimator(EAnim.FRIEDRICE_STIR_SUCCESS);
            yield return null;
            float duration = _mainCharacter.CharacterAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(duration);
            _mainCharacter?.ChangeEmotion(EEmotion.SAD);
            yield return new WaitForSeconds(1f);

            inCallback?.Invoke();
        }
    }
}
