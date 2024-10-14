using System.Collections;
using System.Collections.Generic;
using PlayJam.InGame.UI;
using PlayJam.Utils;
using UnityEngine;

namespace PlayJam.InGame
{
    /// <summary>
    /// UI 관리
    /// </summary>
    public class MiniGameUIController : BaseMiniGameController
    {
        [SerializeField]
        private MiniGameTimerUI _timerUI;

        [SerializeField]
        private MiniGameTransitionUI _transitionUI;

        private List<BaseMiniGameUI> _miniGameUIs;

        public override void Initialize()
        {
            _miniGameUIs = new List<BaseMiniGameUI>
            {
                _timerUI,
                _transitionUI
            };

            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].Initialize();
            }

            MiniGameManager.OnMiniGameStart.AddListener(OnMiniGameStart);
            MiniGameManager.OnMiniGamePause.AddListener(OnMiniGamePause);
            MiniGameManager.OnMiniGameResume.AddListener(OnMiniGameResume);
            MiniGameManager.OnMiniGameEnd.AddListener(OnMiniGameEnd);
        }

        public void OnMiniGameStart()
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].OnMiniGameStart();
            }
        }

        public void OnMiniGamePause()
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].OnMiniGamePause();
            }
        }

        public void OnMiniGameResume()
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].OnMiniGameResume();
            }
        }

        public void OnMiniGameEnd(bool isSuccess)
        {
            StartCoroutine(Co_OnMiniGameEnd(isSuccess));
        }

        private IEnumerator Co_OnMiniGameEnd(bool isSuccess)
        {
            List<WaitForSignal> flags = new List<WaitForSignal>();

            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_miniGameUIs[i].Co_OnMiniGameEnd(isSuccess, flag.Signal));
            }

            for (int i = 0; i < flags.Count; i++)
            {
                yield return flags[i].Wait();
            }

            MiniGameManager.OnMiniGamePrevStart.Invoke();
        }

        public override void Clear()
        {
            _miniGameUIs.ForEach(x => x.Clear());
            _miniGameUIs = null;
        }
    }
}