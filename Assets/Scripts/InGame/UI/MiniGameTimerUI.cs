using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayJam.InGame.UI
{
    public class MiniGameTimerUI : BaseMiniGameUI
    {
        [SerializeField]
        private Animator _animClock;

        [SerializeField]
        private Image _imgFill;

        private IEnumerator _timerCoroutine;

        public override void OnMiniGameStart()
        {
            base.OnMiniGameStart();
            _animClock.Play("Appear");

            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }

            _timerCoroutine = RefreshTimer();
            StartCoroutine(_timerCoroutine);
        }

        public override IEnumerator Co_OnMiniGameEnd(bool isSuccess, Action inCallback)
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }

            _animClock.Play("Disappear");
            yield return null;
            yield return new WaitForSeconds(_animClock.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            inCallback?.Invoke();
        }

        private IEnumerator RefreshTimer()
        {
            float fillAnount = 0;

            while (fillAnount < 1)
            {
                fillAnount = Mathf.Min(1, 1 - MiniGameSharedData.Instance.LeftTime / MiniGameSharedData.Instance.CurStageTime);
                _imgFill.fillAmount = fillAnount;
                yield return null;
            }

            MiniGameManager.OnMiniGameEnd.Invoke(false);
        }
    }
}