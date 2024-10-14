using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayJam.InGame
{
    /// <summary>
    /// 미니게임 시간 제어
    /// </summary>
    public class MiniGameTimeController : BaseMiniGameController
    {
        private IEnumerator _timerCoroutine;

        public override void Initialize()
        {
            base.Initialize();

            MiniGameManager.OnMiniGamePostStart.AddListener(StartNewTimer);
            MiniGameManager.OnMiniGamePause.AddListener(PauseTimer);
            MiniGameManager.OnMiniGameResume.AddListener(ResumeTimer);
            MiniGameManager.OnMiniGameEnd.AddListener(EndTimer);
        }

        public void StartNewTimer()
        {
            _timerCoroutine = Co_PlayTimer();
            StartCoroutine(_timerCoroutine);
        }

        public void PauseTimer()
        {
            if (_timerCoroutine == null)
                return;

            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        public void ResumeTimer()
        {
            if (_timerCoroutine != null)
            {
                PauseTimer();
            }

            _timerCoroutine = Co_PlayTimer();
            StartCoroutine(_timerCoroutine);
        }

        public void EndTimer(bool isSuccess)
        {
            if (_timerCoroutine == null)
                return;

            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        public override IEnumerator Co_PrevEndGame(Action inEndCallback)
        {
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            return base.Co_PrevEndGame(inEndCallback);
        }

        private IEnumerator Co_PlayTimer()
        {
            while (MiniGameSharedData.Instance.LeftTime >= 0)
            {
                MiniGameSharedData.Instance.LeftTime = Mathf.Max(0, MiniGameSharedData.Instance.LeftTime - Time.deltaTime);
                yield return null;
            }

            _timerCoroutine = null;
        }
    }
}